using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{

    public class WorkflowRoleMapping : DSWBaseEntity
    {
        #region [ Constructor ]
        public WorkflowRoleMapping() : this(Guid.NewGuid()) { }

        public WorkflowRoleMapping(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]

        public string IdInternalActivity { get; set; }

        public string AccountName { get; set; }

        public string MappingTag { get; set; }

        public WorkflowAuthorizationType AuthorizationType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowRepository WorkflowRepository { get; set; }

        public virtual Role Role { get; set; }
        #endregion
    }
}
