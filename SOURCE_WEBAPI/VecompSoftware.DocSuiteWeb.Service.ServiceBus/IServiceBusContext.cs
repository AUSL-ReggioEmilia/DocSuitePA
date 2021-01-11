using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Service.ServiceBus
{
    public interface IServiceBusContext : IDisposable
    {
        Task<BrokeredMessage> PublishAsync(BrokeredMessage message);

        Task<ICollection<BrokeredMessage>> GetMessagesAsync();

        Task<EntityStatus> GetStatusAsync();
    }
}
