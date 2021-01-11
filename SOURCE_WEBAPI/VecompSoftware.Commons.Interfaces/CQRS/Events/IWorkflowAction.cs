using System;
using System.Collections.Generic;
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
        /// Specified message type of dependency
        /// </summary>
        string MessageTypeDependency { get; set; }

        ICollection<IWorkflowAction> WorkflowActions { get; set; }
    }

}
