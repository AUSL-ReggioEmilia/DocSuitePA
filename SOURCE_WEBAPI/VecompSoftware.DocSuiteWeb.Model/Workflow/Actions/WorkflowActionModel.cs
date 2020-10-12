using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Actions
{
    public abstract class WorkflowActionModel : IWorkflowAction
    {
        #region [ Constructor ]
        public WorkflowActionModel()
        {
            UniqueId = Guid.NewGuid();
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }
        public IContentBase Referenced { get; set; }

        /// <summary>
        /// Specified message's type of dependency
        /// </summary>
        public string MessageTypeDependency { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
