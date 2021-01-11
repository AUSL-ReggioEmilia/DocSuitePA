using System;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleUDS : DSWBaseEntity
    {
        #region [ Constructor ]

        public FascicleUDS() : this(Guid.NewGuid()) { }

        public FascicleUDS(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]

        public Guid IdUDS { get; set; }

        public ReferenceType ReferenceType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }

        public virtual UDSRepository UDSRepository { get; set; }

        public virtual FascicleFolder FascicleFolder { get; set; }
        #endregion
    }
}
