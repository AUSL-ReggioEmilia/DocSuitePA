namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows
{
    /// <summary>
    /// Rappresenta lo stato del workflow
    /// </summary>
    public enum WorkflowStatus : short
    {
        /// <summary>
        /// Invalid
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Started
        /// </summary>
        Started = 1,
        /// <summary>
        /// Progress
        /// </summary>
        Progress = 2 * Started,
        /// <summary>
        /// Done
        /// </summary>
        Done = 2 * Progress,
        /// <summary>
        /// Error
        /// </summary>
        Error = 2 * Done
    }
}
