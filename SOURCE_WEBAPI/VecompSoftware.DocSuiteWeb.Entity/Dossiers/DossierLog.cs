using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierLog : DSWBaseLogEntity<Dossier, DossierLogType>
    {
        #region [ Constructor ]
        public DossierLog() : this(Guid.NewGuid()) { }

        public DossierLog(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]


        #endregion

        #region [ Navigation Properties ]

        public virtual DossierFolder DossierFolder { get; set; }
        #endregion
    }
}
