using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{
    public abstract class DSWTenantBaseEntity : DSWBaseEntity
    {
        protected DSWTenantBaseEntity(Guid uniqueId) : base(uniqueId)
        {
        }

        #region [ Properties ]
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public string Note { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion

    }
}
