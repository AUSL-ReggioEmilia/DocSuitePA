using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowInstanceRoleMapper : BaseEntityMapper<WorkflowInstanceRole, WorkflowInstanceRole>, IWorkflowInstanceRoleMapper
    {
        public override WorkflowInstanceRole Map(WorkflowInstanceRole entity, WorkflowInstanceRole entityTransformed)
        {
            #region [ Base ]

            entityTransformed.AuthorizationType = entity.AuthorizationType;

            #endregion

            return entityTransformed;
        }

    }
}
