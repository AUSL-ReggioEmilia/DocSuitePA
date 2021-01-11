using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Core.Command.CQRS.Events.Models.Protocols;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.ServiceBus.BiblosDS;
using VecompSoftware.ServiceBus.BiblosDS.BiblosDS;
using VecompSoftware.ServiceBus.Receiver.Base;
using VecompSoftware.ServiceBus.Receiver.Base.Exceptions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands.Models.Protocols;
using VecompSoftware.Services.Command.CQRS.Events.Models.Protocols;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.DeleteProtocol
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public class Execution : IListenerExecution<ICommandDeleteProtocol>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly BiblosClient _biblosClient;
        private readonly IWebAPIClient _webApiClient;
        protected static IEnumerable<LogCategory> _logCategories = null;
        private readonly JsonSerializerSettings _serializerSettings;

        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
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
        public IDictionary<string, object> Properties { get; set; }
        public EvaluationModel RetryPolicyEvaluation { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        #endregion

        #region [ Constructor ]
        public Execution(ILogger logger, IWebAPIClient webApiClient, BiblosClient biblosClient)
        {
            _logger = logger;
            _biblosClient = biblosClient;
            _webApiClient = webApiClient;
            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

        }
        #endregion

        #region [ Methods ]
        public async Task ExecuteAsync(ICommandDeleteProtocol command)
        {
            Protocol protocol = new Protocol();

            ProtocolBuildModel protocolBuildModel = command.ContentType.ContentTypeValue;
            ProtocolModel protocolModel = protocolBuildModel.Protocol;
            IdWorkflowActivity = protocolBuildModel.IdWorkflowActivity;
            Document toDelete = null;
            try
            {
                if (RetryPolicyEvaluation != null && !string.IsNullOrEmpty(RetryPolicyEvaluation.ReferenceModel))
                {
                    _logger.WriteDebug(new LogMessage("Load reference model from RetryPolicyEvaluation"), LogCategories);
                    protocolModel = JsonConvert.DeserializeObject<ProtocolModel>(RetryPolicyEvaluation.ReferenceModel, _serializerSettings);
                }
                else
                {
                    _logger.WriteDebug(new LogMessage("Generate new RetryPolicyEvaluation model"), LogCategories);
                    RetryPolicyEvaluation = new EvaluationModel();
                }
                _logger.WriteInfo(new LogMessage($"Cancel requested for protocol {protocol.GetTitle()}/{protocol.UniqueId}-{protocol.Object}"), LogCategories);
                _logger.WriteDebug(new LogMessage($"Cancel requested from WorkflowName {protocolBuildModel.WorkflowName} and IdWorkflowActivity {protocolBuildModel.IdWorkflowActivity}"), LogCategories);

                #region Annullamento del protocollo

                //Non so quanto abbia senso farlo anche qui essendo l'ultimo stato ma per sviluppi futuri potrebbe essere utile
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "ENTITY_UPDATED"))
                {
                    protocol.UniqueId = protocolModel.UniqueId;
                    protocol.LastChangedReason = protocolModel.CancelMotivation;
                    protocol.LastChangedUser = protocolBuildModel.RegistrationUser;
                    protocol.WorkflowName = protocolBuildModel.WorkflowName;
                    protocol.IdWorkflowActivity = protocolBuildModel.IdWorkflowActivity;
                    protocol.WorkflowAutoComplete = protocolBuildModel.WorkflowAutoComplete;
                    foreach (IWorkflowAction workflowAction in protocolBuildModel.WorkflowActions)
                    {
                        protocol.WorkflowActions.Add(workflowAction);
                    }

                    protocol = await _webApiClient.DeleteEntityAsync(protocol, DeleteActionType.DeleteProtocol.ToString());
                    _logger.WriteInfo(new LogMessage($"Protocol {protocol.UniqueId} has been canceled"), LogCategories);
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "ENTITY_UPDATED",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set ENTITY_UPDATED RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "ENTITY_UPDATED");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation ENTITY_UPDATED"), LogCategories);
                }

                #endregion


                #region [ Detach main document ]
                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "DELETE_MAIN_DOCUMENT"))
                {
                    Guid mainDocumentId = protocolModel.MainDocument?.DocumentId ?? Guid.Empty;

                    if (mainDocumentId != Guid.Empty)
                    {
                        toDelete = _biblosClient.Document.GetDocumentInfoById(mainDocumentId);
                        if (toDelete != null)
                        {
                            _logger.WriteInfo(new LogMessage($"Protocol - Detach main document, id: {mainDocumentId}"), LogCategories);
                            _biblosClient.Document.DocumentDetach(toDelete);
                        }
                    }
                    else
                    {
                        _logger.WriteWarning(new LogMessage($"Skipping detach main document from protocolo {protocol.UniqueId}"), LogCategories);
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "DELETE_MAIN_DOCUMENT",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set DELETE_MAIN_DOCUMENT RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.First(f => f.Name == "DELETE_MAIN_DOCUMENT");
                    protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                    _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation DELETE_MAIN_DOCUMENT"), LogCategories);
                }
                #endregion

                #region [ Detach attachments with relative ]

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "DELETE_ATTACHMENTS") && protocolModel.Attachments.Any())
                {
                    foreach (DocumentModel attachment in protocolModel.Attachments.Where(f => !f.DocumentId.HasValue && f.DocumentId.Value != Guid.Empty))
                    {
                        toDelete = _biblosClient.Document.GetDocumentInfoById(attachment.DocumentId.Value);
                        if (toDelete != null)
                        {
                            _logger.WriteInfo(new LogMessage($"Protocol - Detach attached document, id: {attachment.DocumentId.Value}"), LogCategories);
                            _biblosClient.Document.DocumentDetach(toDelete);
                        }
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "DELETE_ATTACHMENTS",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set DELETE_ATTACHMENTS RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "DELETE_ATTACHMENTS");
                    _logger.WriteDebug(new LogMessage("RetryPolicyEvaluation DELETE_ATTACHMENTS"), LogCategories);
                    if (protocolStatus != null)
                    {
                        protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                        _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation DELETE_ATTACHMENTS"), LogCategories);
                    }
                }

                #endregion

                #region [ Detach annexes ]

                if (!RetryPolicyEvaluation.Steps.Any(f => f.Name == "DELETE_ANNEXES") && protocolModel.Annexes.Any())
                {
                    foreach (DocumentModel annexedItem in protocolModel.Annexes.Where(f => !f.DocumentId.HasValue && f.DocumentId.Value != Guid.Empty))
                    {
                        toDelete = _biblosClient.Document.GetDocumentInfoById(annexedItem.DocumentId.Value);
                        if (toDelete != null)
                        {
                            _logger.WriteInfo(new LogMessage($"Protocol - Detach annexed document, id: {annexedItem.DocumentId.Value}"), LogCategories);
                            _biblosClient.Document.DocumentDetach(toDelete);
                        }
                    }
                    RetryPolicyEvaluation.Steps.Add(new StepModel()
                    {
                        Name = "DELETE_ANNEXES",
                        LocalReference = JsonConvert.SerializeObject(protocol, _serializerSettings)
                    });
                    _logger.WriteDebug(new LogMessage("Set DELETE_ANNEXES RetryPolicyEvaluation"), LogCategories);
                }
                else
                {
                    StepModel protocolStatus = RetryPolicyEvaluation.Steps.FirstOrDefault(f => f.Name == "DELETE_ANNEXES");
                    if (protocolStatus != null)
                    {
                        protocol = JsonConvert.DeserializeObject<Protocol>(protocolStatus.LocalReference);
                        _logger.WriteDebug(new LogMessage("Load protocol entity from RetryPolicyEvaluation DELETE_ANNEXES"), LogCategories);
                    }
                    _logger.WriteDebug(new LogMessage("RetryPolicyEvaluation DELETE_ANNEXES"), LogCategories);
                }

                #endregion

                #region [ EventCompleteDeleteProtocolBuild ]

                protocolBuildModel.Protocol = protocolModel;
                IEventCompleteProtocolDelete eventDeleteProtocol = new EventCompleteProtocolDelete(Guid.NewGuid(), protocolBuildModel.UniqueId, command.TenantName, command.TenantId,
                    command.TenantAOOId, command.Identity, protocolBuildModel, null);

                if (!await _webApiClient.PushEventAsync(eventDeleteProtocol))
                {
                    _logger.WriteError(new LogMessage($"EventDeleteProtocol {protocol.GetTitle()} has not been sended"), LogCategories);
                    throw new Exception("IEventDeleteProtocol not sent");
                }
                _logger.WriteInfo(new LogMessage($"EventDeleteProtocol {eventDeleteProtocol.Id} has been sended"), LogCategories);

                #endregion
            }
            catch (Exception ex)
            {
                RetryPolicyEvaluation.ReferenceModel = JsonConvert.SerializeObject(protocolModel, _serializerSettings);

                _logger.WriteError(ex, LogCategories);
                throw new ServiceBusEvaluationException(RetryPolicyEvaluation);
            }
        }

        #endregion


    }
}
