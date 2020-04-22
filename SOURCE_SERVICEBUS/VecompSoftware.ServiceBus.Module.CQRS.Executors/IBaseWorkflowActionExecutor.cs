using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Workflow.Actions;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.ServiceBus.Module.CQRS.Executors
{
    public interface IBaseWorkflowActionExecutor<TWFAction> : IWorkflowActionExecutor
        where TWFAction : WorkflowActionModel
    {
        Task CreateActionEventAsync(ICommandCQRS command, TWFAction workflowAction);
    }
}
