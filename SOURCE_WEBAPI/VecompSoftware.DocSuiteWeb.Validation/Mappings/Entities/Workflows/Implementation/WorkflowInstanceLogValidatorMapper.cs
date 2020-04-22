using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows
{
    public class WorkflowInstanceLogValidatorMapper : BaseMapper<WorkflowInstanceLog, WorkflowInstanceLogValidator>, IWorkflowInstanceLogValidatorMapper
    {
        public WorkflowInstanceLogValidatorMapper() { }


        public override WorkflowInstanceLogValidator Map(WorkflowInstanceLog entity, WorkflowInstanceLogValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.WorkflowInstance = entity.Entity;
            #endregion

            return entityTransformed;
        }
    }
}
