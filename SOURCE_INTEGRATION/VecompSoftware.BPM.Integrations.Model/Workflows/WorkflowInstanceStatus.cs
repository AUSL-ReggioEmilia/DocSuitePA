namespace VecompSoftware.BPM.Integrations.Model.Workflows
{
    public enum WorkflowInstanceStatus
    {
        NotSpecified = 0,
        NotStarted = 1,
        Started = 2,
        Suspended = 3,
        Canceling = 4,
        Canceled = 5,
        Terminated = 6,
        Completed = 7
    }
}
