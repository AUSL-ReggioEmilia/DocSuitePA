using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public abstract class DSWBaseEntityFascicolable<T> : DSWBaseEntity, IDSWEntityFascicolable<T>
        where T : DSWBaseEntity
    {
        #region [ Constructor ]
        protected DSWBaseEntityFascicolable(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        #region [ Properties ]

        public ReferenceType ReferenceType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }

        public virtual T DocumentUnit { get; set; }

        public virtual FascicleFolder FascicleFolder { get; set; }
        #endregion
    }
}
