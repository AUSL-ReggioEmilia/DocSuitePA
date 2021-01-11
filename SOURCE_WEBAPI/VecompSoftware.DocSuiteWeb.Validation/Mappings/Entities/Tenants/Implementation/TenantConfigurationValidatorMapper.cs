using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants
{
    public class TenantConfigurationValidatorMapper : BaseMapper<TenantConfiguration, TenantConfigurationValidator>, ITenantConfigurationValidatorMapper
    {
        public TenantConfigurationValidatorMapper() { }

        public override TenantConfigurationValidator Map(TenantConfiguration entity, TenantConfigurationValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ConfigurationType = entity.ConfigurationType;
            entityTransformed.JsonValue = entity.JsonValue;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Tenant = entity.Tenant;

            #endregion

            return entityTransformed;
        }

    }
}
