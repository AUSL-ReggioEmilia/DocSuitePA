using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IQueueService : IServiceAsync<ServiceBusMessage, ServiceBusMessage>
    {
        Task<ServiceBusMessage> SendToQueueAsync(ServiceBusMessage message);
        Task<ICollection<ServiceBusMessage>> GetMessagesFromQueueAsync();
        Task<ServiceBusMessageState> GetQueueStatusAsync();
        IQueueService SubscribeQueue(string queueName);
        IQueueService SubscribeQueue(string queueName, Action<ServiceBusMessage> Callback);
    }
}
