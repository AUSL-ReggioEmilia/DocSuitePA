using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public interface IBaseWorkflowActionExecutor<TWFAction> : IWorkflowActionExecutor
        where TWFAction : WorkflowActionModel
    {
        IEvent BuildEvent(ICommandCQRS command, TWFAction workflowAction);
        Task CreateActionEventAsync(ICommandCQRS command, TWFAction workflowAction);        
    }
}
