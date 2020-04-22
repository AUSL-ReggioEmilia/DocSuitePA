using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{
    public class WorkflowPropertyDao : BaseNHibernateDao<WorkflowProperty>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowPropertyDao() : base(){ }

        public WorkflowPropertyDao(string sessionFactoryName)
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
        public WorkflowProperty GetWorkflowPropertyByActivityAndName(Guid idWorkflowActivity, string propertyName)
        {
            WorkflowProperty prop = NHibernateSession.QueryOver<WorkflowProperty>()
                                                    .Where(x => x.WorkflowActivity.Id == idWorkflowActivity)
                                                    .Where(x => x.Name == propertyName)
                                                    .SingleOrDefault();

            return prop;
        }

        /// <summary>
        /// Recupera una specifica <typeparamref name="WorkflowProperty"/> di un particolare Instance
        /// </summary>
        /// <param name="idWorkflowInstance">Id della <typeparamref name="WorkflowInstance"/> da filtrare</param>
        /// <param name="propertyName">Nome della proprietà da filtrare</param>
        /// <returns></returns>   
        public WorkflowProperty GetWorkflowPropertyByInstanceAndName(Guid idWorkflowInstance, string propertyName)
        {
            WorkflowProperty prop = NHibernateSession.QueryOver<WorkflowProperty>()
                                                    .Where(x => x.WorkflowInstance.Id == idWorkflowInstance)
                                                    .Where(x => x.Name == propertyName)
                                                    .SingleOrDefault();

            return prop;
        }
        #endregion
    }
}
