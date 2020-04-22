using System;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolLog : DSWBaseLogEntity<Protocol, string>, IUnauditableEntity
    {
        #region [ Constructor ]

        public ProtocolLog() : this(Guid.NewGuid())
        {
            LogDate = DateTime.UtcNow;
        }
        public ProtocolLog(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region[ Properties ]

        public short Year { get; set; }
        public int Number { get; set; }

        public DateTime LogDate { get; set; }

        public string Program { get; set; }

        #endregion

        #region[ Navigation Properties ]

        #endregion
    }
}
