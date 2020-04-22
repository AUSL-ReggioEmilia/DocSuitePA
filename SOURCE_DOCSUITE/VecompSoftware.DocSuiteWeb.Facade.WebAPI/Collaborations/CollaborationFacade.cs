using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager.Exceptions;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Collaborations
{
    public class CollaborationFacade : FacadeWebAPIBase<Collaboration, CollaborationDao>
    {
        public CollaborationFacade(ICollection<TenantModel> model)
            :base(model.Select(s => new WebAPITenantConfiguration<Collaboration, CollaborationDao>(s)).ToList())
        {

        }

        public Collaboration GetByIncremental(int incremental)
        {
            IWebAPITenantConfiguration<Collaboration, CollaborationDao> configuration = _daoConfigurations.FirstOrDefault();
            try
            {
                return configuration.Dao.GetByIncremental(incremental);
            }
            catch (Exception ex)
            {
                FileLogger.Error(Logger, string.Format(ERROR_MESSAGE, "Count", configuration.Tenant.TenantName), ex);
                throw new WebAPIException<int>(ex.Message, ex);
            }            
        }
    }
}
