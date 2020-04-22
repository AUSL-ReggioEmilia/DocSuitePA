using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.Validations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Filters
{
    public class DSWExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region [ Fields ]

        //private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]
        public DSWExceptionFilterAttribute()
        {
            //_logger = logger;
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
            messages.Add(new ValidationMessageModel() { Message = ex.Message, MessageCode = validationCode });
            return messages;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
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