using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierRole : DSWBaseEntity
    {
        #region [ Constructor ] 
        public DossierRole() : this(Guid.NewGuid()) { }

        public DossierRole(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public AuthorizationRoleType AuthorizationRoleType { get; set; }
        public DossierRoleStatus Status { get; set; }
        public bool IsMaster { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Dossier Dossier { get; set; }
        public virtual Role Role { get; set; }
        #endregion
    }
}
