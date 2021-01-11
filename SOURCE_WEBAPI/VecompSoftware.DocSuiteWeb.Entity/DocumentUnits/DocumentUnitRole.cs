using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitRole : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public DocumentUnitRole() : this(Guid.NewGuid()) { }
        public DocumentUnitRole(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public string RoleLabel { get; set; }

        public Guid UniqueIdRole { get; set; }

        public string AssignUser { get; set; }

        public AuthorizationRoleType AuthorizationRoleType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
