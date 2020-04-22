using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.DocSuiteWeb.EnterpriseLogging
{
    public class GlobalLogger : ILogger
    {
        #region [ Fields ]
        private bool _disposed = false;
        private static LogWriterFactory _logWriterFactory = null;
        #endregion

        #region [ Properties ]

        public bool IsLoggingEnabled => Logger.Writer != null && Logger.Writer.IsLoggingEnabled();

        #endregion

        #region [ Constructor ]
        public GlobalLogger()
        {
            if (_logWriterFactory == null)
            {
                IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
                _logWriterFactory = new LogWriterFactory(configurationSource);
                Logger.SetLogWriter(_logWriterFactory.Create());
            }
        }
        #endregion

        #region [ Dispose ]
        ~GlobalLogger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                //                Logger.Reset();

                if (disposing)
                {
                }

                _disposed = true;
            }
        }
        #endregion

        #region [ Methods ]
        private void WriteEntry(LogMessage message, IEnumerable<LogCategory> categories, ObjectEventType severity)
        {
            if (!IsLoggingEnabled)
            {
                return;
            }

            Logger.Writer.Write(new LogEntry()
            {
                Categories = categories.Select(c => c.Category).ToList(),
                Message = message.Message,
                Severity = ObjectEventHelper.ConvertToTraceEvent(severity)
            });
        }

        private void WriteException(Exception ex, IEnumerable<LogCategory> categories, ObjectEventType severity)
        {
            if (!IsLoggingEnabled)
            {
                return;
            }

            if (ex == null)
            {
                return;
            }

            WriteException(ex.InnerException, categories, severity);
            WriteEntry(new LogMessage(string.Concat(ex.Message, Environment.NewLine, ex.StackTrace)), categories, severity);
        }

        #region [ Critical ]
        public void WriteCritical(LogMessage message, Exception ex, LogCategory category)
        {
            WriteCritical(message, ex, new Collection<LogCategory> { category });
        }

        public void WriteCritical(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteCritical(message, categories);
            WriteCritical(ex, categories);
        }

        public void WriteCritical(LogMessage message, LogCategory category)
        {
            WriteCritical(message, new Collection<LogCategory> { category });
        }

        public void WriteCritical(LogMessage message, IEnumerable<LogCategory> categories)
        {
            WriteEntry(message, categories, ObjectEventType.Critical);
        }

        public void WriteCritical(Exception ex, LogCategory category)
        {
            WriteCritical(ex, new Collection<LogCategory> { category });
        }

        public void WriteCritical(Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteException(ex, categories, ObjectEventType.Critical);
        }

        #endregion

        #region [ Error ]

        public void WriteError(LogMessage message, Exception ex, LogCategory category)
        {
            WriteError(message, ex, new Collection<LogCategory> { category });
        }

        public void WriteError(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteError(message, categories);
            WriteError(ex, categories);
        }

        public void WriteError(LogMessage message, LogCategory category)
        {
            WriteError(message, new Collection<LogCategory> { category });
        }

        public void WriteError(LogMessage message, IEnumerable<LogCategory> categories)
        {
            WriteEntry(message, categories, ObjectEventType.Error);
        }

        public void WriteError(Exception ex, LogCategory category)
        {
            WriteError(ex, new Collection<LogCategory> { category });
        }

        public void WriteError(Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteException(ex, categories, ObjectEventType.Error);
        }

        #endregion

        #region [ Warning ]

        public void WriteWarning(LogMessage message, Exception ex, LogCategory category)
        {
            WriteWarning(message, ex, new Collection<LogCategory> { category });
        }

        public void WriteWarning(LogMessage message, Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteWarning(message, categories);
            WriteWarning(ex, categories);
        }

        public void WriteWarning(LogMessage message, LogCategory category)
        {
            WriteWarning(message, new Collection<LogCategory> { category });
        }

        public void WriteWarning(LogMessage message, IEnumerable<LogCategory> categories)
        {
            WriteEntry(message, categories, ObjectEventType.Warning);
        }

        public void WriteWarning(Exception ex, LogCategory category)
        {
            WriteWarning(ex, new Collection<LogCategory> { category });
        }

        public void WriteWarning(Exception ex, IEnumerable<LogCategory> categories)
        {
            WriteException(ex, categories, ObjectEventType.Warning);
        }

        #endregion

        #region [ Debug ]

        public void WriteDebug(LogMessage message, LogCategory category)
        {
            WriteDebug(message, new Collection<LogCategory> { category });
        }

        public void WriteDebug(LogMessage message, IEnumerable<LogCategory> categories)
        {
            WriteEntry(message, categories, ObjectEventType.Debug);
        }

        #endregion

        #region [ Info ]

        public void WriteInfo(LogMessage message, LogCategory category)
        {
            WriteInfo(message, new Collection<LogCategory> { category });
        }

        public void WriteInfo(LogMessage message, IEnumerable<LogCategory> categories)
        {
            WriteEntry(message, categories, ObjectEventType.Information);
        }

        #endregion

        #endregion

    }
}
