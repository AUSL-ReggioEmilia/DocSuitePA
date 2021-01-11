using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowRepositoryValidatorMapper : BaseMapper<WorkflowRepository, WorkflowRepositoryValidator>, IWorkflowRepositoryValidatorMapper
    {
        public WorkflowRepositoryValidatorMapper() { }

        public override WorkflowRepositoryValidator Map(WorkflowRepository entity, WorkflowRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ActiveFrom = entity.ActiveFrom;
            entityTransformed.ActiveTo = entity.ActiveTo;
            entityTransformed.Name = entity.Name;
            entityTransformed.Version = entity.Version;
            entityTransformed.Xaml = entity.Xaml;
            entityTransformed.Json = entity.Json;
            entityTransformed.Status = entity.Status;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.CustomActivities = entity.CustomActivities;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.WorkflowInstances = entity.WorkflowInstances;
            entityTransformed.Roles = entity.Roles;
            entityTransformed.WorkflowRoleMappings = entity.WorkflowRoleMappings;
            entityTransformed.FascicleWorkflowRepositories = entity.FascicleWorkflowRepositories;



            #endregion

            return entityTransformed;
        }

    }
}
