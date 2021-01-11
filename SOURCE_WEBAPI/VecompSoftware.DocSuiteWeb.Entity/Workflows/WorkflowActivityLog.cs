using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{

    public class WorkflowActivityLog : DSWBaseLogEntity<WorkflowActivity, WorkflowStatus>
    {
        #region Constructors

        public WorkflowActivityLog() : this(Guid.NewGuid()) { }

        public WorkflowActivityLog(Guid uniqueId)
            : base(uniqueId)
        {
            LogDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties

        public DateTimeOffset LogDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
