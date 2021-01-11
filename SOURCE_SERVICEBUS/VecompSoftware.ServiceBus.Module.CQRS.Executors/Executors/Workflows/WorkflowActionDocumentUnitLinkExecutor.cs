using System;
using VecompSoftware.Core.Command.CQRS.Events.Models.Workflows;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Workflows
{
    public class WorkflowActionDocumentUnitLinkExecutor : BaseWorkflowActionExecutor<WorkflowActionDocumentUnitLinkModel>, IWorkflowActionDocumentUnitLinkExecutor
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowActionDocumentUnitLinkExecutor(ILogger logger, IWebAPIClient webApiClient)
            : base(logger, webApiClient)
        {
            _logger = logger;
            _webAPIClient = webApiClient;
        }
        #endregion

        #region [ Methods ]
        public override IEvent BuildEvent(ICommandCQRS command, WorkflowActionDocumentUnitLinkModel workflowAction)
        {
            EventWorkflowActionDocumentUnitLink @event = new EventWorkflowActionDocumentUnitLink(Guid.NewGuid(), workflowAction.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, workflowAction, null);
            return @event;
        }
        #endregion
    }
}
