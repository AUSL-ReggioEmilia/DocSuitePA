using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.Services.Command.CQRS;

namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus.Messages
{
    public interface ICQRSMessageMapper : IDomainMapper<IMessage, ServiceBusMessage>
    {

    }
}
