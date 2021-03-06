﻿using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolBuildModel : IWorkflowContentBase
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ProtocolBuildModel()
        {
            WorkflowActions = new List<IWorkflowAction>();
        }
        #endregion

        #region [ Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public Guid UniqueId { get; set; }
        public string WorkflowName { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public string RegistrationUser { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public ProtocolModel Protocol { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        #endregion

    }
}
