using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Exceptions;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Collaborations
{
    public class CollaborationFacade : FacadeWebAPIBase<Collaboration, CollaborationDao>
    {
        public CollaborationFacade(ICollection<TenantModel> model, Tenant currentTenant)
            :base(model.Select(s => new WebAPITenantConfiguration<Collaboration, CollaborationDao>(s)).ToList(), currentTenant)
        {

        }

        public Collaboration GetByIncremental(int incremental)
        {
            return WebAPIImpersonatorFacade.ImpersonateDao<CollaborationDao, Collaboration, Collaboration>(this.CurrentTenantConfiguration.Dao,
            (impersonationType, dao) =>
            {
                try
                {
                    return dao.GetByIncremental(incremental);
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName, string.Format(ERROR_MESSAGE, "Count", CurrentTenantConfiguration.Tenant.TenantName), ex);
                    throw new WebAPIException<int>(ex.Message, ex);
                }
            });        
        }
    }
}
