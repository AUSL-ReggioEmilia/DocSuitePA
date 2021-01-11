using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSSchemaRepository : DSWBaseEntity
    {
        #region Constructors

        public UDSSchemaRepository() : this(Guid.NewGuid()) { }

        public UDSSchemaRepository(Guid uniqueId)
            : base(uniqueId)
        {
            Repositories = new HashSet<UDSRepository>();
        }

        #endregion

        #region [ Properties ]

        public string SchemaXML { get; set; }

        public short Version { get; set; }

        public DateTimeOffset ActiveDate { get; set; }

        public DateTimeOffset? ExpiredDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<UDSRepository> Repositories { get; set; }

        #endregion
    }
}
