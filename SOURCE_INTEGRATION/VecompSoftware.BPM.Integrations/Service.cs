using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using VecompSoftware.BPM.Integrations.Model.Configurations;
using VecompSoftware.BPM.Integrations.ModuleManager;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;

namespace VecompSoftware.BPM.Integrations
{
    [LogCategory(LogCategoryDefinition.GENERAL)]
    public partial class Service : ServiceBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private static readonly string _path_environment = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Service)).Location);

        private readonly ILogger _logger;
        private readonly ModulesManager _modulesManager;

        private const string CONFIGURATION_FILENAME = "global.configuration.json";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Service));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]       
        public Service()
        {
            InitializeComponent();
            _logger = new GlobalLogger();
            GlobalConfiguration configuration = JsonConvert.DeserializeObject<GlobalConfiguration>(File.ReadAllText(Path.Combine(_path_environment, CONFIGURATION_FILENAME)));
            _modulesManager = new ModulesManager(_logger, configuration);
        }
        #endregion

        #region [ Methods ]
        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _logger.WriteInfo(new LogMessage(string.Concat("OnStart service ", ServiceName)), LogCategories);
            _modulesManager.InitializeModules();
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage(string.Concat("OnStop service ", ServiceName)), LogCategories);
            if (_modulesManager != null)
            {
                _modulesManager.StopModules();
                _modulesManager.WaitStoppedModules().Wait();
            }
        }

        protected override void OnShutdown()
        {
            Stop();
        }

        public void Start()
        {
            OnStart(null);
        }
        #endregion        
    }
}
