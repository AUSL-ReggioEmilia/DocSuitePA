using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Workflows
{
    public class WorkflowAuthorizationMapper : BaseEntityMapper<WorkflowAuthorization, WorkflowAuthorization>, IWorkflowAuthorizationMapper
    {
        public override WorkflowAuthorization Map(WorkflowAuthorization entity, WorkflowAuthorization entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Account = entity.Account;
            entityTransformed.IsHandler = entity.IsHandler;

            #endregion

            return entityTransformed;
        }

    }
}
