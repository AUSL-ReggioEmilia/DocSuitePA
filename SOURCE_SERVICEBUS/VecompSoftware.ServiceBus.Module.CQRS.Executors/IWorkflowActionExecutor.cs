using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public interface IWorkflowActionExecutor
    {
        IEvent BuildEvent(ICommandCQRS command, IWorkflowAction workflowAction);
        Task CreateActionEventAsync(ICommandCQRS command, IWorkflowAction workflowAction);
    }
}
