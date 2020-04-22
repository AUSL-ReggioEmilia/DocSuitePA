using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public abstract class ModuleBase : MarshalByRefObject, IModule
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private bool _isWorking = false;
        private readonly object _thisLock = new object();
        private bool _cancel = false;
        private readonly string _identifier;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ModuleBase));
                }
                return _logCategories;
            }
        }

        public string Identifier => _identifier;

        public bool Cancel => _cancel;

        private bool IsWorking
        {
            get
            {
                lock (_thisLock)
                {
                    return _isWorking;
                }
            }
            set
            {
                lock (_thisLock)
                {
                    _isWorking = value;
                }
            }
        }

        public bool IsBusy => IsWorking;
        #endregion

        #region [ Constructor ]
        public ModuleBase(ILogger logger, string identifier)
        {
            _logger = logger;
            _identifier = identifier;
        }
        #endregion

        #region [ Methods ]
        public void Dispose()
        {

        }

        protected abstract void Execute();
        protected abstract void OnStop();

        public void OnExecute()
        {
            if (IsBusy || Cancel)
            {
                return;
            }
            try
            {
                IsWorking = true;
                Execute();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"Execution module {Identifier} error"), ex, LogCategories);
            }
            finally
            {
                _logger.WriteDebug(new LogMessage($"Execution module {Identifier} completed"), LogCategories);
                IsWorking = false;
            }
        }

        public void Stop()
        {
            _cancel = true;
            OnStop();
            _cancel = false;
        }
        #endregion               
    }
}
