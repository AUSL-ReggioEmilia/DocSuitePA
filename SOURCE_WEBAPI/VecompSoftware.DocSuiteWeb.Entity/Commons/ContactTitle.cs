using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class ContactTitle : DSWBaseEntity
    {
        #region [ Constructor ]
        public ContactTitle() : this(Guid.NewGuid()) { }
        public ContactTitle(Guid uniqueId)
            : base(uniqueId)
        {
            Contacts = new HashSet<Contact>();
            ProtocolContactManuals = new HashSet<ProtocolContactManual>();
        }

        #endregion

        #region[ Properties ]

        public string Code { get; set; }
        public string Description { get; set; }
        public bool isActive { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ProtocolContactManual> ProtocolContactManuals { get; set; }
        #endregion
    }
}
