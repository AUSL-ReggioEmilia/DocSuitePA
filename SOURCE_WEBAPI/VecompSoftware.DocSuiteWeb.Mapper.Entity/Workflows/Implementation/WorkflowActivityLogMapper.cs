using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowActivityLogMapper : BaseEntityMapper<WorkflowActivityLog, WorkflowActivityLog>, IWorkflowActivityLogMapper
    {
        public override WorkflowActivityLog Map(WorkflowActivityLog entity, WorkflowActivityLog entityTransformed)
        {
            #region [ Base ]

            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;

            #endregion

            return entityTransformed;
        }

    }
}
