using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowRepositoryFacade : BaseProtocolFacade<WorkflowRepository, Guid, WorkflowRepositoryDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowRepositoryFacade(string userName)
        : base()
    {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]

        public IList<WorkflowRepository> GetActiveRepositories()
        {
            return _dao.GetActiveRepositories();
        }

        public IList<WorkflowRepository> GetNameRepositories(string propertyName)
        {
            return _dao.GetNameRepositories(propertyName);
        }
        #endregion
    }
}
