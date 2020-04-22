using System;
using System.Threading.Tasks;

namespace VecompSoftware.ServiceBus.Receiver.Base
{
    public interface IListenerMessage
    {
        Type ICommandFilterType { get; }
        string CommandName { get; }
        Task StartListeningAsync(bool? autoComplete = null, int? maxConcurrentCalls = null);
        Task CloseListeningAsync();
    }
}
