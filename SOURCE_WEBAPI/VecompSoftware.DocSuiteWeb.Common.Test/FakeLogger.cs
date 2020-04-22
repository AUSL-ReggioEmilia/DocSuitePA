using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.Common.Test
{
    public class FakeLogger : ILogger
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool IsLoggingEnabled => false;

        public void WriteCritical(LogMessage message, Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteCritical(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteCritical(LogMessage message, LogCategory category)
        {
            return;
        }

        public void WriteCritical(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteCritical(Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteCritical(Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteError(LogMessage message, Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteError(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }


        public void WriteError(LogMessage message, LogCategory category)
        {
            return;
        }

        public void WriteError(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteError(Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteError(Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteDebug(LogMessage message, LogCategory category)
        {
            return;
        }

        public void WriteDebug(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteInfo(LogMessage message, LogCategory category)
        {
            return;
        }

        public void WriteInfo(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteWarning(LogMessage message, Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteWarning(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteWarning(LogMessage message, LogCategory category)
        {
            return;
        }

        public void WriteWarning(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public void WriteWarning(Exception ex, LogCategory category)
        {
            return;
        }

        public void WriteWarning(Exception ex, IEnumerable<LogCategory> categories)
        {
            return;
        }

        public Task WriteCriticalAsync(LogMessage message, LogCategory category)
        {
            return null;
        }

        public Task WriteCriticalAsync(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteCriticalAsync(Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteCriticalAsync(Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteErrorAsync(LogMessage message, LogCategory category)
        {
            return null;
        }

        public Task WriteErrorAsync(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteErrorAsync(Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteErrorAsync(Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteDebugAsync(LogMessage message, LogCategory category)
        {
            return null;
        }

        public Task WriteDebugAsync(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteInfoAsync(LogMessage message, LogCategory category)
        {
            return null;
        }

        public Task WriteInfoAsync(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return null;
        }
        public Task WriteWarningAsync(LogMessage message, LogCategory category)
        {
            return null;
        }

        public Task WriteWarningAsync(LogMessage message, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteWarningAsync(Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteWarningAsync(Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteCriticalAsync(LogMessage message, Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteCriticalAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteErrorAsync(LogMessage message, Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteErrorAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }

        public Task WriteWarningAsync(LogMessage message, Exception ex, LogCategory category)
        {
            return null;
        }

        public Task WriteWarningAsync(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            return null;
        }
    }
}
