using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;


namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierFolderRole : DSWBaseEntity
    {
        #region [ Constructor ]
        public DossierFolderRole() : this(Guid.NewGuid()) { }

        public DossierFolderRole(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public AuthorizationRoleType AuthorizationRoleType { get; set; }
        public bool IsMaster { get; set; }
        public DossierRoleStatus Status { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual DossierFolder DossierFolder { get; set; }
        public virtual Role Role { get; set; }

        #endregion
    }
}
