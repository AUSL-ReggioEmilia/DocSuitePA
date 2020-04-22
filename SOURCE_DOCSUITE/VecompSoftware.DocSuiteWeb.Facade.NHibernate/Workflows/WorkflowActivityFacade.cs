using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowActivityFacade : BaseProtocolFacade<WorkflowActivity, Guid, WorkflowActivityDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowActivityFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
