using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowRoleMappingMapper : BaseEntityMapper<WorkflowRoleMapping, WorkflowRoleMapping>, IWorkflowRoleMappingMapper
    {
        public override WorkflowRoleMapping Map(WorkflowRoleMapping entity, WorkflowRoleMapping entityTransformed)
        {
            #region [ Base ]

            entityTransformed.MappingTag = entity.MappingTag;
            entityTransformed.IdInternalActivity = entity.IdInternalActivity;
            entityTransformed.AccountName = entity.AccountName;
            entityTransformed.AuthorizationType = entity.AuthorizationType;

            #endregion

            return entityTransformed;
        }

    }
}
