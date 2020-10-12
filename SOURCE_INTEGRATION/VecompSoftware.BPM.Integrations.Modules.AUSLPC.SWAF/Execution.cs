using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.EventBuilders;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models;
using VecompSoftware.BPM.Integrations.Services.BiblosDS;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.StampaConforme;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Services.Command.CQRS.Events.Models.Integrations.GenericProcesses;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly SWAFClient _swafClient;
        private readonly EventBuilder _eventBuilder;
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
        public Execution(ILogger logger, IWebAPIClient webAPIClient, IServiceBusClient serviceBusClient, IDocumentClient documentClient, IStampaConformeClient stampaConformeClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _webAPIClient = webAPIClient;
                _serviceBusClient = serviceBusClient;
                _swafClient = new SWAFClient(logger, _moduleConfiguration.SWAFAPIUrl);
                _eventBuilder = new EventBuilder(webAPIClient, documentClient, stampaConformeClient, _moduleConfiguration);
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLPC.SWAF -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
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
                _logger.WriteError(new LogMessage("AUSLPC.SWAF -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLPC.SWAF"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventIntegrationRequest>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartSWAFNotificationSubscription, EventSWAFNotificationCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartShareToSWAFNotificationSubscription, EventShareToSWAFNotificationCallbackAsync));
                _needInitializeModule = false;
            }
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

        private async Task EventSWAFNotificationCallbackAsync(IEventIntegrationRequest evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventSWAFNotificationCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                DocSuiteEvent @event = evt.ContentType.ContentTypeValue;
                if (@event.EventModel == null)
                {
                    _logger.WriteWarning(new LogMessage($"Event id {evt.Id} is not evaluated correctly, EventModel is null."), LogCategories);
                    throw new Exception($"Event id {evt.Id} is not evaluated correctly, EventModel is null.");
                }

                bool eventEvaluated = EvaluateSWAFEvent(@event);
                if (!eventEvaluated)
                {
                    _logger.WriteWarning(new LogMessage($"Event id {evt.Id} is not evaluated correctly, EventType or FiscalCode not found on CustomProperties."), LogCategories);
                    throw new Exception($"Event id {evt.Id} is not evaluated correctly, EventType or FiscalCode not found on CustomProperties.");
                }

                string notificationType = @event.EventModel.CustomProperties.Single(x => x.Key.Equals("SWAFEventType")).Value;
                _logger.WriteDebug(new LogMessage($"Send notification {notificationType} to SWAF"), LogCategories);
                await _swafClient.SendEventAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventSWAFNotificationCallbackAsync -> error occouring when calling SWAF service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private bool EvaluateSWAFEvent(DocSuiteEvent @event)
        {
            bool evaluated = @event.EventModel.CustomProperties.Any(x => x.Key.Equals("SWAFEventType") && !string.IsNullOrEmpty(x.Value))
                && @event.EventModel.CustomProperties.Any(x => x.Key.Equals("FiscalCode") && !string.IsNullOrEmpty(x.Value));
            return evaluated;
        }

        private async Task EventShareToSWAFNotificationCallbackAsync(IEventShareDocumentUnit evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventShareToSWAFNotificationCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                DocumentUnit documentUnit = evt.ContentType.ContentTypeValue;
                documentUnit = await _webAPIClient.GetDocumentUnitAsync(documentUnit.UniqueId);
                if (documentUnit == null)
                {
                    _logger.WriteError(new LogMessage($"DocumentUnit with id {documentUnit.UniqueId} not found."), LogCategories);
                    throw new Exception($"DocumentUnit with id {documentUnit.UniqueId} not found.");
                }

                _logger.WriteDebug(new LogMessage($"Build event for DocumentUnit {documentUnit.UniqueId} ..."), LogCategories);
                DocSuiteEvent @event = _eventBuilder.CreateSwafEvent(documentUnit);
                _logger.WriteDebug(new LogMessage($"Send notification [NewDocument] for DocumentUnit {documentUnit.UniqueId} to SWAF"), LogCategories);
                await _swafClient.SendEventAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventShareToSWAFNotificationCallbackAsync -> error occouring when calling SWAF service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
