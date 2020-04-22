using Microsoft.Extensions.Logging;

namespace VecompSoftware.DocSuite.SPID.Model.Logs
{
    public class LogClientRequest
    {
        public string LoggerName { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
    }
}
