using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Helpers
{
    public class ServiceBusResponseStateHelper
    {
        public static ServiceBusResponseState ConvertState(MessageState state)
        {
            switch (state)
            {
                case MessageState.Active:
                    return ServiceBusResponseState.Active;

                case MessageState.Deferred:
                    return ServiceBusResponseState.Deferred;

                case MessageState.Scheduled:
                    return ServiceBusResponseState.Scheduled;

                default:
                    return ServiceBusResponseState.Deferred;
            }
        }

        public static MessageState ConvertState(ServiceBusResponseState state)
        {
            switch (state)
            {
                case ServiceBusResponseState.Active:
                    return MessageState.Active;

                case ServiceBusResponseState.Deferred:
                    return MessageState.Deferred;

                case ServiceBusResponseState.Scheduled:
                    return MessageState.Scheduled;

                default:
                    return MessageState.Deferred;
            }
        }
    }
}
