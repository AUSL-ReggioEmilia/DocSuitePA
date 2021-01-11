namespace VecompSoftware.DocSuiteWeb.Entity.Tasks
{
    public enum TaskStatus
    {
        Queued = 0,
        OnError = 1,
        DoneWithErrors = 2,
        DoneWithWarnings = 3,
        Done = 100
    }
}
