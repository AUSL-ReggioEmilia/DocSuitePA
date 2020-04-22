using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowAuthorizationFacade : BaseProtocolFacade<WorkflowAuthorization, Guid, WorkflowAuthorizationDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        public WorkflowAuthorization GetWorkflowAuthorizationByActivity(Guid idWorkflowActivity)
        {
            return _dao.GetWorkflowAuthorizationByActivity(idWorkflowActivity);
        }

        public bool IsAuthorized(Guid idWorkflowActivity)
        {
            return _dao.IsAuthorized(idWorkflowActivity, _userName);
        }
        #endregion
    }
}
