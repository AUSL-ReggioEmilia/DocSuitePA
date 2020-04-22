using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Common.Loggers
{
    public interface ILogger : IDisposable
    {
        bool IsLoggingEnabled { get; }
        void WriteCritical(LogMessage message, Exception ex, LogCategory category);
        void WriteCritical(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        void WriteCritical(LogMessage message, LogCategory category);
        void WriteCritical(LogMessage message, IEnumerable<LogCategory> categories);
        void WriteCritical(Exception ex, LogCategory category);
        void WriteCritical(Exception ex, IEnumerable<LogCategory> categories);
        void WriteError(LogMessage message, Exception ex, LogCategory category);
        void WriteError(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        void WriteError(LogMessage message, LogCategory category);
        void WriteError(LogMessage message, IEnumerable<LogCategory> categories);
        void WriteError(Exception ex, LogCategory category);
        void WriteError(Exception ex, IEnumerable<LogCategory> categories);
        void WriteWarning(LogMessage message, LogCategory category);
        void WriteWarning(LogMessage message, IEnumerable<LogCategory> categories);
        void WriteWarning(LogMessage message, Exception ex, LogCategory category);
        void WriteWarning(LogMessage message, Exception ex, IEnumerable<LogCategory> categories);
        void WriteWarning(Exception ex, LogCategory category);
        void WriteWarning(Exception ex, IEnumerable<LogCategory> categories);
        void WriteDebug(LogMessage message, LogCategory category);
        void WriteDebug(LogMessage message, IEnumerable<LogCategory> categories);
        void WriteInfo(LogMessage message, LogCategory category);
        void WriteInfo(LogMessage message, IEnumerable<LogCategory> categories);
    }
}
