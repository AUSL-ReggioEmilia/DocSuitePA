using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class ContactPlaceName : DSWBaseEntity
    {
        #region [ Constructor ]
        public ContactPlaceName() : this(Guid.NewGuid()) { }
        public ContactPlaceName(Guid uniqueId)
            : base(uniqueId)
        {
            Contacts = new HashSet<Contact>();
            ProtocolContactManuals = new HashSet<ProtocolContactManual>();
        }

        #endregion

        #region[ Properties ]

        public string Description { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }
        #endregion
    }
}
