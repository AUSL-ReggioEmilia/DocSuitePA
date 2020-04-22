using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ENavigare.Data.Configurations
{
    public class ENavigareDbConfiguration : DbConfiguration
    {
        public ENavigareDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));
        }
    }
}
