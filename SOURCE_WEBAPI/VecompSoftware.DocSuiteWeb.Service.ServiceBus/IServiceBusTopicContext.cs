using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IServiceBusTopicContext : IServiceBusContext
    {
        string TopicName { get; }
        string SubscriptionName { get; }
        Task<IServiceBusContext> BeginContextAsync(string topicName, string subscriptionName = "", string filter = "");
        Task<IServiceBusContext> CreateSubscriptionAsync(string subscriptionName, string correlationId, string defaultFilter, Action<BrokeredMessage> handler);
        Task<IServiceBusContext> RemoveSubscriptionAsync(string subscriptionName);
    }
}
