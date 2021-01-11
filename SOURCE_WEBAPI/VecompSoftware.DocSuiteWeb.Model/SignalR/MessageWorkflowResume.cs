namespace VecompSoftware.DocSuiteWeb.Model.SignalR
{
    /// <summary>
    /// A message that gets returned to the client when he tries resuming a connection
    /// with signalR including the status and the correlationId
    /// </summary>
    public class MessageWorkflowResume
    {
        /// <summary>
        /// The correlation id for which a <see cref="WorkflowBusSubscriber"/> was found and a <see cref="WorkflowHubSubscriber"/>
        /// attached to id
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The status of the resume attempt
        /// </summary>
        public MessageWorkflowResumeStatus Status { get; set; }
    }
}