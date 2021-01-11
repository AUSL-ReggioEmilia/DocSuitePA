using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants
{
    public class TenantWorkflowRepositoryValidatorMapper : BaseMapper<TenantWorkflowRepository, TenantWorkflowRepositoryValidator>, ITenantWorkflowRepositoryValidatorMapper
    {
        public TenantWorkflowRepositoryValidatorMapper() { }

        public override TenantWorkflowRepositoryValidator Map(TenantWorkflowRepository entity, TenantWorkflowRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.JsonValue = entity.JsonValue;
            entityTransformed.IntegrationModuleName = entity.IntegrationModuleName;
            entityTransformed.Conditions = entity.Conditions;
            entityTransformed.ConfigurationType = entity.ConfigurationType;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.WorkflowRepository = entity.WorkflowRepository;
            entityTransformed.Tenant = entity.Tenant;

            #endregion

            return entityTransformed;
        }
    }
}
