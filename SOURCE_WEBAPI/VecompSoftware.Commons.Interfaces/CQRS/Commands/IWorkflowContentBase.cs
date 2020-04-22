using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.Commons.Interfaces.CQRS.Commands
{
    public interface IWorkflowContentBase : IContentBase
    {
        string WorkflowName { get; set; }
        Guid? IdWorkflowActivity { get; set; }
        ICollection<IWorkflowAction> WorkflowActions { get; set; }
    }
}
