using System;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class MetadataValue : DSWBaseEntity
    {
        #region [ Constructor ]
        public MetadataValue() : this(Guid.NewGuid()) { }
        public MetadataValue(Guid uniqueId)
            : base(uniqueId)
        {
            
        }
        #endregion

        #region [ Properties ]
        public MetadataPropertyType PropertyType { get; set; }
        public string Name { get; set; }
        public long? ValueInt { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueDouble { get; set; }
        public bool? ValueBoolean { get; set; }
        public Guid? ValueGuid { get; set; }
        public string ValueString { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Fascicle Fascicle { get; set; }
        public virtual Dossier Dossier { get; set; }
        #endregion
    }
}
