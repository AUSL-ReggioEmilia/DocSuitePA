using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Validation;

namespace VecompSoftware.DocSuite.WebAPI.Common.Helpers
{
    public static class ActionHelper
    {
        #region [ Fields ]
        private const int _retry_tentative = 5;
        private static readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(1);
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public static async Task<IHttpActionResult> TryCatchWithLoggerAsync(Func<Task<IHttpActionResult>> func,
            Func<string, BadRequestErrorMessageResult> badRequest,
            Func<HttpStatusCode, ValidationModel, NegotiatedContentResult<ValidationModel>> validationResult,
            Func<InternalServerErrorResult> internalServerError, ILogger logger, IEnumerable<LogCategory> logCategories,
            bool logValidationException = false)
        {
            try
            {
                return await func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is DSWValidationException || ie is DSWException)
                    {
                        return badRequest(ie.Message);
                    }
                    logger.WriteError(ie, logCategories);
                    return internalServerError();
                }
                return badRequest(ae.Message);
            }
            catch (DSWValidationException ex)
            {
                if (logValidationException)
                {
                    foreach (ValidationMessageModel item in ex.ValidationMessages)
                    {
                        logger.WriteWarning(new LogMessage(item.ToString()), logCategories);
                    }
                }
                ValidationModel model = new ValidationModel()
                {
                    ValidationCode = (int)ex.ExceptionCode,
                    ValidationMessages = ex.ValidationMessages
                };
                return validationResult(HttpStatusCode.BadRequest, model);
            }
            catch (DSWException ex)
            {
                return badRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex, logCategories);
                return internalServerError();
            }
        }

        public static TResult TryCatchWithLoggerGeneric<TResult>(Func<TResult> func, ILogger logger,
            IEnumerable<LogCategory> logCategories)
        {
            try
            {
                return func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is DSWValidationException || ie is DSWException)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    logger.WriteError(ie, logCategories);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (DSWValidationException ex)
            {
                ValidationModel model = new ValidationModel
                {
                    ValidationCode = (int)ex.ExceptionCode,
                    ValidationMessages = ex.ValidationMessages
                };
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(model, Defaults.DefaultJsonSerializer)),
                    ReasonPhrase = null,
                    StatusCode = HttpStatusCode.BadRequest
                };
                throw new HttpResponseException(responseMessage);
            }
            catch (DSWSecurityException ex)
            {
                if (ex.ExceptionCode == DSWExceptionCode.SC_NotFoundAccount)
                {
                    ValidationModel model = new ValidationModel
                    {
                        ValidationCode = (int)ex.ExceptionCode
                    };
                    IList<ValidationMessageModel> messages = new List<ValidationMessageModel>
                    {
                        new ValidationMessageModel() { Key = "USER NOT FOUND", Message = ex.Message, MessageCode = (int)ex.ExceptionCode }
                    };
                    model.ValidationMessages = new ReadOnlyCollection<ValidationMessageModel>(messages);
                    HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(model, Defaults.DefaultJsonSerializer)),
                        ReasonPhrase = null,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    throw new HttpResponseException(responseMessage);
                }
                throw ex;
            }
            catch (DSWException ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = null,
                    StatusCode = HttpStatusCode.BadRequest
                };
                throw new HttpResponseException(responseMessage);
            }
            catch (Exception ex)
            {
                if (!(ex is HttpResponseException))
                {
                    logger.WriteError(ex, logCategories);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                throw ex;
            }
        }

        public static void TryCatchWithLoggerGeneric(Action func, ILogger logger,
            IEnumerable<LogCategory> logCategories)
        {
            try
            {
                func();
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is DSWValidationException || ie is DSWException)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    logger.WriteError(ie, logCategories);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (DSWException ex)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = null,
                    StatusCode = HttpStatusCode.BadRequest
                };
                throw new HttpResponseException(responseMessage);
            }
            catch (Exception ex)
            {
                if (!(ex is HttpResponseException))
                {
                    logger.WriteError(ex, logCategories);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                throw ex;
            }
        }

        public static async Task RetryingPolicyActionAsync<T>(Func<Task> func, ILogger logger, IEnumerable<LogCategory> logCategories, int step = 1)
        {
            logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), logCategories);
            if (step >= _retry_tentative)
            {
                logger.WriteError(new LogMessage("VecompSoftware.DocSuite.WebAPI.Common.Helpers: retry policy expired maximum tentatives"), logCategories);
                throw new Exception("WebAPI retry policy expired maximum tentatives");
            }
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, logCategories);
                await Task.Delay(_threadWaiting);
                await RetryingPolicyActionAsync<T>(func, logger, logCategories, ++step);
            }
        }
        #endregion
    }
}
