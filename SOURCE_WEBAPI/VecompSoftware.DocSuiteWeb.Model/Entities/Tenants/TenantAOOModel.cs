using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public class TenantAOOModel
    {
        #region [ Constructor ]
        public TenantAOOModel() { }
        #endregion

        #region [ Properties ]
        public Guid IdTenantAOO { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string CategorySuffix { get; set; }
        public TenantType TenantType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion
    }
}
