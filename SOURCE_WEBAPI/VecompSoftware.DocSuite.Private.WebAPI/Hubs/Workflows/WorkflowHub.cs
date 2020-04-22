using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS;
using VecompSoftware.Core.Command.CQRS.Events.Models.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Documents.Signs;
using VecompSoftware.DocSuiteWeb.Model.Integrations.GenericProcesses;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.DocSuiteWeb.Service.Workflow;
using VecompSoftware.Services.Command.CQRS.Events.Models;

namespace VecompSoftware.DocSuite.Private.WebAPI.Hubs.Workflows
{
    public class WorkflowHub : BaseAuthenticateHub
    {
        #region [ Fields ]

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

        #endregion

        #region [ Constructor ]

        public WorkflowHub()
        {
        }

        #endregion

        #region [ Methods ]

        protected override IList<string> GetSubscriptionNames()
        {
            return _subscriptionNames;
        }

        public async Task SubscribeStartWorkflow(string correlationId, string value, string topicName, string eventName)
        {
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_DONE, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_STATUS_DONE, SendResponseStatusDone);
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_STATUS_ERROR, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_STATUS_ERROR, SendResponseStatusError);
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_INFO, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_INFO, SendResponseNotificationInfo);
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_INFO_MODEL, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_INFO_AS_MODEL, SendResponseNotificationInfoModel);
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_WARNING, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_WARNING, SendResponseNotificationWarning);
            await SubscribeTopicAsync(WorkflowBaseService<WorkflowHub>.WORKFLOW_NOTIFICATION_ERROR, correlationId, WorkflowBaseService<WorkflowHub>.TYPE_WORKFLOW_NOTIFICATION_ERROR, SendResponseNotificationError);
            try
            {
                DocumentManagementRequestModel requestModel = new DocumentManagementRequestModel();
                if (!string.IsNullOrEmpty(value))
                {
                    List<WorkflowReferenceBiblosModel> workflowReferenceBiblosModel = JsonConvert.DeserializeObject<List<WorkflowReferenceBiblosModel>>(value);

                    foreach (WorkflowReferenceBiblosModel wrbm in workflowReferenceBiblosModel)
                    {
                        requestModel.Documents.Add(wrbm);
                        if (!string.IsNullOrEmpty(wrbm.ReferenceModel))
                        {
                            requestModel.UserProfileRemoteSignProperty = JsonConvert.DeserializeObject<RemoteSignProperty>(wrbm.ReferenceModel);
                        }
                    }
                }

                ServiceBusMessage serviceBusMessage = new ServiceBusMessage
                {
                    ChannelName = topicName,
                    ContentType = ServiceBusMessageType.Event,
                    CorrelationId = correlationId,
                    Content = new EventDematerialisationRequest(Guid.Parse(correlationId), ParameterEnvService.CurrentTenantName, ParameterEnvService.CurrentTenantId,
                    new IdentityContext(Context.User.Identity.Name), requestModel)
                };
                serviceBusMessage.Properties[CustomPropertyName.EVENT_NAME] = eventName;
                serviceBusMessage.Properties.Add(CustomPropertyName.CORRELATION_ID, correlationId);
                ServiceBusMessage response = await TopicService.SendToTopicAsync(serviceBusMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteError(new LogMessage($"SendToTopicAsync - {ex.Message}"), ex, LogCategories);
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

        #endregion
    }
}