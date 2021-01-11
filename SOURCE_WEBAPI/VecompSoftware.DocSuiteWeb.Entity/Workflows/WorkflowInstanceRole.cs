using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{

    public class WorkflowInstanceRole : DSWBaseEntity
    {
        #region [ Constructor ]

        public WorkflowInstanceRole() : this(Guid.NewGuid()) { }

        public WorkflowInstanceRole(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]
        public AuthorizationRoleType AuthorizationType { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual WorkflowInstance WorkflowInstance { get; set; }

        public virtual Role Role { get; set; }

        #endregion
    }
}
