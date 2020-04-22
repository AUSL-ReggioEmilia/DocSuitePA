using System;

namespace VecompSoftware.BPM.Integrations.Model.Workflows
{
    public class WorkflowInstanceModel
    {
        public Guid InstanceId { get; set; }
        public WorkflowInstanceStatus Status { get; set; }
    }
}
