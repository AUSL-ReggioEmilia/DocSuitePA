using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Workflows
{
    public class WorkflowActionAggregationExecutor : BaseWorkflowActionExecutor<WorkflowActionModel>, IWorkflowActionAggregationExecutor
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        private readonly Func<IDictionary<Type, IWorkflowActionExecutor>> _workflowExecutorsFunc;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowActionAggregationExecutor(ILogger logger, IWebAPIClient webApiClient, Func<IDictionary<Type, IWorkflowActionExecutor>> workflowExecutorsFunc)
            : base(logger, webApiClient)
        {
            _logger = logger;
            _webAPIClient = webApiClient;
            _workflowExecutorsFunc = workflowExecutorsFunc;
        }
        #endregion

        #region [ Methods ]
        public override IEvent BuildEvent(ICommandCQRS command, WorkflowActionModel workflowAction)
        {
            IEvent @event = null;
            foreach (IWorkflowActionExecutor workflowExecutor in _workflowExecutorsFunc().Where(x => workflowAction.GetType() == x.Key).Select(s => s.Value))
            {
                @event = workflowExecutor.BuildEvent(command, workflowAction);
                if (workflowAction.CorrelationId != Guid.Empty)
                {
                    @event.ReplyTo = ServiceBus.ServiceBusClient.WorkflowAggregationTopicName;
                }
            }
            EventWorkflowActionAggregation aggregationEvent = new EventWorkflowActionAggregation(Guid.NewGuid(), workflowAction.UniqueId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, workflowAction, null);
            aggregationEvent.CorrelatedMessages.Add(@event);
            return aggregationEvent;
        }
        #endregion
    }
}
