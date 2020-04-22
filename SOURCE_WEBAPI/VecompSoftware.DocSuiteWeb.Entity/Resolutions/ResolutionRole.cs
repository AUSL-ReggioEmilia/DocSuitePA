using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    /// <summary>
    /// TODO: La Resolution è da estendere con tutte le proprietà e navigation properties mancanti
    /// </summary>
    public class ResolutionRole : DSWBaseEntity
    {
        #region [ Constructor ]
        public ResolutionRole() : this(Guid.NewGuid()) { }
        public ResolutionRole(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

        public int IdResolutionRoleType { get; set; }


        #endregion

        #region [ Navigation Properties ]

        //public virtual Resolution Resolution { get; set; }

        public virtual Role Role { get; set; }

        #endregion


    }
}
