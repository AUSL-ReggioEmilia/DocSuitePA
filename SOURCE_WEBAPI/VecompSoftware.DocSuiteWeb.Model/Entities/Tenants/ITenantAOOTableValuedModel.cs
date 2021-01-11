using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Tenants
{
    public interface ITenantAOOTableValuedModel
    {
        Guid? TenantAOO_IdTenantAOO { get; set; }
        string TenantAOO_Name { get; set; }
    }
}
