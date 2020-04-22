using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Helpers
{
    public class ServiceBusMessageStateHelper
    {
        public static ServiceBusMessageState ConvertState(EntityStatus state)
        {
            switch (state)
            {
                case EntityStatus.Active:
                    return ServiceBusMessageState.Active;

                case EntityStatus.Disabled:
                    return ServiceBusMessageState.Disabled;

                case EntityStatus.ReceiveDisabled:
                    return ServiceBusMessageState.ReceiveDisabled;

                case EntityStatus.Restoring:
                    return ServiceBusMessageState.Restoring;

                case EntityStatus.SendDisabled:
                    return ServiceBusMessageState.SendDisabled;

                default:
                    return ServiceBusMessageState.Disabled;
            }
        }

        public static EntityStatus ConvertState(ServiceBusMessageState state)
        {
            switch (state)
            {
                case ServiceBusMessageState.Active:
                    return EntityStatus.Active;

                case ServiceBusMessageState.Disabled:
                    return EntityStatus.Disabled;

                case ServiceBusMessageState.ReceiveDisabled:
                    return EntityStatus.ReceiveDisabled;

                case ServiceBusMessageState.Restoring:
                    return EntityStatus.Restoring;

                case ServiceBusMessageState.SendDisabled:
                    return EntityStatus.SendDisabled;

                default:
                    return EntityStatus.Disabled;
            }
        }
    }
}
