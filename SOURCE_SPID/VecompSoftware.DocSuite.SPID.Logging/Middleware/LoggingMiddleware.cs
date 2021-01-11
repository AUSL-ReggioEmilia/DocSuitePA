using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using VecompSoftware.DocSuite.SPID.Logging.Configurations;
using System.IO;
using VecompSoftware.DocSuite.SPID.Model.Logs;
using VecompSoftware.DocSuite.SPID.Logging.Core;
using System.Collections.Generic;
using VecompSoftware.DocSuite.SPID.Common.Extensions;
using VecompSoftware.DocSuite.SPID.Logging.Extensions;

namespace VecompSoftware.DocSuite.SPID.Logging.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingService _loggingService;
        public LoggingMiddleware(RequestDelegate next, ILoggingService loggingService)
        {
            _next = next;
            _loggingService = loggingService;
        }

        public async Task Invoke(HttpContext context)
        {
            string url = context.Request.GetDisplayUrl();
            if (LoggingUrlHelpers.IsLoggingUrl(url))
            {
                ProcessRequest(context);
                return;
            }

            await _next(context);
        }

        private void ProcessRequest(HttpContext context)
        {
            IHeaderDictionary headers = context.Request.Headers;
            string urlReferrer = headers.SafeGet("Referer");
            string url = context.Request.GetDisplayUrl();

            LogRequest logRequestBase = new LogRequest()
            {
                UserAgent = headers.SafeGet("User-Agent"),
                UserHostAddress = context.Connection.RemoteIpAddress.ToString(),
                Url = urlReferrer ?? url,
                Headers = headers.ConvertHeadersToDictionary()
            };

            string httpMethod = context.Request.Method;

            string json;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            LogResponse response = _loggingService.ProcessLogRequest(json, logRequestBase, httpMethod);
            ToHttpResponse(response, context.Response);            
        }

        private void ToHttpResponse(LogResponse logResponse, HttpResponse owinResponse)
        {
            owinResponse.StatusCode = logResponse.StatusCode;

            foreach (KeyValuePair<string, string> kvp in logResponse.Headers)
            {
                owinResponse.Headers[kvp.Key] = kvp.Value;
            }

            owinResponse.ContentType = "text/plain";
            owinResponse.ContentLength = 0;
        }
    }
}
