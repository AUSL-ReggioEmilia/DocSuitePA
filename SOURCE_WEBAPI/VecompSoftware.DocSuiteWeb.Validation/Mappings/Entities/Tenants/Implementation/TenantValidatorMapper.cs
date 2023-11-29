using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants
{
    public class TenantValidatorMapper : BaseMapper<Tenant, TenantValidator>, ITenantValidatorMapper
    {
        public TenantValidatorMapper() { }

        public override TenantValidator Map(Tenant entity, TenantValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.TenantName = entity.TenantName;
            entityTransformed.CompanyName = entity.CompanyName;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Note = entity.Note;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Configurations = entity.Configurations;
            entityTransformed.Containers = entity.Containers;
            entityTransformed.PECMailBoxes = entity.PECMailBoxes;
            entityTransformed.TenantWorkflowRepositories = entity.TenantWorkflowRepositories;
            entityTransformed.WorkflowActivities = entity.WorkflowActivities;
            entityTransformed.TenantTypologyType = entity.TenantTypology;

            #endregion

            return entityTransformed;
        }

    }
}
