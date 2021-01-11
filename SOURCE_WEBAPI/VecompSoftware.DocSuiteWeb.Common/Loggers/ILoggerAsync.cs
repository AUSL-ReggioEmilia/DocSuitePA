using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Common.Loggers
{
    public interface ILoggerAsync : ILogger
    {
        Task WriteCriticalAsync(LogMessage message, Exception ex, LogCategory category);
        Task WriteCriticalAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        Task WriteCriticalAsync(LogMessage message, LogCategory category);
        Task WriteCriticalAsync(LogMessage message, IEnumerable<LogCategory> categories);
        Task WriteCriticalAsync(Exception ex, LogCategory category);
        Task WriteCriticalAsync(Exception ex, IEnumerable<LogCategory> categories);
        Task WriteErrorAsync(LogMessage message, Exception ex, LogCategory category);
        Task WriteErrorAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        Task WriteErrorAsync(LogMessage message, LogCategory category);
        Task WriteErrorAsync(LogMessage message, IEnumerable<LogCategory> categories);
        Task WriteErrorAsync(Exception ex, LogCategory category);
        Task WriteErrorAsync(Exception ex, IEnumerable<LogCategory> categories);
        Task WriteWarningAsync(LogMessage message, Exception ex, LogCategory category);
        Task WriteWarningAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        Task WriteWarningAsync(LogMessage message, LogCategory category);
        Task WriteWarningAsync(LogMessage message, IEnumerable<LogCategory> categories);
        Task WriteWarningAsync(Exception ex, LogCategory category);
        Task WriteWarningAsync(Exception ex, IEnumerable<LogCategory> categories);
        Task WriteDebugAsync(LogMessage message, LogCategory category);
        Task WriteDebugAsync(LogMessage message, IEnumerable<LogCategory> categories);
        Task WriteInfoAsync(LogMessage message, LogCategory category);
        Task WriteInfoAsync(LogMessage message, IEnumerable<LogCategory> categories);

    }
}
