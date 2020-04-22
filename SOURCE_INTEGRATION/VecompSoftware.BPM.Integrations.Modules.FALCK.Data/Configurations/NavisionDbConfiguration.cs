using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Configurations
{
    public class NavisionDbConfiguration : DbConfiguration
    {
        public NavisionDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
        }
    }
}
