using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using VecompSoftware.BPM.Integrations.Model.Configurations;
using VecompSoftware.BPM.Integrations.Modules;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.SignServices;
using VecompSoftware.BPM.Integrations.Services.SignServices.Services;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.ModuleManager
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public class MEFModuleLoader
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;

        private readonly ILogger _logger;
        private readonly GlobalConfiguration _configuration;

        private CompositionContainer _container;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(MEFModuleLoader));
                }
                return _logCategories;
            }
        }

        [ImportMany(typeof(IModule))]
        public IEnumerable<Lazy<IModule>> Modules { get; set; }
        #endregion

        #region [ Constructor ]
        public MEFModuleLoader(ILogger logger, GlobalConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Initialize();
        }
        #endregion

        #region [ Methods ]
        private void Initialize()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            IEnumerable<ModuleConfiguration> activeModules = _configuration.ModuleConfigurations.Where(x => x.Enabled);
            if (!activeModules.Any())
            {
                _logger.WriteWarning(new LogMessage("MEFModuleLoader.Initialize() -> no modules configured"), LogCategories);
                return;
            }

            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            foreach (ModuleConfiguration module in activeModules)
            {
                _logger.WriteInfo(new LogMessage(string.Format("MEFModuleLoader.Initialize() -> configure module {0}", module.Name)), LogCategories);
                catalog.Catalogs.Add(new DirectoryCatalog(module.Path, "*.dll"));
            }

            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeExportedValue(_logger);
                _container.ComposeExportedValue<IWebAPIClient>(new WebAPIClient(_logger));
                _container.ComposeExportedValue<IServiceBusClient>(new ServiceBusClient(_logger));
                _container.ComposeExportedValue<IDocumentClient>(new DocumentClient(_logger));
                _container.ComposeExportedValue<IStampaConformeClient>(new StampaConformeClient(_logger));
                _container.ComposeExportedValue<ISignServiceClient>(new SignService(_logger));
                //_container.ComposeExportedValue<IWorkflowInstanceManager>(new WorkflowInstanceManager(_logger));
                _container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("MEFModuleLoader.Initialize() -> configuration error"), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
