using System.Collections.Concurrent;

namespace VecompSoftware.DocSuite.Service.SignalR
{
    /// <summary>
    /// THe <see cref="WorkflowRelay"/> acts as a sepparation layer between the service bus and the client (browser).
    /// <para>The relay creates for each correlation id a <see cref="WorkflowBusSubscriber"/> which listens to service bus and
    /// stores the received messages in a queue and a list</para>
    /// <para>The <see cref="WorkflowBusSubscriber"/> can have a <see cref="WorkflowHubSubscriber"/> which listens to changes
    /// of the queue. </para>
    /// <para>- When messages are added in the queue and if the hubSubscriber exists messages are dequeued and passed</para>
    /// <para>- When messages are added in the queue and if the hubSubscriber is not null, the queue remains unchanged</para>
    /// <para>- When a <see cref="WorkflowHubSubscriber"/> is subscribing to <see cref="WorkflowBusSubscriber"/> messages from the list are restored to the 
    /// queue and are sent to the <see cref="WorkflowHubSubscriber"/></para>
    /// </summary>
    public class WorkflowRelay : IWorkflowRelay
    {
        private static readonly ConcurrentDictionary<string, WorkflowBusSubscriber> _correlatedInstances = new ConcurrentDictionary<string, WorkflowBusSubscriber>();

        public bool ClientCanResume(string correlationId)
        {
            return _correlatedInstances.ContainsKey(correlationId);
        }

        /// <summary>
        /// A <see cref="WorkflowHubSubscriber"/> which contains the actions that send messages to the client is passed.
        /// <para>A <see cref="WorkflowBusSubscriber"/> is created and <see cref="WorkflowHubSubscriber"/> subscribes to it</para>
        /// </summary>
        /// <param name="correlationId">The correlation if for which the <see cref="WorkflowBusSubscriber"/> is created</param>
        /// <param name="clientSubscriber">The signalR subscriber</param>
        /// <param name="correlatedInstance">The service bus subscriber</param>
        public void WorkflowRelayStart(string correlationId, WorkflowHubSubscriber clientSubscriber, out WorkflowBusSubscriber correlatedInstance)
        {
            WorkflowBusSubscriber instance = new WorkflowBusSubscriber(correlationId);

            _correlatedInstances.TryAdd(correlationId, instance);

            instance.Subscribe(clientSubscriber);

            correlatedInstance = instance;
        }

        // <summary>
        /// A <see cref="WorkflowHubSubscriber"/> which contains the actions that send messages to the client is passed.
        /// <para>A <see cref="WorkflowBusSubscriber"/> is searched in the list of saved subscribers 
        /// </summary>
        /// <param name="correlationId">The correlation if for which the <see cref="WorkflowBusSubscriber"/> is created</param>
        /// <param name="clientSubscriber">The signalR subscriber</param>
        /// <param name="correlatedInstance">The service bus subscriber</param>
        /// <returns>Returns true if a busSubscriber was found. False otherwhise</returns>
        public bool WorkflowRelayResume(string correlationId, WorkflowHubSubscriber clientSubscriber, out WorkflowBusSubscriber correlatedInstance)
        {
            _correlatedInstances.TryGetValue(correlationId, out WorkflowBusSubscriber _correlatedInstance);

            if (!(_correlatedInstance is null))
            {
                _correlatedInstance.SubscribeResume(clientSubscriber);
                correlatedInstance = _correlatedInstance;
                return true;
            }

            correlatedInstance = null;
            return false;
        }

        /// <summary>
        /// Removes a <see cref="WorkflowBusSubscriber"/> and it's attached <see cref="WorkflowHubSubscriber"/>
        /// </summary>
        /// <param name="workflowCorrelatedBusSubscriber"></param>
        public void RemoveBusSubscriber(WorkflowBusSubscriber workflowCorrelatedBusSubscriber)
        {
            _correlatedInstances.TryRemove(workflowCorrelatedBusSubscriber.CorrelationId, out WorkflowBusSubscriber instance);
            instance?.Dispose();
        }

        /// <summary>
        /// Removes the <see cref="WorkflowHubSubscriber"/> from <see cref="WorkflowBusSubscriber"/>
        /// </summary>
        /// <param name="correlatedId"></param>
        public void RemoveHubSubscribers(string correlatedId)
        {
            //we need to keep the bus subscriber, as we will receive messages from the bus
            //we need to remove the hub subscriber as we are not passing anything to the client while he is disconected
            _correlatedInstances.TryGetValue(correlatedId, out WorkflowBusSubscriber instance);

            //a lock is already added prior to calling this method, we can safely have multiple executable lines
            instance?.RemoveHubSubscriber();
        }
    }
}