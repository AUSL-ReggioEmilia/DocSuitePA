using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskLog : DSWBaseLogEntity<Desk, DeskLogType>
    {
        #region [ Constructor ]

        public DeskLog() : this(Guid.NewGuid()) { }

        public DeskLog(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public DateTime LogDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
