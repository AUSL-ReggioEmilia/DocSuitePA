using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Filters
{
    [LogCategory(LogCategoryDefinition.GENERAL)]

    public class DSWExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories = null;

        #endregion
        #region [ Properties ]

        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DSWExceptionFilterAttribute));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public DSWExceptionFilterAttribute()
        {
            _logger = (ILogger)UnityConfig.GetConfiguredContainer().GetService(typeof(ILogger));
        }
        #endregion

        #region [ Methods ]

        private List<ValidationMessageModel> FillRecursive(Exception ex, List<ValidationMessageModel> messages, int validationCode)
        {
            if (ex == null)
            {
                return messages;
            }
            messages = FillRecursive(ex.InnerException, messages, validationCode);
            if (ex is OperationCanceledException ope)
            {
                _logger.WriteDebug(new LogMessage($"OperationCanceledException : {ope.TargetSite?.Name}"), LogCategories);
            }
            if (ex is HttpResponseException err && err.Response != null)
            {
                try
                {
                    _logger.WriteDebug(new LogMessage("reading err.Response.Content.ReadAsStringAsync ...."), LogCategories);
                    Task.Run(async () => _logger.WriteError(new LogMessage(await err.Response.Content.ReadAsStringAsync()), LogCategories));
                }
                catch (Exception) {; }
            }
            messages.Add(new ValidationMessageModel() { Message = ex.Message, MessageCode = validationCode });
            return messages;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            
            _logger.WriteError(new LogMessage($"OnException {context.Exception.Message}: {context.Request.GetOwinContext()?.Authentication?.User?.Identity?.Name} {context.Request?.RequestUri}"), context.Exception, LogCategories);
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            List<ValidationMessageModel> messages = new List<ValidationMessageModel>();
            ValidationModel model = new ValidationModel();
            int validationCode = (int)DSWExceptionCode.Invalid;

            responseMessage.ReasonPhrase = null;
            responseMessage.StatusCode = HttpStatusCode.BadRequest;
            if (context.Exception is DSWException)
            {
                DSWException ex = context.Exception as DSWException;

                messages.Add(new ValidationMessageModel() { Message = ex.Message, MessageCode = (int)ex.ExceptionCode });
            }
            else
            {
                messages = FillRecursive(context.Exception, messages, validationCode);
            }

            model.ValidationCode = validationCode;
            model.ValidationMessages = new ReadOnlyCollection<ValidationMessageModel>(messages);
            responseMessage.Content = new StringContent(JsonConvert.SerializeObject(model, JsonSerializerConfig.SerializerSettings));

            context.Response = responseMessage;
        }

        #endregion 
    }
}