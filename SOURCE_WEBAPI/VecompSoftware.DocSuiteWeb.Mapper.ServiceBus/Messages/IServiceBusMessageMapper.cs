using Microsoft.ServiceBus.Messaging;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public interface IServiceBusMessageMapper : IDomainMapper<ServiceBusMessage, BrokeredMessage>
    {

    }
}
