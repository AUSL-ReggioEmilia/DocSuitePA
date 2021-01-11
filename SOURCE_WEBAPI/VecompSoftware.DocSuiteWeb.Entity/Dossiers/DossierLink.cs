using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierLink : DSWBaseEntity
    {
        #region [ Constructor ]
        public DossierLink() : this(Guid.NewGuid()) { }
        public DossierLink(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public DossierLinkType DossierLinkType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Dossier Dossier { get; set; }

        public virtual Dossier DossierLinked { get; set; }

        #endregion
    }
}
