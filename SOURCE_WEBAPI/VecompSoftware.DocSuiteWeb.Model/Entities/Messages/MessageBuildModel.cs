using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Messages
{
    public class MessageBuildModel : IWorkflowContentBase
    {
        #region [ Constructor ]
        public MessageBuildModel()
        {
            UniqueId = Guid.NewGuid();
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region [ Properties ]
        public string WorkflowName { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public MessageModel Message { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion
    }
}
