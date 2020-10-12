using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly AVELCOClient _avelcoClient;
        private readonly ProtocolCreatedEventMapper _mapper_protocolCreatedEvent;
        private readonly CollaborationReferenceMapper _mapper_collaborationReferenceEvent;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webApiClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webApiClient;
                _needInitializeModule = true;
                _avelcoClient = new AVELCOClient(logger, _moduleConfiguration.AVELCOWebApiUrl);
                _mapper_protocolCreatedEvent = new ProtocolCreatedEventMapper(webApiClient);
                _mapper_collaborationReferenceEvent = new CollaborationReferenceMapper();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.AVELCO -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AUSLRE.AVELCO -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLRE.AVELCO"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartAvelcoShareDocumentUnitSubscription, EventShareDocumentUnitCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateProtocol>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartAvelcoProtocolCreatedSubscription, EventProtocolCreatedCallbackAsync));

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

        private async Task EventShareDocumentUnitCallbackAsync(IEventShareDocumentUnit evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventShareDocumentUnitCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                DocumentUnit documentUnit = evt.ContentType.ContentTypeValue;
                _logger.WriteDebug(new LogMessage($"Evaluating documentUnit {documentUnit.Title}/{documentUnit.UniqueId}"), LogCategories);
                Protocol protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {documentUnit.UniqueId}&$expand=ProtocolType")).SingleOrDefault();

                DocSuiteEvent docSuiteEvent = new DocSuiteEvent
                {
                    EventDate = documentUnit.RegistrationDate,
                    WorkflowReferenceId = null,
                    EventModel = new DocSuiteModel()
                    {
                        Title = documentUnit.Title,
                        UniqueId = documentUnit.UniqueId,
                        Year = documentUnit.Year,
                        Number = documentUnit.Number,
                        ModelType = DocSuiteType.Protocol,
                        ModelStatus = DocSuiteStatus.Activated
                    }
                };
                docSuiteEvent.EventModel.CustomProperties = new Dictionary<string, string>
                {
                    { "UFFICIO_TERRITORIALE", JsonConvert.SerializeObject(documentUnit.Container.EntityShortId) }
                };
                if (documentUnit.DocumentUnitRoles.Any())
                {
                    docSuiteEvent.EventModel.CustomProperties.Add("AMBITO", JsonConvert.SerializeObject(documentUnit.DocumentUnitRoles.Select(f => f.UniqueIdRole)));
                }
                if (protocol != null)
                {
                    docSuiteEvent.EventModel.CustomProperties.Add("ProtocolDirection", ((ProtocolTypology)protocol.ProtocolType.EntityShortId).ToString());
                }
                await _avelcoClient.SendEventAsync(docSuiteEvent);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventShareDocumentUnitCallbackAsync -> error occouring when calling AVELCO service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventProtocolCreatedCallbackAsync(IEventCreateProtocol evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventProtocolCreatedCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                Protocol protocol = evt.ContentType.ContentTypeValue;
                _logger.WriteDebug(new LogMessage($"Evaluating documentUnit {protocol.GetTitle()}/{protocol.UniqueId}"), LogCategories);
                protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {protocol.UniqueId}&$expand=ProtocolType")).SingleOrDefault();

                await ExecuteCallbackAsync(evt, protocol, (entity, customProperties) => EvaluateProtocolEvents(entity as Protocol, customProperties),
                    (@event) => _mapper_protocolCreatedEvent.Map(@event as IEventCreateProtocol));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventProtocolCreatedCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }
        private void EvaluateProtocolEvents(Protocol protocol, IDictionary<string, object> properties)
        {
            bool invalidateEvaluation = false;
            if (protocol == null)
            {
                _logger.WriteWarning(new LogMessage("EvaluateProtocolEvents -> protocol argument not found."), LogCategories);
                invalidateEvaluation = true;
            }

            if (!properties.Any(x => x.Key.Equals(CustomPropertyName.COLLABORATION_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WriteWarning(new LogMessage($"EvaluateProtocolEvents -> property {CustomPropertyName.COLLABORATION_ID} not found"), LogCategories);
                invalidateEvaluation = true;
            }

            if (!properties.Any(x => x.Key.Equals(CustomPropertyName.COLLABORATION_UNIQUE_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WriteWarning(new LogMessage($"EvaluateProtocolEvents -> property {CustomPropertyName.COLLABORATION_UNIQUE_ID} not found"), LogCategories);
                invalidateEvaluation = true;
            }

            if (invalidateEvaluation)
            {
                _logger.WriteError(new LogMessage("Errore: l'evento non ha tutte le informazioni necessarie alla corretta gestione verso ZEN"), LogCategories);
                throw new Exception("Errore: l'evento non ha tutte le informazioni necessarie alla corretta gestione verso ZEN");
            }
        }

        private async Task ExecuteCallbackAsync(IEvent evt, IDSWEntity entity, Action<IDSWEntity, IDictionary<string, object>> evaluateEventAction,
            Func<IEvent, DocSuiteEvent> lambdaMapper)
        {
            List<DocSuiteEvent> docSuiteEvents = new List<DocSuiteEvent>();
            try
            {
                int collaborationId = 0;
                _logger.WriteDebug(new LogMessage($"ExecuteCallbackAsync -> evaluate event id {evt.Id}"), LogCategories);
                if (!evt.CustomProperties.ContainsKey(CustomPropertyName.COLLABORATION_ID) ||
                    !int.TryParse(evt.CustomProperties[CustomPropertyName.COLLABORATION_ID].ToString(), out collaborationId))
                {
                    throw new ArgumentNullException($"Undefined {CustomPropertyName.COLLABORATION_ID} property in event properties");
                }
                _logger.WriteDebug(new LogMessage($"ExecuteCallbackAsync -> CollaborationId {collaborationId}"), LogCategories);
                evaluateEventAction(entity, evt.CustomProperties);
                _logger.WriteDebug(new LogMessage($"ExecuteCallbackAsync -> event id {evt.Id} evaluated"), LogCategories);
                _logger.WriteInfo(new LogMessage($"ExecuteCallbackAsync -> execuiting entity {entity.GetType()} mapping with uniqueId {entity.UniqueId} created by {entity.RegistrationUser}"), LogCategories);
                DocSuiteEvent masterDocSuiteEvent = lambdaMapper(evt);
                WorkflowActivity currentCollaborationActivity = (await _webAPIClient.GetWorkflowActivitiesByPropertyAsync(WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID, collaborationId)).SingleOrDefault();
                if (currentCollaborationActivity != null)
                {
                    _logger.WriteDebug(new LogMessage($"ExecuteCallbackAsync -> found workflow instance reference {currentCollaborationActivity.WorkflowInstance.InstanceId}"), LogCategories);
                    masterDocSuiteEvent.WorkflowReferenceId = currentCollaborationActivity.WorkflowInstance.InstanceId;
                }
                docSuiteEvents.Add(masterDocSuiteEvent);
                foreach (DocSuiteEvent docSuiteEvent in docSuiteEvents)
                {
                    await _avelcoClient.SendEventAsync(docSuiteEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ExecuteAsync -> error on manage AVELCO event"), ex, LogCategories);
                throw;
            }
        }
        #endregion
    }
}
