using VecompSoftware.DocSuite.SPID.Model.Logs;

namespace VecompSoftware.DocSuite.SPID.Logging.Core
{
    public interface ILoggingService
    {
        LogResponse ProcessLogRequest(string json, LogRequest logRequest, string httpMethod);
    }
}