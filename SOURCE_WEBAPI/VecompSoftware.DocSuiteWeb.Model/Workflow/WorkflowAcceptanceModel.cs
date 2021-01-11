using System;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class WorkflowAcceptanceModel
    {
        #region [ Properties ]

        public string Owner { get; set; }

        public string Executor { get; set; }

        public WorkflowRole ProposedRole { get; set; }

        public AcceptanceStatus Status { get; set; }

        public DateTimeOffset? AcceptanceDate { get; set; }

        public string AcceptanceReason { get; set; }
        #endregion
    }
}
