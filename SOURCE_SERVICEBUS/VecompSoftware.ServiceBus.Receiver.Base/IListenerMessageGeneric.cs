using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.ServiceBus.Receiver.Base
{
    public interface IListenerMessageGeneric<TCommand> : IListenerMessage
         where TCommand : ICommand
    {
    }
}
