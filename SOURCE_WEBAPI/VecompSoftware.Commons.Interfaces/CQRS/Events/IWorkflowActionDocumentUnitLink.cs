using VecompSoftware.Commons.Interfaces.CQRS.Commands;

namespace VecompSoftware.Commons.Interfaces.CQRS.Events
{
    /// <summary>
    /// Workflow Action : Create link between DocumentUnit
    /// </summary>
    public interface IWorkflowActionDocumentUnitLink : IWorkflowAction
    {
        /// <summary>
        /// Link to DocumentUnit
        /// </summary>
        IContentBase DestinationLink { get; set; }
    }
}
