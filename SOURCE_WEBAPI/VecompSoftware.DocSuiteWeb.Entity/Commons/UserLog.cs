using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class UserLog : DSWBaseEntity
    {
        #region [ Constructor ]

        public UserLog() : this(Guid.NewGuid()) { }
        public UserLog(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]
        public string SystemUser { get; set; }
        public string SystemServer { get; set; }
        public string SystemComputer { get; set; }
        public int? AccessNumber { get; set; }
        public DateTimeOffset? PrevOperationDate { get; set; }
        public string SessionId { get; set; }
        public bool? AdvancedScanner { get; set; }
        public bool? AdvancedViewer { get; set; }
        public string UserMail { get; set; }
        public string MobilePhone { get; set; }
        public string DefaultAdaptiveSearchControls { get; set; }
        public string AdaptiveSearchStatistics { get; set; }
        public string AdaptiveSearchEvaluated { get; set; }
        public int PrivacyLevel { get; set; }
        public Guid? CurrentTenantId { get; set; }
        public string UserProfile { get; set; }
        #endregion

        #region [ Navigation Properties ]
        #endregion
    }
}
