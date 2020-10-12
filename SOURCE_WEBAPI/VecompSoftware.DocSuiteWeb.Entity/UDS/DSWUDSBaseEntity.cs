using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public abstract class DSWUDSBaseEntity : DSWBaseEntity
    {
        protected DSWUDSBaseEntity(Guid uniqueId) : base(uniqueId)
        {
        }

        #region [ Properties ]
        public Guid IdUDS { get; set; }

        public int Environment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual UDSRepository Repository { get; set; }
        public virtual DocumentUnit SourceUDS { get; set; }
        #endregion

    }
}
