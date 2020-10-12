using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationBuildModel : IWorkflowContentBase
    {
        #region [ Constructor ]

        public CollaborationBuildModel()
        {
            UniqueId = Guid.NewGuid();
            WorkflowActions = new List<IWorkflowAction>();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public bool WorkflowAutoComplete { get; set; }
        public string WorkflowName { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string RegistrationUser { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public CollaborationModel Collaboration { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
