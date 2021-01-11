using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using VecompSoftware.DocSuite.SPID.Common.Logging;
using VecompSoftware.DocSuite.SPID.Model.Logs;

namespace VecompSoftware.DocSuite.SPID.Logging.Core
{
    public class LoggingService : ILoggingService
    {
        private readonly ILoggerFactory _loggerFactory;
        public LoggingService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public LogResponse ProcessLogRequest(string json, LogRequest logRequest, string httpMethod)
        {
            LogResponse response = new LogResponse();
            if ((httpMethod != WebRequestMethods.Http.Post) && (httpMethod != "OPTIONS"))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return response;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            if (httpMethod == "OPTIONS")
            {
                response.Headers.Add("Allow", WebRequestMethods.Http.Post);
                return response;
            }

            LogClientRequest clientRequest = JsonConvert.DeserializeObject<LogClientRequest>(json);
            LogData logData = new LogData()
            {
                LoggerName = clientRequest.LoggerName ?? LogCategories.GENERAL,
                LogLevel = clientRequest.LogLevel,
                Message = clientRequest.Message
            };

            Log(logData);
            return response;
        }

        private void Log(LogData logData)
        {
            ILogger logger = _loggerFactory.CreateLogger(logData.LoggerName);
            switch (logData.LogLevel)
            {
                case LogLevel.Trace:
                    logger.LogTrace(logData.Message);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(logData.Message);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(logData.Message);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(logData.Message);
                    break;
                case LogLevel.Error:
                    logger.LogError(logData.Message);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(logData.Message);
                    break;
            }
        }
    }
}
