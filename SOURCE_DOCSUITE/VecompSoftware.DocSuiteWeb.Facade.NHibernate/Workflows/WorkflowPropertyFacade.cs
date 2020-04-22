using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowPropertyFacade : BaseProtocolFacade<WorkflowProperty, Guid, WorkflowPropertyDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowPropertyFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        /// <summary>
        /// Recupera una specifica <typeparamref name="WorkflowProperty"/> di una particolare Activity
        /// </summary>
        /// <param name="idWorkflowActivity">Id della <typeparamref name="WorkflowActivity"/> da filtrare</param>
        /// <param name="propertyName">Nome della proprietà da filtrare</param>
        /// <returns></returns>
        [Obsolete("Utilizzare i metodi GetWorkflowPropertyByActivityAndName e GetWorkflowPropertyByInstanceAndName di Facade WebAPI per recuperare le singole WorkflowProperty per Activity o Instance")]
        public WorkflowProperty GetWorkflowPropertyByActivityAndName(Guid idWorkflowActivity, string propertyName)
        {
            return _dao.GetWorkflowPropertyByActivityAndName(idWorkflowActivity, propertyName);
        }

        /// <summary>
        /// Recupera una specifica <typeparamref name="WorkflowProperty"/> di una particolare WorkflowInstance
        /// </summary>
        /// <param name="idWorkflowInstance">Id della <typeparamref name="WorkflowInstance"/> da filtrare</param>
        /// <param name="propertyName">Nome della proprietà da filtrare</param>
        /// <returns></returns>
        [Obsolete("Utilizzare i metodi GetWorkflowPropertyByInstanceAndName e GetWorkflowPropertyByInstanceAndName di Facade WebAPI per recuperare le singole WorkflowProperty per Activity o Instance")]
        public WorkflowProperty GetWorkflowPropertyByInstanceAndName(Guid idWorkflowInstance, string propertyName)
        {
            return _dao.GetWorkflowPropertyByInstanceAndName(idWorkflowInstance, propertyName);
        }
        #endregion
    }
}
