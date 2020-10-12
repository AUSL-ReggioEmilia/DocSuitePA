using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Mappers;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.Services.Command.CQRS.Events.Entities.PECMails;
using VecompSoftware.Services.Command.CQRS.Events.Entities.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        private readonly ZENClient _zenClient;
        private readonly ProtocolCreatedEventMapper _mapper_protocolCreatedEvent;
        private readonly ProtocolCanceledEventMapper _mapper_protocolCanceledEvent;
        private readonly PECCreatedEventMapper _mapper_pecCreatedEvent;
        private readonly PECReceiptReceivedEventMapper _mapper_pecReceiptReceivedEvent;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _webAPIClient = webAPIClient;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;
                _zenClient = new ZENClient(logger, _moduleConfiguration.ZENWebApiUrl, _moduleConfiguration.ZENUsername, _moduleConfiguration.ZENPassword);
                _mapper_protocolCreatedEvent = new ProtocolCreatedEventMapper();
                _mapper_protocolCanceledEvent = new ProtocolCanceledEventMapper();
                _mapper_pecCreatedEvent = new PECCreatedEventMapper();
                _mapper_pecReceiptReceivedEvent = new PECReceiptReceivedEventMapper();
                _mapper_collaborationReferenceEvent = new CollaborationReferenceMapper();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("AUSLRE.ZEN -> Critical error in costruction module"), ex, LogCategories);
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
                _logger.WriteError(new LogMessage("AUSLRE.ZEN -> Execute critical error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> AUSLRE.ZEN"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreateProtocol>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.ProtocolCreatedSubscription, EventProtocolCreatedCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCancelProtocol>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.ProtocolCancelSubscription, EventProtocolCanceledCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventCreatePECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.PECMailCreatedSubscription, EventPECCreatedCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventReceivedReceiptPECMail>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.PECMailReceiptReceivedSubscription, EventPECReceiptReceivedCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<IEventShareDocumentUnit>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.WorkflowIntegrationTopic,
                    _moduleConfiguration.WorkflowStartZENShareDocumentUnitSubscription, EventShareDocumentUnitCallbackAsync));

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

        private async Task ExecuteCallbackAsync(IEvent evt, IDSWEntity entity, bool evaluateAggregates, Action<IDSWEntity, IDictionary<string, object>> evaluateEventAction,
            Func<IEvent, DocSuiteEvent> lambdaMapper)
        {
            List<DocSuiteEvent> docSuiteEvents = new List<DocSuiteEvent>();
            int collaborationId = 0;
            try
            {
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
                docSuiteEvents.Add(masterDocSuiteEvent);
                if (evaluateAggregates)
                {
                    List<Collaboration> collaborationAggregates = new List<Collaboration>();
                    collaborationAggregates.AddRange(await _webAPIClient.GetCollaborationAggregatesAsync(collaborationId));
                    _logger.WriteInfo(new LogMessage($"ExecuteCallbackAsync -> found {collaborationAggregates.Count} aggregate collaborations"), LogCategories);
                    docSuiteEvents.AddRange(collaborationAggregates.Select(f =>
                    {
                        Dictionary<string, object> customProperties = new Dictionary<string, object>
                        {
                            { CustomPropertyName.COLLABORATION_ID, f.EntityId },
                            { CustomPropertyName.COLLABORATION_UNIQUE_ID, f.UniqueId }
                        };
                        DocSuiteEvent localDocSuiteEvent = localDocSuiteEvent = masterDocSuiteEvent.Clone() as DocSuiteEvent;
                        localDocSuiteEvent.ReferenceModel = _mapper_collaborationReferenceEvent.Map(customProperties);
                        return localDocSuiteEvent;
                    }));
                }
                foreach (DocSuiteEvent docSuiteEvent in docSuiteEvents)
                {
                    _logger.WriteDebug(new LogMessage($"ExecuteCallbackAsync -> sending to ZEN.WebAPI with DocSuite EventId {docSuiteEvent.UniqueId}"), LogCategories);
                    await _zenClient.SendEventAsync(docSuiteEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("ExecuteAsync -> error on manage ZEN event"), ex, LogCategories);
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
                await ExecuteCallbackAsync(evt, protocol, true,
                    (entity, customProperties) => EvaluateProtocolEvents(entity as Protocol, customProperties),
                    (@event) => _mapper_protocolCreatedEvent.Map(@event as IEventCreateProtocol));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventProtocolCreatedCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventProtocolCanceledCallbackAsync(IEventCancelProtocol evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventProtocolCancelCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                Protocol protocol = evt.ContentType.ContentTypeValue;
                await ExecuteCallbackAsync(evt, protocol, true,
                    (entity, customProperties) => EvaluateProtocolEvents(entity as Protocol, customProperties),
                    (@event) => _mapper_protocolCanceledEvent.Map(@event as IEventCancelProtocol));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventProtocolCancelCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventPECCreatedCallbackAsync(IEventCreatePECMail evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventPECCreatedCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                PECMail pecMail = evt.ContentType.ContentTypeValue;
                await ExecuteCallbackAsync(evt, pecMail, false,
                    (entity, customProperties) => EvaluatePECMailEvents(entity as PECMail, customProperties),
                    (@event) => _mapper_pecCreatedEvent.Map(@event as IEventCreatePECMail));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventPECCreatedCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        private async Task EventPECReceiptReceivedCallbackAsync(IEventReceivedReceiptPECMail evt, IDictionary<string, object> properties)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage($"EventPECReceiptReceivedCallbackAsync -> received callback with event id {evt.Id}"), LogCategories);

            try
            {
                PECMail pecMail = evt.ContentType.ContentTypeValue;
                if (pecMail.MailType == "accettazione" || pecMail.MailType == "presa-in-carico")
                {
                    _logger.WriteWarning(new LogMessage($"EventPECReceiptReceivedCallbackAsync -> ignore receipt id {pecMail.EntityId} with mail type '{pecMail.MailType}'"), LogCategories);
                    return;
                }

                await ExecuteCallbackAsync(evt, pecMail, false,
                    (entity, customProperties) => EvaluatePECMailEvents(entity as PECMail, customProperties),
                    (@event) => _mapper_pecReceiptReceivedEvent.Map(@event as IEventReceivedReceiptPECMail));
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventPECReceiptReceivedCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
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

        private void EvaluatePECMailEvents(PECMail pecMail, IDictionary<string, object> properties)
        {
            bool invalidateEvaluation = false;
            if (pecMail == null)
            {
                _logger.WriteWarning(new LogMessage("EvaluatePECMailEvents -> pecMail argument not found."), LogCategories);
                invalidateEvaluation = true;
            }

            if (!properties.Any(x => x.Key.Equals(CustomPropertyName.PROTOCOL_YEAR, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WriteWarning(new LogMessage($"EvaluatePECMailEvents -> property {CustomPropertyName.PROTOCOL_YEAR} not found"), LogCategories);
                invalidateEvaluation = true;
            }

            if (!properties.Any(x => x.Key.Equals(CustomPropertyName.PROTOCOL_NUMBER, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WriteWarning(new LogMessage($"EvaluatePECMailEvents -> property {CustomPropertyName.PROTOCOL_NUMBER} not found"), LogCategories);
                invalidateEvaluation = true;
            }

            if (!properties.Any(x => x.Key.Equals(CustomPropertyName.PROTOCOL_UNIQUE_ID, StringComparison.InvariantCultureIgnoreCase)))
            {
                _logger.WriteWarning(new LogMessage($"EvaluatePECMailEvents -> property {CustomPropertyName.PROTOCOL_UNIQUE_ID} not found"), LogCategories);
                invalidateEvaluation = true;
            }

            if (invalidateEvaluation)
            {
                _logger.WriteError(new LogMessage("Errore: l'evento non ha tutte le informazioni necessarie alla corretta gestione verso ZEN"), LogCategories);
                throw new Exception("Errore: l'evento non ha tutte le informazioni necessarie alla corretta gestione verso ZEN");
            }
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
                Protocol protocol = (await _webAPIClient.GetProtocolAsync($"$filter=UniqueId eq {documentUnit.UniqueId}&$expand=ProtocolContacts($expand=Contact),ProtocolContactManuals,AdvancedProtocol,ProtocolType")).SingleOrDefault();
                if (protocol == null)
                {
                    _logger.WriteError(new LogMessage($"Protocol {documentUnit.Title}/{documentUnit.UniqueId} not exist"), LogCategories);
                    throw new ArgumentNullException("Protocol", $"Protocol {documentUnit.Title}/{documentUnit.UniqueId} not exist");
                }

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
                string sender = string.Join(", ", protocol.ProtocolContacts
                    .Where(f => f.ComunicationType == ComunicationType.Sender)
                    .Select(f => f.Contact.Description.Replace("|", " "))
                    .Union(protocol.ProtocolContactManuals
                                   .Where(f => f.ComunicationType == ComunicationType.Sender)
                                   .Select(f => f.Description.Replace("|", " "))));

                docSuiteEvent.EventModel.CustomProperties = new Dictionary<string, string>
                {
                    { "ExecutorUser", evt.Identity.User },
                    { "ProtocolSubject", documentUnit.Subject },
                    { "ProtocolNote", protocol.AdvancedProtocol?.Note},
                    { "ProtocolDirection", ((ProtocolTypology)protocol.ProtocolType.EntityShortId).ToString()},
                    { "ProtocolSender", sender},
                    { "ProtocolCategoryCode", documentUnit.Category.FullCode},
                    { "ProtocolCategoryDescription", documentUnit.Category.Name}
                };
                await _zenClient.SendEventAsync(docSuiteEvent);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage($"EventShareDocumentUnitCallbackAsync -> error occouring when calling ZEN service for event id {evt.Id}"), ex, LogCategories);
                throw;
            }
        }

        #endregion
    }
}
