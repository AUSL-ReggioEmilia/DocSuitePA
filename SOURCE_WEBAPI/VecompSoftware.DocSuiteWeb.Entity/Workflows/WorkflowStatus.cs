
namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public enum WorkflowStatus : short
    {
        Todo = 1,
        Progress = 2,
        Suspended = 2 * Progress,
        Done = 2 * Suspended,
        Error = 2 * Done,
        LogicalDelete = 2 * Error
    }
}
