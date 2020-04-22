using System;
using VecompSoftware.DocSuiteWeb.Model.Workflow;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Workflows
{
    public class WorkflowActivityModel
    {
        public Guid IdWorkflowActivity { get; set; }
        public Guid IdWorkflowInstance { get; set; }
        public Guid? IdDocumentUnit { get; set; }
        public WorkflowStatus Status { get; set; }
        public string WorkflowName { get; set; }
        public string ActivityName { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public WorkflowAccount RequestorUsername { get; set; }
        public DateTimeOffset RequestedDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset? EndMotivationDate { get; set; }
        public string RegistrationUser { get; set; }
        public string CategoryName { get; set; }
        public string AuthorizationRole { get; set; }
        public string RequestMotivation { get; set; }
        public WorkflowAccount AuthorizationUser { get; set; }
        public WorkflowReferenceModel WorkflowStartReferenceModel { get; set; }
        public string EndMotivation { get; set; }
        public WorkflowReferenceModel WorkflowEndReferenceModel { get; set; }
        public Guid? IdArchiveChain { get; set; }
    }
}
