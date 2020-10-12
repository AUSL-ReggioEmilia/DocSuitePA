namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IWorkflowActionDTO : IAPIArgument
    {
        string WorkflowName { get; }
        WorkflowActionType WorkflowActionType { get; }
    }
}
