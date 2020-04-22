using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSTypology : DSWBaseEntity
    {
        #region [ Constructor ]

        public UDSTypology() : this(Guid.NewGuid()) { }

        public UDSTypology(Guid uniqueId)
            : base(uniqueId)
        {
            UDSRepositories = new HashSet<UDSRepository>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public UDSTypologyStatus Status { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<UDSRepository> UDSRepositories { get; set; }

        #endregion
    }
}
