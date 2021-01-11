using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows;
using VecompSoftware.DocSuiteWeb.DTO.WebAPI;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Exceptions;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
{
    public class WorkflowPropertyFacade : FacadeWebAPIBase<WorkflowProperty, WorkflowPropertyDao>
    {
        #region [ Fields ]  
        private readonly WebAPIDtoMapper<WorkflowProperty> _mapper;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowPropertyFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<WorkflowProperty, WorkflowPropertyDao>(s)).ToList(), currentTenant)
        {
            this._mapper = new WebAPIDtoMapper<WorkflowProperty>();
        }
        #endregion

        #region [ Methods ]        
        #endregion
    }
}
