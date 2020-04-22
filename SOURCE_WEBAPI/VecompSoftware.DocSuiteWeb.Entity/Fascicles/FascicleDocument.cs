using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleDocument : DSWBaseEntity
    {
        #region [ Constructor ]

        public FascicleDocument() : this(Guid.NewGuid()) { }

        public FascicleDocument(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]

        public ChainType ChainType { get; set; }

        public Guid IdArchiveChain { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }

        public virtual FascicleFolder FascicleFolder { get; set; }

        #endregion
    }
}
