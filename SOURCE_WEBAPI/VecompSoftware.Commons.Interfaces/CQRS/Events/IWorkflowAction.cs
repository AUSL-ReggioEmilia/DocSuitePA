using System;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;

namespace VecompSoftware.Commons.Interfaces.CQRS.Events
{
    public interface IWorkflowAction
    {
        Guid UniqueId { get; set; }

        Guid CorrelationId { get; set; }

        string WorkflowName { get; set; }

        Guid? IdWorkflowActivity { get; set; }

        IContentBase Referenced { get; set; }

        /// <summary>
        /// Specified message's type of dependency
        /// </summary>
        string MessageTypeDependency { get; set; }
    }

}
