using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowInstanceFacade : BaseProtocolFacade<WorkflowInstance, Guid, WorkflowInstanceDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowInstanceFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}

