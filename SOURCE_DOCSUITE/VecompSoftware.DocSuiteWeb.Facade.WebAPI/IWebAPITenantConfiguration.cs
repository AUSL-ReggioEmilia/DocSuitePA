using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI
{
    public interface IWebAPITenantConfiguration<T, TDao> 
        where TDao : IWebAPIDao<T>
    {
        TenantModel Tenant { get; set; }
        TDao Dao { get; set; }
        bool IsCurrent { get; }
    }
}
