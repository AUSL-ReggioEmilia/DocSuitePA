using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolDocumentType : DSWBaseEntity
    {
        #region [ Constructor ]

        public ProtocolDocumentType() : this(Guid.NewGuid()) { }
        public ProtocolDocumentType(Guid uniqueId)
            : base(uniqueId)
        {
            Protocols = new HashSet<Protocol>();
        }
        #endregion

        #region[ Properties ]

        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? HiddenFields { get; set; }
        public bool? NeedPackage { get; set; }
        public string CommonUser { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual ICollection<Protocol> Protocols { get; set; }
        #endregion
    }
}
