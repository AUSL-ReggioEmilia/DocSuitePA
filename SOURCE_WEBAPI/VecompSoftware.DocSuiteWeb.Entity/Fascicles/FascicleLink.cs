using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleLink : DSWBaseEntity
    {
        #region [ Constructor ]

        public FascicleLink() : this(Guid.NewGuid()) { }

        public FascicleLink(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public FascicleLinkType FascicleLinkType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }

        public virtual Fascicle FascicleLinked { get; set; }

        #endregion
    }
}
