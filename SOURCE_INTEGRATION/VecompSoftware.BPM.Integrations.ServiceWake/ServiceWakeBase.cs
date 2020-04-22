using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Model.Configurations;
using VecompSoftware.BPM.Integrations.Model.Interfaces;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.ServiceWake
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public abstract class ServiceWakeBase : IServiceWake
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private ModuleConfiguration _currentModule;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ServiceWakeBase));
                }
                return _logCategories;
            }
        }
        public ModuleConfiguration CurrentModule => _currentModule;

        #endregion

        #region [ Events ]
        #endregion

        #region [ Constructor ]
        public ServiceWakeBase(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public abstract void OnAddModule();

        public abstract void OnStart(Action executeAction, Action closeAction);

        public abstract void OnStop();

        public void AddModule(ModuleConfiguration module)
        {
            _currentModule = module;
            OnAddModule();
        }

        public void Start(Action executeAction, Action closeAction)
        {
            if (_currentModule == null)
            {
                _logger.WriteError(new LogMessage("ServiceWakeBase.Start() -> module is not defined."), LogCategories);
                throw new ArgumentNullException(nameof(CurrentModule));
            }

            _logger.WriteInfo(new LogMessage("ServiceWakeBase.Start() -> received Start command."), LogCategories);
            try
            {
                _logger.WriteInfo(new LogMessage($"ServiceWakeBase.Start() -> starting module '{_currentModule.Name}'..."), LogCategories);
                OnStart(executeAction, closeAction);
                _logger.WriteInfo(new LogMessage($"ServiceWakeBase.Start() -> module '{_currentModule.Name}' started correctly."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"ServiceWakeBase.Start() -> error starting module '{_currentModule.Name}'."), ex, LogCategories);
            }
        }

        public void Stop()
        {
            if (_currentModule == null)
            {
                _logger.WriteWarning(new LogMessage("ServiceWakeBase.Stop() -> module is not defined."), LogCategories);
                throw new ArgumentNullException(nameof(CurrentModule));
            }

            _logger.WriteInfo(new LogMessage("ServiceWakeBase.Stop() -> received Stop command."), LogCategories);
            try
            {
                _logger.WriteInfo(new LogMessage($"ServiceWakeBase.Stop() -> stopping module '{_currentModule.Name}'..."), LogCategories);
                OnStop();
                _logger.WriteInfo(new LogMessage($"ServiceWakeBase.Stop() -> module '{_currentModule.Name}' has been correctly stopped."), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"ServiceWakeBase.Stop() -> error stopping module '{_currentModule.Name}'."), ex, LogCategories);
            }
        }
        #endregion
    }
}


