using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{

    public class WorkflowInstanceDao : BaseNHibernateDao<WorkflowInstance>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowInstanceDao() : base() { }

        public WorkflowInstanceDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
