using System;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.SignalR;

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
    public class WorkflowBusSubscriber : IWorkflowBusSubscriberDone, IDisposable
    {
        // All messages are stored in one repository to maintain the order
        public IMessageQueueRepository<WorflowFeedMessage> Messages { get; }

        private WorkflowHubSubscriber _subscriber;

        public event Action<WorkflowBusSubscriberDoneArgs> InstanceDone;

        public string CorrelationId { get; }

        public WorkflowBusSubscriber(string correlationId)
        {
            Messages = new InMemoryMessageQueueRepository<WorflowFeedMessage>();
            CorrelationId = correlationId;

            //InstanceDone will clear hub subscriber and remove itself from the static list of subscribers
            //if the answer is never done, we should clear this 
            System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMinutes(20).TotalSeconds);
            timer.Elapsed += (_, __) =>
            {
                InstanceDone?.Invoke(new WorkflowBusSubscriberDoneArgs(this));
            };
        }

        /// <summary>
        /// Restores the queue will all received messages (some of them were sent on the first subscription).
        /// Subscribes the hub channel to the queue.
        /// </summary>
        /// <param name="subscriber"></param>
        public void SubscribeResume(WorkflowHubSubscriber subscriber)
        {
            Volatile.Write(ref _subscriber, subscriber);
            //RestoreOlder uses an internal lock. The collection will not suffer changes while restoration is in progress
            Messages.RestoreOlder();
            TrySendMessagesInternal();
        }

        public void Subscribe(WorkflowHubSubscriber subscriber)
        {
            Volatile.Write(ref _subscriber, subscriber);
            TrySendMessagesInternal();
        }

        public void RemoveHubSubscriber()
        {
            //otherwise other threads will not have it as null. They will have local copies
            Volatile.Write(ref _subscriber, null);
        }

        /// <summary>
        /// If there is a <see cref="WorkflowHubSubscriber"/> it will deque messages and send them. 
        /// Otherwise the queue remain unchainges
        /// </summary>
        private void TrySendMessagesInternal()
        {
            lock (WorkflowHubLock.GetLock(CorrelationId))
            {
                if (!(_subscriber is null) && Messages.Count > 0)
                {
                    WorflowFeedMessage message = Messages.Dequeue();

                    switch (message.Type)
                    {
                        case WorkflowFeedMessageType.StatusDone:
                            _subscriber.SendResponseStatusDone(message.Message);
                            break;
                        case WorkflowFeedMessageType.StatusError:
                            _subscriber.SendResponseStatusError(message.Message);
                            break;
                        case WorkflowFeedMessageType.NotificationInfo:
                            _subscriber.SendResponseNotificationInfo(message.Message);
                            break;
                        case WorkflowFeedMessageType.NotificationInfoModel:
                            _subscriber.SendResponseNotificationInfoModel(message.Message);
                            break;
                        case WorkflowFeedMessageType.NotificationWarning:
                            _subscriber.SendResponseNotificationWarning(message.Message);
                            break;
                        case WorkflowFeedMessageType.NotificationError:
                            _subscriber.SendResponseNotificationError(message.Message);
                            break;
                        default:
                            throw new Exception();
                    }

                    if (Messages.Count > 0)
                    {
                        TrySendMessagesInternal();
                    }
                }
            }
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowStatusDone of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseStatusDone(ServiceBusMessage result)
        {
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.StatusDone));
            TrySendMessagesInternal();
            InstanceDone?.Invoke(new WorkflowBusSubscriberDoneArgs(this));
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowStatusError of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseStatusError(ServiceBusMessage result)
        {
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.StatusError));
            TrySendMessagesInternal();
            InstanceDone?.Invoke(new WorkflowBusSubscriberDoneArgs(this));
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowNotificationInfo of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseNotificationInfo(ServiceBusMessage result)
        {
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.NotificationInfo));
            TrySendMessagesInternal();
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowNotificationInfModel of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseNotificationInfoModel(ServiceBusMessage result)
        {
            //TODO : recheck this. Resume flow is not implemented for the simulation
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.NotificationInfoModel));
            TrySendMessagesInternal();
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowNotificationWarning of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseNotificationWarning(ServiceBusMessage result)
        {
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.NotificationWarning));
            TrySendMessagesInternal();
        }

        /// <summary>
        /// Callback method listening to service bus for event WorkflowNotificationError of <see cref="CorrelationId"/>
        /// </summary>
        /// <param name="result"></param>
        public void QueueResponseNotificationError(ServiceBusMessage result)
        {
            Messages.Enqueue(new WorflowFeedMessage(result, WorkflowFeedMessageType.NotificationError));
            TrySendMessagesInternal();
        }

        public void Dispose()
        {
            _subscriber = null;
            InstanceDone = null; //https://stackoverflow.com/questions/153573/how-can-i-clear-event-subscriptions-in-c
            Messages.Clear();
            //if was called, finalizer no longer needed
            GC.SuppressFinalize(this);
        }

        ~WorkflowBusSubscriber()
        {
            Dispose();
        }
    }
}