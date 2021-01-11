using System;
using System.Collections.Generic;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Services.Logging;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Exceptions;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
{
    public class TenantFinder : BaseWebAPIFinder<Tenant, Tenant>
    {
        public TenantFinder(TenantModel tenant) : base(tenant)
        {
        }

        public TenantFinder(IReadOnlyCollection<TenantModel> tenants) : base(tenants)
        {
        }

        #region [ Properties ] 
        public bool IncludeContainers { get; set; }
        public bool IncludeTenantAOO { get; set; }
        public Guid? IdTenantAOO { get; set; }
        #endregion

        #region [ Method ] 
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IncludeContainers)
            {
                odataQuery = odataQuery.Expand("Containers");
            }
            if (IncludeTenantAOO)
            {
                odataQuery = odataQuery.Expand("TenantAOO");
            }
            if (IdTenantAOO.HasValue)
            {
                odataQuery = odataQuery.Filter($"TenantAOO/UniqueId eq {IdTenantAOO}");
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            IncludeContainers = false;   
        }
        private TResult CurrentTenantExecutionWebAPI<TModel, TResult>(Func<TenantModel, TResult> func, string methodName)
        {
            string errorMessage = string.Concat("Errore nell'esecuzione del metodo ", methodName, " .");

            try
            {
                return func(CurrentTenant);
            }
            catch (Exception ex)
            {
                FileLogger.Error(Logger, errorMessage, ex);
                throw new WebAPIException<TResult>(ex.Message, ex);
            }
        }

        public override int Count()
        {
            return CurrentTenantExecutionWebAPI<Tenant, int>((tenant) =>
            {
                WebApiHttpClient httpClient = GetWebAPIClient(CurrentTenant);
                string result = httpClient.GetAsync<Tenant>().WithRowQuery("/$count").ResponseToString();
                return int.Parse(result);
            }, nameof(Count));
        }
        #endregion
    }
}
