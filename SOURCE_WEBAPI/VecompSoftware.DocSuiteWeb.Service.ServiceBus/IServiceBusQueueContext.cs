using Microsoft.ServiceBus.Messaging;
using System;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IServiceBusQueueContext : IServiceBusContext
    {
        string QueueName { get; }
        IServiceBusContext BeginContext(string queueName, Action<BrokeredMessage> handler);
    }
}
