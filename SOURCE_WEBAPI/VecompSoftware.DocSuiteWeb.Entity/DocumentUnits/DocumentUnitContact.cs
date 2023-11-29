using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentUnits
{
    public class DocumentUnitContact : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentUnitContact() : this(Guid.NewGuid()) { }
        public DocumentUnitContact(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string ContactManual { get; set; }

        public short ContactType { get; set; }

        public string ContactLabel { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Contact Contact { get; set; }
        public virtual DocumentUnit DocumentUnit { get; set; }
        #endregion
    }
}
