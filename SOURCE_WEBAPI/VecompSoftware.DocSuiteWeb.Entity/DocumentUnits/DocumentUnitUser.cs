using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitUser : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public DocumentUnitUser() : this(Guid.NewGuid()) { }
        public DocumentUnitUser(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public string Account { get; set; }
        public AuthorizationRoleType AuthorizationType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual DocumentUnit DocumentUnit { get; set; }

        #endregion
    }
}
