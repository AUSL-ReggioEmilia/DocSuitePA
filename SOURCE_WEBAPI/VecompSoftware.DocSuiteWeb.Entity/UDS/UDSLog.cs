using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSLog : DSWBaseLogEntity<UDSRepository, UDSLogType>, IUnauditableEntity
    {
        #region [ Constructor ]

        public UDSLog() : this(Guid.NewGuid()) { }

        public UDSLog(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        #region [ Properties ]

        public int Environment { get; set; }

        public Guid IdUDS { get; set; }

        #endregion
    }
}
