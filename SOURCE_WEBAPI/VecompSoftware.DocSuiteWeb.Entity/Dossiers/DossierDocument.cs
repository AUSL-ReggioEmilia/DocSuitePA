using System;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierDocument : DSWBaseEntity
    {
        #region [ Constructor ]
        public DossierDocument() : this(Guid.NewGuid()) { }

        public DossierDocument(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public ChainType ChainType { get; set; }
        public Guid IdArchiveChain { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Dossier Dossier { get; set; }
        #endregion
    }
}
