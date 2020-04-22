using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Configurations
{
    public class ERPDbConfiguration : DbConfiguration
    {
        public ERPDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
        }
    }
}
