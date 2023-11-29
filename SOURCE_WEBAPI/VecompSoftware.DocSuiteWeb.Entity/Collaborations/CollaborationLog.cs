using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationLog : DSWBaseLogEntity<Collaboration, string>
    {
        #region [ Constructor ]

        public CollaborationLog() : this(Guid.NewGuid()) { }

        public CollaborationLog(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public DateTime? LogDate { get; set; }
        /// <summary>
        /// Get or set CollaborationIncremental
        /// </summary>
        public int? CollaborationIncremental { get; set; }

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public short? Incremental { get; set; }

        /// <summary>
        /// Get or set IdChain
        /// </summary>
        public int? IdChain { get; set; }

        /// <summary>
        /// Get or set Program
        /// </summary>
        public string Program { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
