using System;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSMessage : DSWUDSRelationBaseEntity<Message>
    {
        #region [ Constructor ]
        public UDSMessage() : this(Guid.NewGuid()) { }

        public UDSMessage(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
