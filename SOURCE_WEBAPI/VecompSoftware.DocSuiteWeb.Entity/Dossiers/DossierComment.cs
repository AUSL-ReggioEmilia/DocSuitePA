using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Dossiers
{
    public class DossierComment : DSWBaseEntity
    {
        #region [ Constructor ]
        public DossierComment() : this(Guid.NewGuid()) { }

        public DossierComment(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string Comment { get; set; }
        public string Author { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Dossier Dossier { get; set; }
        public virtual DossierFolder DossierFolder { get; set; }
        #endregion
    }
}
