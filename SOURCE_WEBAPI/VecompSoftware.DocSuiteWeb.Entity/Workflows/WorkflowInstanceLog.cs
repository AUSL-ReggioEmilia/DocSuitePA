using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public class WorkflowInstanceLog : DSWBaseLogEntity<WorkflowInstance, WorkflowInstanceLogType>
    {
        #region [ Constructor ]

        public WorkflowInstanceLog() : this(Guid.NewGuid()) { }

        public WorkflowInstanceLog(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]


        #endregion

        #region [ Navigation Properties ]


        #endregion
    }
}
