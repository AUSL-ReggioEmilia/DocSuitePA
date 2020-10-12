using System;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.DTO.Workflows
{
    public class WorkflowActivityResult
    {
        public WorkflowActivityResult() : base()
        { 
        }

        public Guid WorkflowActivityId { get; set; }

        public string WorkflowActivityName { get; set; }

        public string WorkflowRepositoryName { get; set; }
        
        public string WorkflowActivityRequestorUser { get; set; }

        public WorkflowActivityType WorkflowActivityType { get; set; }

        public DateTimeOffset WorkflowActivityPublicationDate { get; set; }

        public DateTimeOffset? WorkflowActivityLastChangedDate { get; set; }

        public WorkflowStatus WorkflowActivityStatus { get; set; }

        public string WorkflowActivityProperty { get; set; }

        public string WorkflowSubject { get; set; }

        public Guid WorkflowInstanceId { get; set; }

    }
}
