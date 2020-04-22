using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class PrivacyLevel : DSWBaseEntity
    {
        #region [ Constructor ]

        public PrivacyLevel() : this(Guid.NewGuid()) { }
        public PrivacyLevel(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]

        public int Level { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Colour { get; set; }
        #endregion

        #region [ Navigation Properties ]
        #endregion
    }
}
