using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSContact : DSWUDSRelationBaseEntity<Contact>
    {
        #region [ Constructor ]
        public UDSContact() : this(Guid.NewGuid()) { }

        public UDSContact(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string ContactManual { get; set; }
        public short? ContactType { get; set; }
        public string ContactLabel { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
