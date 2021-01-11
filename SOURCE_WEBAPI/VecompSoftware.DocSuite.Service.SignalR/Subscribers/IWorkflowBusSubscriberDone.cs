using System;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    /// <summary>
    /// A contract for <see cref="WorkflowBusSubscriber"/> which enforces an event to be triggered when 
    /// there operations for the current correlation Id are finished and instance can start clean up processes.
    /// The event is trigger when the following queues are called : 
    /// <para>- <see cref="WorkflowBusSubscriber.QueueResponseStatusDone(DocSuiteWeb.Model.ServiceBus.ServiceBusMessage)"/></para>
    /// <para>- <see cref="WorkflowBusSubscriber.QueueResponseStatusError(DocSuiteWeb.Model.ServiceBus.ServiceBusMessage)(DocSuiteWeb.Model.ServiceBus.ServiceBusMessage)"/></para>
    /// <para>Note: The <see cref="WorkflowBusSubscriber"/> is a subscriber for the service bus. In the case that the service bus breaks during the processes,
    /// the queues above will never be called and the client will be stuck in the loop because he has the correlation Id in storage, and 
    /// the <see cref="WorkflowBusSubscriber"/> still exists in the Api. On Resume he will listen and never stop. The <see cref="WorkflowBusSubscriber"/> constructor has a timer which triggers
    /// this event after a while</para>
    /// </summary>
    public interface IWorkflowBusSubscriberDone
    {
        /// <summary>
        /// An event which triggers when the service bus subscription for the correlation id 
        /// has pushed all messages or when the timer allocated for the <see cref="WorkflowBusSubscriber"/> expired
        /// </summary>
        event Action<WorkflowBusSubscriberDoneArgs> InstanceDone;
    }
}