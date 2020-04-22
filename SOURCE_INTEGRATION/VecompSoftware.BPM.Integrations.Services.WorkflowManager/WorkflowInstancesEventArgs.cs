using System;
using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Model.Workflows;

namespace VecompSoftware.BPM.Integrations.Services.WorkflowManager
{
    public class WorkflowInstancesEventArgs : EventArgs
    {
        public WorkflowInstancesEventArgs()
        {
            Instances = new List<WorkflowInstanceModel>();
        }

        public ICollection<WorkflowInstanceModel> Instances { get; set; }
    }
}
