﻿using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
{
    public class WorkflowRoleMappingFacade : FacadeWebAPIBase<WorkflowRoleMapping, WorkflowRoleMappingDao>
    {
        #region [ Fields ]  
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<WorkflowRoleMapping, WorkflowRoleMappingDao>(s)).ToList(), currentTenant)
        {
            
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
