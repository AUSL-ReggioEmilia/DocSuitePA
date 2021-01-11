using System;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSCollaboration : DSWUDSRelationBaseEntity<Collaboration>
    {
        #region [ Constructor ]

        public UDSCollaboration() : this(Guid.NewGuid()) { }

        public UDSCollaboration(Guid uniqueId)
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
