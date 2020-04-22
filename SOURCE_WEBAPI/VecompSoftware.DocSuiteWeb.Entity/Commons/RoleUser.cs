using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class RoleUser : DSWBaseEntity
    {
        #region [ Constructor ]

        public RoleUser() : this(Guid.NewGuid()) { }
        public RoleUser(Guid uniqueId)
            : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]

        public string Type { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public bool? Enabled { get; set; }
        public string Email { get; set; }
        public bool? IsMainRole { get; set; }
        public DSWEnvironmentType DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Role Role { get; set; }

        #endregion
    }
}
