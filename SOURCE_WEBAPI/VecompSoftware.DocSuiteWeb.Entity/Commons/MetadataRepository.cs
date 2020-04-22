using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class MetadataRepository : DSWBaseEntity
    {
        #region [ Constructor ]
        public MetadataRepository() : this(Guid.NewGuid()) { }
        public MetadataRepository(Guid uniqueId)
            : base(uniqueId)
        {
            Fascicles = new HashSet<Fascicle>();
            Categories = new HashSet<Category>();
            Dossiers = new HashSet<Dossier>();
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public MetadataRepositoryStatus Status { get; set; }
        public string JsonMetadata { get; set; }
        public int Version { get; set; }
        public DateTimeOffset DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Fascicle> Fascicles { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        #endregion
    }
}
