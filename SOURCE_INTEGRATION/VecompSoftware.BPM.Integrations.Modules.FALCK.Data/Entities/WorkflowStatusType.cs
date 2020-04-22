namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities
{
    public enum WorkflowStatusType
    {
        Default = 0,
        WorkflowBootable = 1,
        WorkflowStarted = 2,
        WorkflowCompleted = 3,
        ValidationError = 4,
        FileNotFound = 5,
        UnexpectedError = 6
    }
}
