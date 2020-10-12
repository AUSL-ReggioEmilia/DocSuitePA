using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public enum TenantTypologyType : short
    {
        [Description("InternalTenant")]
        InternalTenant = 0,
        [Description("ExternalTenant")]
        ExternalTenant = 1
    }
}
