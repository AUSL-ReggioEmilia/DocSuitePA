using System;
using VecompSoftware.Core.Command.CQRS.Events.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.ServiceBus.WebAPI;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors.Executors.Workflows
{
    public class WorkflowActionShareDocumentUnitModelExecutor : BaseWorkflowActionExecutor<WorkflowActionShareDocumentUnitModel>, IWorkflowActionShareDocumentUnitModelExecutor
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IWebAPIClient _webAPIClient;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowActionShareDocumentUnitModelExecutor(ILogger logger, IWebAPIClient webApiClient)
            : base(logger, webApiClient)
        {
            _logger = logger;
            _webAPIClient = webApiClient;
        }
        #endregion

        #region [ Methods ]
        public override IEvent BuildEvent(ICommandCQRS command, WorkflowActionShareDocumentUnitModel workflowAction)
        {
            DocumentUnitModel documentUnitModel = workflowAction.GetReferenced();
            DocumentUnit documentUnit = _webAPIClient.GetDocumentUnitAsync(new DocumentUnit(documentUnitModel.UniqueId)).Result;
            documentUnit.WorkflowName = workflowAction.WorkflowName;
            documentUnit.IdWorkflowActivity = workflowAction.IdWorkflowActivity;

            EventShareDocumentUnit @event = new EventShareDocumentUnit(Guid.NewGuid(), workflowAction.CorrelationId, command.TenantName, command.TenantId, command.TenantAOOId, command.Identity, documentUnit, null);
            return @event;
        }
        #endregion
    }
}
