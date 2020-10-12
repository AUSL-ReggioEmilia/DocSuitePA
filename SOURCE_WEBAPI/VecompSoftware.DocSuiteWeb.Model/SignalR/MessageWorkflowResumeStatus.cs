namespace VecompSoftware.DocSuiteWeb.Model.SignalR
{
    /// <summary>
    /// A status that gets returned to the client when he tries resuming a connection
    /// with signalR. 
    /// </summary>
    public enum MessageWorkflowResumeStatus
    {
        /// <summary>
        /// Notifies the client that resuming was succesfull
        /// </summary>
        Resumed = 0,

        /// <summary>
        /// Notifies the client that resuming was not possible. This
        /// happens when the correlationId which is saved in the client's storage
        /// does not have an instance saved in <see cref="WorkflowRelay"/> because everything was already processed
        /// </summary>
        DidNotResume = 1
    }
}