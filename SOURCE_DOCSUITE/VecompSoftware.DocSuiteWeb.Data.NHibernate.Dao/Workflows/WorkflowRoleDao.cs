using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{
    public class WorkflowRoleDao : BaseNHibernateDao<WorkflowRole>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowRoleDao() : base() { }

        public WorkflowRoleDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion

        #region [ Methods ]       
        #endregion
    }
}
