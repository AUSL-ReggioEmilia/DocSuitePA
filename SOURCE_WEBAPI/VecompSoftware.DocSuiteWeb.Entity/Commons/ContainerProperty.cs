using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class ContainerProperty : DSWBaseEntity
    {
        #region [ Constructor ]

        public ContainerProperty() : this(Guid.NewGuid()) { }
        public ContainerProperty(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]
        public string Name { get; set; }
        public ContainerPropertyType ContainerType { get; set; }
        public int? ValueInt { get; set; }
        public DateTime? ValueDate { get; set; }
        public double? ValueDouble { get; set; }
        public bool? ValueBoolean { get; set; }
        public Guid? ValueGuid { get; set; }
        public string ValueString { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Container Container { get; set; }
        #endregion
    }
}
