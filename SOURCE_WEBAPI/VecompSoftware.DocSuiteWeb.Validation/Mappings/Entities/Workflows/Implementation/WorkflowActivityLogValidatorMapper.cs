using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowActivityLogValidatorMapper : BaseMapper<WorkflowActivityLog, WorkflowActivityLogValidator>, IWorkflowActivityLogValidatorMapper
    {
        public WorkflowActivityLogValidatorMapper() { }

        public override WorkflowActivityLogValidator Map(WorkflowActivityLog entity, WorkflowActivityLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowActivity = entity.Entity;
            #endregion

            return entityTransformed;
        }

    }
}
