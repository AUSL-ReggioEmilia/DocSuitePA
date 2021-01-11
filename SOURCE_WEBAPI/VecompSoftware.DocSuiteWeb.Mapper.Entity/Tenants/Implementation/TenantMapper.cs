using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenants
{
    public class TenantMapper : BaseEntityMapper<Tenant, Tenant>, ITenantMapper
    {
        public override Tenant Map(Tenant entity, Tenant entityTransformed)
        {
            #region [ Base ]
            entityTransformed.TenantName = entity.TenantName;
            entityTransformed.CompanyName = entity.CompanyName;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.TenantTypology = entity.TenantTypology;
            #endregion

            return entityTransformed;
        }

    }
}
