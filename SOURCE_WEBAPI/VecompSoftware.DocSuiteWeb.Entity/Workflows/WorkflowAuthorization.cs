using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public class WorkflowAuthorization : DSWBaseEntity
    {
        #region [ Constructor ]

        public WorkflowAuthorization() : this(Guid.NewGuid()) { }

        public WorkflowAuthorization(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

        public string Account { get; set; }

        public bool IsHandler { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowActivity WorkflowActivity { get; set; }
        #endregion
    }
}
