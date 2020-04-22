using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleLog : DSWBaseLogEntity<Fascicle, FascicleLogType>
    {
        #region [ Constructor ]

        public FascicleLog() : this(Guid.NewGuid()) { }

        public FascicleLog(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]


        #endregion

        #region [ Navigation Properties ]


        #endregion
    }
}
