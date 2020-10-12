using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public class TenantModel
    {
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
        public string Note { get; set; }
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        public TenantType TenantType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
    }
}
