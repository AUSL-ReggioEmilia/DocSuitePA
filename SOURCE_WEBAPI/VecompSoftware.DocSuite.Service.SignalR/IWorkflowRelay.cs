namespace VecompSoftware.DocSuite.Service.SignalR
{
    public interface IWorkflowRelay
    {
        bool ClientCanResume(string correlationId);
        void RemoveBusSubscriber(WorkflowBusSubscriber workflowCorrelatedBusSubscriber);
        void RemoveHubSubscribers(string correlatedId);
        bool WorkflowRelayResume(string correlationId, WorkflowHubSubscriber clientSubscriber, out WorkflowBusSubscriber correlatedInstance);
        void WorkflowRelayStart(string correlationId, WorkflowHubSubscriber clientSubscriber, out WorkflowBusSubscriber correlatedInstance);
    }
}