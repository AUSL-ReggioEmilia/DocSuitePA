using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{
    public class WorkflowAuthorizationDao : BaseNHibernateDao<WorkflowAuthorization>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationDao() : base() { }

        public WorkflowAuthorizationDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Recupera una specifica <typeparamref name="WorkflowProperty"/> di una particolare Activity
        /// </summary>
        /// <param name="idWorkflowActivity">Id della <typeparamref name="WorkflowActivity"/> da filtrare</param>
        /// <param name="propertyName">Nome della proprietà da filtrare</param>
        /// <returns></returns>   
        public WorkflowAuthorization GetWorkflowAuthorizationByActivity(Guid idWorkflowActivity)
        {
            WorkflowAuthorization prop = NHibernateSession.QueryOver<WorkflowAuthorization>()
                                                    .Where(x => x.WorkflowActivity.Id == idWorkflowActivity && x.IsHandler == "1")
                                                    .SingleOrDefault();

            return prop;
        }

        public bool IsAuthorized(Guid idWorkflowActivity, string account)
        {
            int count = NHibernateSession.QueryOver<WorkflowAuthorization>()
                .Where(x => x.WorkflowActivity.Id == idWorkflowActivity && x.Account == account)
                .RowCount();

            return count > 0;
        }
        #endregion
    }
}
