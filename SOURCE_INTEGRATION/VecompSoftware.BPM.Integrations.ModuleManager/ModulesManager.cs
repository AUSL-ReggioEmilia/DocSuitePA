using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Model.Configurations;
using VecompSoftware.BPM.Integrations.Model.Interfaces;
using VecompSoftware.BPM.Integrations.Modules;
using VecompSoftware.BPM.Integrations.ServiceWake.Periodic;
using VecompSoftware.BPM.Integrations.ServiceWake.Single;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.ModuleManager
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public class ModulesManager
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private static readonly AutoResetEvent _autoEvent = new AutoResetEvent(false);

        private readonly ILogger _logger;
        private readonly GlobalConfiguration _configuration;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ModulesManager));
                }
                return _logCategories;
            }
        }

        public IDictionary<IModule, IServiceWake> LoadedModules { get; private set; }
        #endregion

        #region [ Constructor ]
        public ModulesManager(ILogger logger, GlobalConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            LoadedModules = new Dictionary<IModule, IServiceWake>();
        }
        #endregion

        #region [ Methods ]
        public void InitializeModules()
        {
            try
            {
                _logger.WriteInfo(new LogMessage("ModuleManager.InitializeModulesAsync() -> loading modules..."), LogCategories);
                MEFModuleLoader loader = new MEFModuleLoader(_logger, _configuration);

                if (!loader.Modules.Any())
                {
                    _logger.WriteInfo(new LogMessage("ModuleManager.InitializeModulesAsync() -> no modules loaded..."), LogCategories);
                    return;
                }

                Parallel.ForEach(loader.Modules, module =>
                {
                    IModule currentInstantiatedModule = null;
                    try
                    {
                        IModule instantiatedModule = module.Value;
                        currentInstantiatedModule = instantiatedModule;
                        _logger.WriteInfo(new LogMessage($"ModuleManager.InitializeModulesAsync() -> configuring module '{instantiatedModule.Identifier}'..."), LogCategories);
                        ModuleConfiguration moduleConfiguration = _configuration.ModuleConfigurations.FirstOrDefault(x => x.Name == instantiatedModule.Identifier);
                        if (moduleConfiguration != null)
                        {
                            IServiceWake serviceWake = null;
                            switch (moduleConfiguration.ServiceWakeType)
                            {
                                case ServiceWakeType.Timer:
                                    serviceWake = new PeriodicServiceWake(_logger, _configuration);
                                    break;
                                case ServiceWakeType.Single:
                                    serviceWake = new SingleServiceWake(_logger);
                                    break;
                            }
                            serviceWake.AddModule(moduleConfiguration);
                            serviceWake.Start(() => instantiatedModule.OnExecute(), () => instantiatedModule.Stop());

                            LoadedModules.Add(instantiatedModule, serviceWake);
                        }
                        _logger.WriteInfo(new LogMessage($"ModuleManager.InitializeModulesAsync() -> module '{instantiatedModule.Identifier}' configured correctly"), LogCategories);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteError(new LogMessage($"ModuleManager.InitializeModulesAsync() -> module configuration error: '{currentInstantiatedModule?.Identifier}'"), ex, LogCategories);
                    }
                });
            }
            catch (AggregateException ae)
            {
                foreach (Exception ie in ae.Flatten().InnerExceptions)
                {
                    if (ie is CompositionException)
                    {
                        foreach (CompositionError item in (ie as CompositionException).Errors)
                        {
                            _logger.WriteError(new LogMessage($"CompositionError -> {item.Element.DisplayName} {item.Element.Origin} : {item.Description}"), item.Exception, LogCategories);
                        }
                    }
                }
                _logger.WriteCritical(new LogMessage("ModuleManager.InitializeModulesAsync() -> AggregateException error"), ae, LogCategories);
                throw;
            }
            catch (Exception ex)
            {
                _logger.WriteCritical(new LogMessage($"ModuleManager.InitializeModulesAsync() -> configuration error {ex.GetType()}"), ex, LogCategories);
                throw;
            }
        }

        public void StopModules()
        {
            foreach (KeyValuePair<IModule, IServiceWake> module in LoadedModules)
            {
                try
                {
                    _logger.WriteInfo(new LogMessage($"ModuleManager.StopModules() -> stopping module '{module.Key.Identifier}'..."), LogCategories);
                    module.Value.Stop();
                    module.Key.Stop();
                    _logger.WriteInfo(new LogMessage($"ModuleManager.StopModules() -> module '{module.Key.Identifier}' stopped correctly"), LogCategories);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage($"ModuleManager.StopModules() -> stop module error: '{module.Key.Identifier}'"), ex, LogCategories);
                }
            }
        }

        public async Task WaitStoppedModules()
        {
            await Task.Run(() =>
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(WaitModules), _autoEvent);
                if (_autoEvent.WaitOne(30000))
                {
                    _logger.WriteInfo(new LogMessage("ModuleManager.WaitStoppedModules -> all modules completed. Stopping service"), LogCategories);
                }
                else
                {
                    _logger.WriteInfo(new LogMessage("ModuleManager.WaitStoppedModules -> check completed module timeout. Stopping service"), LogCategories);
                }
            });
        }

        private void WaitModules(object stateInfo)
        {
            while (true)
            {
                if (LoadedModules.All(x => !x.Key.IsBusy))
                {
                    _logger.WriteDebug(new LogMessage("ModuleManager.WaitModules -> all modules completed"), LogCategories);
                    ((AutoResetEvent)stateInfo).Set();
                    break;
                }
                Thread.Sleep(5000);
            }
        }
        #endregion
    }
}
