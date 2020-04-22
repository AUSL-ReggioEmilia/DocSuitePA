using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{
    public class WorkflowRepositoryDao : BaseNHibernateDao<WorkflowRepository>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WorkflowRepositoryDao() : base(){ }

        public WorkflowRepositoryDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion

        #region [ Methods ]

        public IList<WorkflowRepository> GetActiveRepositories()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            IList<WorkflowRepository> results = NHibernateSession.QueryOver<WorkflowRepository>()
                                           .Where(x => now >= x.ActiveFrom && (x.ActiveTo == null || now < x.ActiveTo) && x.Status == WorkflowStatus.Active)
                                           .List<WorkflowRepository>();
            return results;
        }
        public IList<WorkflowRepository> GetNameRepositories(string propertyName)
        {
       
           IList<WorkflowRepository> results = NHibernateSession.QueryOver<WorkflowRepository>()
                                           .Where(Restrictions.InsensitiveLike("Name", propertyName, MatchMode.Anywhere))
                                           .OrderBy(x => x.Name)
                                           .Asc                                           
                                           .List<WorkflowRepository>();
            return results;
        }
        #endregion
    }
}
