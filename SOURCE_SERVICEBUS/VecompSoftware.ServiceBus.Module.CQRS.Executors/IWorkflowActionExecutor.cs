using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public interface IWorkflowActionExecutor
    {
        Task CreateActionEventAsync(ICommandCQRS command, IWorkflowAction workflowAction);
    }
}
