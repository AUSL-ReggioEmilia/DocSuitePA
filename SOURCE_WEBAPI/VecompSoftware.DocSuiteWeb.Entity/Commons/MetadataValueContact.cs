using System;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class MetadataValueContact : DSWBaseEntity
    {
        #region [ Constructor ]
        public MetadataValueContact() : this(Guid.NewGuid()) { }
        public MetadataValueContact(Guid uniqueId)
            : base(uniqueId)
        {
            
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public string ContactManual { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Contact Contact { get; set; }
        public virtual Fascicle Fascicle { get; set; }
        public virtual Dossier Dossier { get; set; }
        #endregion
    }
}
