using System;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    public class WorkflowBusSubscriberDoneArgs : EventArgs
    {
        public WorkflowBusSubscriberDoneArgs(WorkflowBusSubscriber instance)
        {
            Instance = instance;
        }

        public WorkflowBusSubscriber Instance { get; }
    }
}