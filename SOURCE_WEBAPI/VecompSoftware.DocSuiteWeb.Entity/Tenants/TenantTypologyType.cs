using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{
    public enum TenantTypologyType : short
    {
        [Description("InternalTenant")]
        InternalTenant = 0,
        [Description("ExternalTenant")]
        ExternalTenant = 1
    }
}
