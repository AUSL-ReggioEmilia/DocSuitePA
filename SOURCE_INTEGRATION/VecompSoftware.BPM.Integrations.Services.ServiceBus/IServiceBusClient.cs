using System;
using System.Threading.Tasks;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.BPM.Integrations.Services.ServiceBus
{
    public interface IServiceBusClient
    {
        Guid StartListening<TEvent>(string moduleName, string topicName, string subscriptionName, Func<TEvent, Task> callbackAsync)
            where TEvent : IEvent;

        Guid StartListening<TCommand>(string moduleName, string queueName, Func<TCommand, Task> callbackAsync)
            where TCommand : ICommand;

        Task CloseListeningAsync(Guid clientId);

        Task<string> CreateSubscriptionAsync(string topicName, string filter);

        Task SendCommandAsync<TCommand>(TCommand command, string queueName, DateTime? scheduleEnqueueTimeUtc)
            where TCommand : ICommand;

        Task SendEventAsync<TEvent>(TEvent @event, string topicName, DateTime? scheduleEnqueueTimeUtc)
            where TEvent : IEvent;

        Task DeleteSubscriptionAsync(string topicName, string filter);

    }
}
