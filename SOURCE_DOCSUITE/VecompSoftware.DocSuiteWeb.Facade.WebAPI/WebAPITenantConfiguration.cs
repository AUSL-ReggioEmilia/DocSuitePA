using System;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public class WebAPITenantConfiguration<T, TDao> : IWebAPITenantConfiguration<T, TDao>
        where TDao : IWebAPIDao<T>
    {
        public TenantModel Tenant { get; set; }
        public TDao Dao { get; set; }
        public bool IsCurrent
        {
            get
            {
                return Tenant != null && Tenant.CurrentTenant;
            }
        }

        public WebAPITenantConfiguration(TenantModel tenant)
        {
            Tenant = tenant;
            Dao = (TDao)Activator.CreateInstance(typeof(TDao), tenant);
        }
    }
}
