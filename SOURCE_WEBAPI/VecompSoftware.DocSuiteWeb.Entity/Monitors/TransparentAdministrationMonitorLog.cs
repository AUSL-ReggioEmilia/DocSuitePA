using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Monitors
{
    public class TransparentAdministrationMonitorLog : DSWBaseEntity
    {
        #region [ Constructor ]
        public TransparentAdministrationMonitorLog() : this(Guid.NewGuid()) { }

        public TransparentAdministrationMonitorLog(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]

        public string DocumentUnitName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual DocumentUnit DocumentUnit { get; set; }
        public virtual Role Role { get; set; }
        #endregion
    }
}
