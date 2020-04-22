using VecompSoftware.Commons.Interfaces.CQRS.Commands;

namespace VecompSoftware.Commons.Interfaces.CQRS.Events
{
    /// <summary>
    /// Workflow Action : Insert referenced document unit into fascicle
    /// </summary>

    public interface IWorkflowActionFascicle : IWorkflowAction
    {
        /// <summary>
        /// Link to DocumentUnit
        /// </summary>
        IContentBase Fascicle { get; set; }
    }
}
