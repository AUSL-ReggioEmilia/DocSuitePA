using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Workflows;
using VecompSoftware.NHibernateManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Workflows
{
    public class WorkflowRoleMappingDao : BaseNHibernateDao<WorkflowRoleMapping>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingDao() : base() { }

        public WorkflowRoleMappingDao(string sessionFactoryName)
            : base(sessionFactoryName)
        {
        }
        #endregion

        #region [ Methods ]
        public IList<WorkflowRoleMapping>GetWorkflowRoleMappingsByWorkflowRepositoryId(Guid id)
        {

            IList<WorkflowRoleMapping> results = NHibernateSession.QueryOver<WorkflowRoleMapping>()
                                            .Where(x => x.WorkflowRepository.Id == id)
                                            .OrderBy(x => x.MappingTag)
                                            .Asc
                                            .List<WorkflowRoleMapping>();
                                            
            return results;
        }
        #endregion
    }
}
