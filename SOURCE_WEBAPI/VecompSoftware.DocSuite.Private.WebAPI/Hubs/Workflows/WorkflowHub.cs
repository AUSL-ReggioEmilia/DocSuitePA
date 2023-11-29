using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuite.Service.SignalR;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.SignalR;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models;
using VecompSoftware.Helpers.Workflow;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuite.Document;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Hubs.Workflows
{
    public class WorkflowHub : BaseAuthenticateHub
    {
        #region [ Fields ]

        private readonly IWorkflowStartService _workflowStartService;
        private readonly ICQRSMessageMapper _mapper;
		private readonly IDocumentContext<ModelDocument.Document, ArchiveDocument> _documentService;
		private static readonly IList<string> _subscriptionNames = new List<string>
        {
            WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_DONE,
            WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_ERROR,
            WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_INFO,
            WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_WARNING,
            WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_ERROR
        };

        #endregion

        #region [ Const ]
        #endregion

        #region [ Properties ]
        private WorkflowRelay WorkflowRelay { get; }
        #endregion

        #region [ Constructor ]

        public WorkflowHub()
        {
            _workflowStartService = (IWorkflowStartService)UnityConfig.GetConfiguredContainer().GetService(typeof(IWorkflowStartService));
            _mapper = (ICQRSMessageMapper)UnityConfig.GetConfiguredContainer().GetService(typeof(ICQRSMessageMapper));
			_documentService = (IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument>)UnityConfig.GetConfiguredContainer().GetService(typeof(IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument>));

			WorkflowRelay = new WorkflowRelay();
        }

        #endregion

        #region [ Methods ]

        protected override IList<string> GetSubscriptionNames()
        {
            return _subscriptionNames;
        }

        /// <summary>
        /// Creates a <see cref="WorkflowHubSubscriber"/> which contains the actions needed to send client messages with
        /// SignalR
        /// </summary>
        /// <returns></returns>
        private WorkflowHubSubscriber CreateWorkflowHubSubscriber()
        {
            return new WorkflowHubSubscriber(
                    SendResponseStatusDone, SendResponseStatusError, SendResponseNotificationInfo,
                    SendResponseNotificationInfoModel, SendResponseNotificationWarning, SendResponseNotificationError
                );
        }

        public async Task SubscribeResumeWorkflow(string correlationId)
        {
            if (!await SubscriptionConfigurationExistsAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_DONE, correlationId))
            {
                //if trying to resume and the subscription is deleted, send a response via a signalR channel notifying that 
                //the operation he is trying to connect to does not exist
                SendResponseResumeStatus(new MessageWorkflowResume
                {
                    CorrelationId = correlationId,
                    Status = MessageWorkflowResumeStatus.DidNotResume
                });

                return;
            }

            //make sure the connection is added in the _connection list. Otherwise no messages
            //will be sent to the client
            await base.OnReconnected();

            bool successfull = WorkflowRelay.WorkflowRelayResume(correlationId, CreateWorkflowHubSubscriber(), out WorkflowBusSubscriber correlatedInstance);

            if (!successfull)
            {
                await RemoveSubscriptionsForAsync(correlationId);

                //if trying to resume and the subscription is deleted, send a response via a signalR channel notifying that 
                //the operation he is trying to connect to does not exist
                //Reason : when the service bus sends an status done or error, an event is triggered notifying that
                //there is no longer need for WorkflowCorrelatedHubSubscriber and this is null at this stage
                SendResponseResumeStatus(new MessageWorkflowResume
                {
                    CorrelationId = correlationId,
                    Status = MessageWorkflowResumeStatus.DidNotResume
                });

                return;
            }
            else
            {
                SendResponseResumeStatus(new MessageWorkflowResume
                {
                    CorrelationId = correlationId,
                    Status = MessageWorkflowResumeStatus.Resumed
                });
            }
        }

        public async Task SubscribeStartWorkflow(string correlationId, string value)
        {
            WorkflowRelay.WorkflowRelayStart(correlationId, CreateWorkflowHubSubscriber(), out WorkflowBusSubscriber busSubscriber);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_DONE, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_STATUS_DONE, busSubscriber.QueueResponseStatusDone);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_ERROR, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_STATUS_ERROR, busSubscriber.QueueResponseStatusError);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_INFO, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_INFO, busSubscriber.QueueResponseNotificationInfo);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_INFO_MODEL, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_INFO_AS_MODEL, busSubscriber.QueueResponseNotificationInfoModel);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_WARNING, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_WARNING, busSubscriber.QueueResponseNotificationWarning);

            await SubscribeTopicAsync(
                WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_ERROR, correlationId,
                WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_ERROR, busSubscriber.QueueResponseNotificationError);

            busSubscriber.InstanceDone += BusSubscriber_InstanceDone;

            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    Logger.WriteWarning(new LogMessage("Parameter [value] is null or empty"), LogCategories);
                    throw new ArgumentNullException(nameof(value), "Parameter [value] is null or empty");
                }

                WorkflowStart workflowStart = JsonConvert.DeserializeObject<WorkflowStart>(value);
                if (workflowStart.Arguments.ContainsKey(WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS))
                {
					WorkflowReferenceModel dsw_a_ArchiveDocuments = JsonConvert.DeserializeObject<WorkflowReferenceModel>(workflowStart.Arguments[WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS].ValueString, ServiceHelper.SerializerSettings);
                    WorkflowReferenceModel workflowReferenceModel = JsonConvert.DeserializeObject<WorkflowReferenceModel>(workflowStart.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL].ValueString, ServiceHelper.SerializerSettings);
					if (dsw_a_ArchiveDocuments != null)
                    {
						ICollection<ArchiveDocument> archiveDocuments = await ArchiveDocumentsAsync(dsw_a_ArchiveDocuments);
                        workflowReferenceModel.Documents.Clear();
                        foreach (ArchiveDocument archiveDocument in archiveDocuments)
                        {
                            workflowReferenceModel.Documents.Add(new WorkflowReferenceBiblosModel
                            {
                                ArchiveChainId = archiveDocument.IdChain,
                                ArchiveDocumentId = archiveDocument.IdDocument,
                                ArchiveName = archiveDocument.Archive,
                                DocumentName = archiveDocument.Name
                            });
                        }

                        dsw_a_ArchiveDocuments.ReferenceModel = null;
                        workflowStart.Arguments[WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS].ValueString = JsonConvert.SerializeObject(dsw_a_ArchiveDocuments);
                        workflowStart.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL].ValueString = JsonConvert.SerializeObject(workflowReferenceModel);
					}
				}
                EventWorkflowStartRequest @event = new EventWorkflowStartRequest(
                        Guid.NewGuid(),
                        Guid.Parse(correlationId),
                        workflowStart.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME].ValueString,
                        workflowStart.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID].ValueGuid.Value,
                        workflowStart.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID].ValueGuid.Value,
                        new IdentityContext(Context.User.Identity.Name),
                        workflowStart,
                        null);

                ServiceBusMessage serviceBusMessage = _mapper.Map(@event, new ServiceBusMessage());
                ServiceBusMessage response = await TopicService.SendToTopicAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteError(new LogMessage($"SubscribeStartWorkflowUncoupled - {ex.Message}"), ex, LogCategories);
                throw ex;
            }
        }

        public async Task SubscribeCancelWorkflow(string correlationId, string value)
        {
            try
            {
                WorkflowRequestStatus workflowRequestStatus = JsonConvert.DeserializeObject<WorkflowRequestStatus>(value);
                EventWorkflowStartRequestError @event = new EventWorkflowStartRequestError(
                        Guid.NewGuid(),
                        Guid.Parse(correlationId),
                        ParameterEnvService.CurrentTenantName,
                        ParameterEnvService.CurrentTenantId,
                        Guid.NewGuid(),
                        new IdentityContext(Context.User.Identity.Name),
                        workflowRequestStatus,
                        null);

                ServiceBusMessage serviceBusMessage = _mapper.Map(@event, new ServiceBusMessage());
                ///TODO: ripensare utilizzando un diverso tipo di evento, in quanto si è reso necessario specificare nel codice il 
                ///nome della topic, altrimenti, in base alla message.configuration.json, l'evento sarebbe stato inviato nella topic relativa alle notifiche
                serviceBusMessage.ChannelName = "workflow_integration";
                ServiceBusMessage response = await TopicService.SendToTopicAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteError(new LogMessage($"SubscribeCancelWorkflow - {ex.Message}"), ex, LogCategories);
                throw ex;
            }
        }

        private string GetWorkflowStatusMessages(ServiceBusMessage result)
        {
            IEventModel<WorkflowRequestStatus> @event = result.Content as IEventModel<WorkflowRequestStatus>;
            if (@event != null && @event.ContentType != null && @event.ContentType.ContentTypeValue != null)
            {
                return @event.ContentType.ContentTypeValue.Description;
            }
            return string.Empty;
        }

        private string GetWorkflowNotificationMessages(ServiceBusMessage result)
        {
            IEventModel<WorkflowNotification> @event = result.Content as IEventModel<WorkflowNotification>;
            if (@event != null && @event.ContentType != null && @event.ContentType.ContentTypeValue != null)
            {
                return @event.ContentType.ContentTypeValue.Description;
            }
            return string.Empty;
        }

        /// <summary>
        /// This event will replace the default OnDisconnected functionality of the base class. It removes ServiceBus subscriptions and it will also 
        /// remove <see cref="WorkflowBusSubscriber"/> from the relay. If this is not removed and the client has an id in storage, it will try to resume and
        /// wait for a complete message (Which will never arrive). Cleanup is therefore mandatory
        /// </summary>
        /// <param name="obj"></param>
        private void BusSubscriber_InstanceDone(WorkflowBusSubscriberDoneArgs obj)
        {
            //a status done or error was created. 
            lock (WorkflowHubLock.GetLock(obj.Instance.CorrelationId))
            {
                //first make sure subscriptions are removed. The callback are attached to the hub subscriber object
                RemoveSubscriptionsForAsync(obj.Instance.CorrelationId).Wait();
                //wremvoe the subscriber will clean the hub subscriber and other dependencies
                WorkflowRelay.RemoveBusSubscriber(obj.Instance);
            }
        }

        /// <summary>
        /// The default functionality will remove service bus subscribers. In our case we need thos esubscribers to still work
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //prevent hub subscribers to send messages, the client is already disconnected
            lock (WorkflowHubLock.GetLock(Context.QueryString["correlationId"]))
            {
                string correlationId = Context.QueryString["correlationId"];

                //we remove all connections from the workflowhub to the browser
                WorkflowRelay.RemoveHubSubscribers(correlationId);

                //remove the signalR connection Id from the list of connections
                base.RemoveConnection();

                Logger.WriteWarning(new LogMessage("Client disconnected. Workflow service bus listeners remain active. SignalR subscribers are removed."), LogCategories);

                return Task.CompletedTask;
            }
        }

        private void SendResponseResumeStatus(MessageWorkflowResume resumeMsg)
        {
            SendClientResponse(resumeMsg, resumeMsg.CorrelationId, (action, content) => action.workflowResumeStatus(resumeMsg.Status));
        }

        private void SendResponseStatusDone(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowStatusDone(GetWorkflowStatusMessages(result)));
        }

        private void SendResponseStatusError(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowStatusError(GetWorkflowStatusMessages(result)));
        }

        private void SendResponseNotificationInfo(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowNotificationInfo(GetWorkflowNotificationMessages(result)));
        }

        private void SendResponseNotificationInfoModel(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowNotificationInfoAsModel(GetWorkflowNotificationMessages(result)));
        }

        private void SendResponseNotificationWarning(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowNotificationWarning(GetWorkflowNotificationMessages(result)));
        }

        private void SendResponseNotificationError(ServiceBusMessage result)
        {
            SendClientResponse(result, (action, content) => action.workflowNotificationError(GetWorkflowNotificationMessages(result)));
        }

		private async Task<ICollection<ArchiveDocument>> ArchiveDocumentsAsync(WorkflowReferenceModel referenceModel)
		{
			ICollection<ArchiveDocument> results = null;
			WorkflowDocumentModel workflowDocumentModel = JsonConvert.DeserializeObject<WorkflowDocumentModel>(referenceModel.ReferenceModel, ServiceHelper.SerializerSettings);
			if (workflowDocumentModel != null && workflowDocumentModel.Documents.Count > 0)
			{
				Logger.WriteDebug(new LogMessage($"Initialize archive document {workflowDocumentModel.Documents.Count} to store in workflow storage"), LogCategories);
				short workflowLocationId = ParameterEnvService.WorkflowLocationId;
				Location workflowLocation = UnitOfWork.Repository<Location>().Find(workflowLocationId);
				if (workflowLocation == null)
				{
					throw new DSWException($"Workflow Location {workflowLocationId} not found", null, DSWExceptionCode.WF_Mapper);
				}
				results = workflowDocumentModel.Documents
					.Where(f => f.Value != null && f.Value.ContentStream != null && !string.IsNullOrEmpty(f.Value.FileName))
					.Select(f => new ArchiveDocument
					{
						Archive = workflowLocation.ProtocolArchive,
						ContentStream = f.Value.ContentStream,
						Name = f.Value.FileName
					}).ToList();
				if (results.Any())
				{
					Logger.WriteDebug(new LogMessage($"archiving {results.Count} documents to workflow storage"), LogCategories);
					results = await _documentService.InsertDocumentsAsync(results);
				}
			}
			return results;
		}
		#endregion
	}
}