using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Workflows
{
    public class WorkflowRoleMappingFacade : BaseProtocolFacade<WorkflowRoleMapping, Guid, WorkflowRoleMappingDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingFacade(string userName)
            : base()
        {
            _userName = userName;
        }
        #endregion

        #region [ Methods ]
        public IList<WorkflowRoleMapping> GetWorkflowRoleMappingsByWorkflowRepositoryId(Guid id)
        {
            return _dao.GetWorkflowRoleMappingsByWorkflowRepositoryId(id);
        }
        #endregion
    }
}
