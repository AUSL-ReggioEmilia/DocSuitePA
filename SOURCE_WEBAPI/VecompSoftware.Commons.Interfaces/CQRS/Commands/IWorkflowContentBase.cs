using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.Commons.Interfaces.CQRS.Commands
{
    public interface IWorkflowContentBase : IContentBase
    {
        /// <summary>
        /// BuildAutoComplete se impostato a true l'azione di build del workflow effettuerà
        /// in automatico l'azione di WorkflowNotify del workflow
        /// </summary>
        bool WorkflowAutoComplete { get; set; }
        string WorkflowName { get; set; }
        Guid? IdWorkflowActivity { get; set; }
        ICollection<IWorkflowAction> WorkflowActions { get; set; }
    }
}
