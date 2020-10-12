using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenants
{
    public class TenantAOOMapper : BaseEntityMapper<TenantAOO, TenantAOO>, ITenantAOOMapper
    {
        public override TenantAOO Map(TenantAOO entity, TenantAOO entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.Note = entity.Note;
            entityTransformed.CategorySuffix = entity.CategorySuffix;
            entityTransformed.TenantTypology = entity.TenantTypology;
            #endregion
            return entityTransformed;
        }
    }
}
