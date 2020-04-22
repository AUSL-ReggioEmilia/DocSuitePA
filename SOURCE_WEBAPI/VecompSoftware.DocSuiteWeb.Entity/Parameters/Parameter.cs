using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Parameters
{
    public class Parameter : DSWBaseEntity
    {
        #region [ Constructor ]
        public Parameter() : this(Guid.NewGuid()) { }
        public Parameter(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region [ Properties ]

        public int Incremental { get; set; }
        public short LastUsedYear { get; set; }
        public int LastUsedNumber { get; set; }
        public bool Locked { get; set; }
        public string Password { get; set; }
        public short LastUsedIdCategory { get; set; }
        public short LastUsedIdRecipient { get; set; }
        public short LastUsedIdContainer { get; set; }
        public short Version { get; set; }
        public short LastUsedIdDistributionList { get; set; }
        public string DomainName { get; set; }
        public string AlternativePassword { get; set; }
        public string ServiceField { get; set; }
        public short LastUsedIdRole { get; set; }
        public short LastUsedIdRoleUser { get; set; }
        public int? LastUsedIdResolution { get; set; }
        public short LastUsedResolutionYear { get; set; }
        public short LastUsedResolutionNumber { get; set; }
        public short LastUsedBillNumber { get; set; }
        public short LastUsedYearReg { get; set; }
        public int? LastUsedNumberReg { get; set; }
        #endregion
    }
}
