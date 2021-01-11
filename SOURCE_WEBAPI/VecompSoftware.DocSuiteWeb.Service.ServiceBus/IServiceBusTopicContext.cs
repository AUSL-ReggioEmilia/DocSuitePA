using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IServiceBusTopicContext : IServiceBusContext
    {
        string TopicName { get; }
        string SubscriptionName { get; }
        Task<IServiceBusContext> BeginContextAsync(string topicName, string subscriptionName = "", string filter = "");
        Task<ICollection<BrokeredMessage>> GetMessagesAsync(string subscriptionName);
        bool SubscriptionExists(string subscriptionName);
        Task<IServiceBusContext> CreateSubscriptionAsync(string subscriptionName, string correlationId, string defaultFilter, Action<BrokeredMessage> handler);
        Task<IServiceBusContext> CreateSubscriptionAsync(string subscriptionName, string correlationId, string defaultFilter, bool createSubscription);
        Task<IServiceBusContext> RemoveSubscriptionAsync(string subscriptionName);
    }
}
