using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.Reporting.Configuration;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.Reporting.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Services.Command.CQRS.Events.Models.Workflows;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.Reporting
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;

        #endregion

        #region [ Properties ]

        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }

        #endregion

        #region [ Constructor ]

        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            _logger = logger;
            _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
            _serviceBusClient = serviceBusClient;
            _needInitializeModule = true;
        }

        #endregion

        #region [ Methods ]

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventWorkflowStartRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.TopicWorkflowIntegration, _moduleConfiguration.WorkflowStartEventAnalyzerSubscription, EventWorkflowStartedRequestCallbackAsync));
                _needInitializeModule = false;
            }
        }

        private void SerializeToDisk(EventWorkflowStartRequest eventWorkflowStartRequest)
        {
            string directory = Path.Combine(_moduleConfiguration.WorkflowEventsPath, DateTime.Today.ToString("yyyyMMdd"));
            string filename = Path.Combine(directory, $"{eventWorkflowStartRequest.Id}.json");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filename, JsonConvert.SerializeObject(eventWorkflowStartRequest, ModuleConfigurationHelper.JsonSerializerSettings));
        }

        private async Task EventWorkflowStartedRequestCallbackAsync(IEventWorkflowStartRequest evt)
        {
            await Task.Run(() => SerializeToDisk((EventWorkflowStartRequest)evt));
        }

        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TECMARKET.Reporting -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> TECMARKET.Reporting"), LogCategories);
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        #endregion
    }
}
