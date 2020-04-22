using System;

namespace VecompSoftware.BPM.Integrations.Services.WorkflowManager
{
    public interface IWorkflowInstanceManager
    {
        event EventHandler<WorkflowInstancesEventArgs> InstancesChanged;
        void Start();
        void Stop();
    }
}
