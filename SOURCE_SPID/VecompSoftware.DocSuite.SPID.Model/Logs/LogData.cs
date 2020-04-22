using Microsoft.Extensions.Logging;

namespace VecompSoftware.DocSuite.SPID.Model.Logs
{
    public class LogData
    {
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
