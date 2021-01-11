using System;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSPECMail : DSWUDSRelationBaseEntity<PECMail>
    {
        #region [ Constructor ]

        public UDSPECMail() : this(Guid.NewGuid()) { }

        public UDSPECMail(Guid uniqueId)
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
