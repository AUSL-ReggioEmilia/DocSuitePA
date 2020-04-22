using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class SecurityUserWorkflowAuthorizationMapper : BaseEntityMapper<SecurityUser, WorkflowAuthorization>, ISecurityUserWorkflowAuthorizationMapper
    {
        public SecurityUserWorkflowAuthorizationMapper()
        {

        }

        public override WorkflowAuthorization Map(SecurityUser entity, WorkflowAuthorization entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = string.Concat(entity.UserDomain, "\\", entity.Account);
            #endregion

            return entityTransformed;
        }

    }
}
