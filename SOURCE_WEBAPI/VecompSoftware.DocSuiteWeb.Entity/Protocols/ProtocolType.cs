using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{

    public class ProtocolType : DSWBaseEntity
    {
        #region [ Constructor ]
        public ProtocolType() : this(Guid.NewGuid()) { }

        public ProtocolType(Guid uniqueId)
            : base(uniqueId)
        {
            Containers = new HashSet<Container>();
            Protocols = new HashSet<Protocol>();
        }
        #endregion

        #region [ Properties ]

        public string Description { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Container> Containers { get; set; }
        public virtual ICollection<Protocol> Protocols { get; set; }
        #endregion
    }
}
