using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface ITopicService : IServiceAsync<ServiceBusMessage, ServiceBusMessage>
    {
        Task<ServiceBusMessage> SendToTopicAsync(ServiceBusMessage message);
        Task<ICollection<ServiceBusMessage>> GetMessagesFromTopicAsync();
        Task<ServiceBusMessage> GetMessageByIdFromTopicAsync(Guid messageId);
        Task<ICollection<ServiceBusMessage>> GetMessagesAsync(string topicName, string subscriptionName);
        Task<ServiceBusMessageState> GetTopicStatusAsync();
        Task<bool> SubscriptionExists(string topicName, string subscriptionName);
        Task<ITopicService> CreateSubscriptionAsync(string topicName, string subscriptionName, string correlationId, string commandName);
        Task<ITopicService> SubscribeTopicAsync(string topicName, string subscriptionName, string correlationId, string commandName, Action<ServiceBusMessage> Callback);
        Task<ITopicService> UnsubscribeTopicAsync(string topicName, string subscriptionName);
        Task InitializeAsync(string topicName, string subscriptionName, string filter = "");
    }
}
