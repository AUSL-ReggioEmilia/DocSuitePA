using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenants
{
    public class TenantConfigurationMapper : BaseEntityMapper<TenantConfiguration, TenantConfiguration>, ITenantConfigurationMapper
    {
        public override TenantConfiguration Map(TenantConfiguration entity, TenantConfiguration entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ConfigurationType = entity.ConfigurationType;
            entityTransformed.JsonValue = entity.JsonValue;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            #endregion

            return entityTransformed;
        }

    }
}
