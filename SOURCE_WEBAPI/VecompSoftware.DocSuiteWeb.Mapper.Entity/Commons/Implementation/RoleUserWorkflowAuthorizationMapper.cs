using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class RoleUserWorkflowAuthorizationMapper : BaseEntityMapper<RoleUser, WorkflowAuthorization>, IRoleUserWorkflowAuthorizationMapper
    {
        public RoleUserWorkflowAuthorizationMapper()
        {

        }

        public override WorkflowAuthorization Map(RoleUser entity, WorkflowAuthorization entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = entity.Account;
            #endregion

            return entityTransformed;
        }

    }
}
