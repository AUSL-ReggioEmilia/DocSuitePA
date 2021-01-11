using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowInstanceLogMapper : BaseEntityMapper<WorkflowInstanceLog, WorkflowInstanceLog>, IWorkflowInstanceLogMapper
    {
        public override WorkflowInstanceLog Map(WorkflowInstanceLog entity, WorkflowInstanceLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            #endregion

            return entityTransformed;
        }

    }
}
