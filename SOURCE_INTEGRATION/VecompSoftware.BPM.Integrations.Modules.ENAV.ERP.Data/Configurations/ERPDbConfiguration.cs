using Oracle.ManagedDataAccess.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Configurations
{
    public class ERPDbConfiguration : DbConfiguration
    {
        public ERPDbConfiguration()
        {
#if DEBUG
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
#else
            SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
#endif
        }
    }
}
