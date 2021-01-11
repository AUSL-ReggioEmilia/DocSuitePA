using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Resolutions
{
    public class ResolutionLog : DSWBaseLogEntity<Resolution, string>
    {
        #region [ Constructor ]

        public ResolutionLog() : this(Guid.NewGuid()) { }

        public ResolutionLog(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        #region [ Properties ]
        public int IdResolution { get; set; }

        public DateTime LogDate { get; set; }

        public string Program { get; set; }

        #endregion
    }
}
