using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity
{

    public abstract class DSWBaseLogEntity<TLogEntity, TType> : DSWBaseEntity, IDSWLogEntity<TLogEntity, TType>
        where TLogEntity : DSWBaseEntity
    {
        #region [ Constructor ]
        protected DSWBaseLogEntity(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        #region [ Properties ]

        public string SystemComputer { get; set; }

        public TType LogType { get; set; }

        public string LogDescription { get; set; }

        public SeverityLog? Severity { get; set; }

        public string Hash { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual TLogEntity Entity { get; set; }

        #endregion

    }
}
