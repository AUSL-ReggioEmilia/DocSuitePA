using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Tenants
{
    public class TenantWorkflowRepositoryMapper : BaseEntityMapper<TenantWorkflowRepository, TenantWorkflowRepository>, ITenantWorkflowRepositoryMapper
    {
        public override TenantWorkflowRepository Map(TenantWorkflowRepository entity, TenantWorkflowRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.JsonValue = entity.JsonValue;
            entityTransformed.IntegrationModuleName = entity.IntegrationModuleName;
            entityTransformed.Conditions = entity.Conditions;
            entityTransformed.ConfigurationType = entity.ConfigurationType;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;

            #endregion

            return entityTransformed;
        }
    }
}
