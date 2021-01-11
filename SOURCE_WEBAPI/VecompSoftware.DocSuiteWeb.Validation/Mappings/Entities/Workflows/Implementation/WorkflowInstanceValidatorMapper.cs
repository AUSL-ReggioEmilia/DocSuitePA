using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowInstanceValidatorMapper : BaseMapper<WorkflowInstance, WorkflowInstanceValidator>, IWorkflowInstanceValidatorMapper
    {
        public WorkflowInstanceValidatorMapper() { }

        public override WorkflowInstanceValidator Map(WorkflowInstance entity, WorkflowInstanceValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Status = entity.Status;
            entityTransformed.InstanceId = entity.InstanceId;
            entityTransformed.Json = entity.Json;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.Subject = entity.Subject;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowActivities = entity.WorkflowActivities;
            entityTransformed.WorkflowProperties = entity.WorkflowProperties;
            entityTransformed.WorkflowRepository = entity.WorkflowRepository;
            entityTransformed.Collaborations = entity.Collaborations;
            entityTransformed.WorkflowInstanceLogs = entity.WorkflowInstanceLogs;
            entityTransformed.WorkflowInstanceRoles = entity.WorkflowInstanceRoles;
            entityTransformed.Dossiers = entity.Dossiers;
            entityTransformed.Fascicles = entity.Fascicles;
            #endregion

            return entityTransformed;
        }

    }
}
