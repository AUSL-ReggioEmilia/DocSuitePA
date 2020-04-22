using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowInstanceMapper : BaseEntityMapper<WorkflowInstance, WorkflowInstance>, IWorkflowInstanceMapper
    {
        public override WorkflowInstance Map(WorkflowInstance entity, WorkflowInstance entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Status = entity.Status;
            entityTransformed.InstanceId = entity.InstanceId;
            entityTransformed.Json = entity.Json;

            #endregion

            return entityTransformed;
        }

    }
}
