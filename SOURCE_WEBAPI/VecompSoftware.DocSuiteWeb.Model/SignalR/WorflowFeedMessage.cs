using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Model.SignalR
{
    /// <summary>
    /// Wrapper aroung a <see cref="ServiceBusMessage"/> that identifies the channel destination 
    /// based on the <see cref="WorkflowFeedMessageType"/>
    /// </summary>
    public class WorflowFeedMessage
    {
        public ServiceBusMessage Message { get; }

        public WorkflowFeedMessageType Type { get; }

        public WorflowFeedMessage(ServiceBusMessage message, WorkflowFeedMessageType type)
        {
            Message = message;
            Type = type;
        }
    }
}