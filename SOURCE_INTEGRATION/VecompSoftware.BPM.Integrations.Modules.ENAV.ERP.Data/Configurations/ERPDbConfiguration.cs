using Oracle.ManagedDataAccess.EntityFramework;
using System.Data.Entity;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Configurations
{
    public class ERPDbConfiguration : DbConfiguration
    {
        public ERPDbConfiguration()
        {
            SetProviderServices("Oracle.ManagedDataAccess.Client", EFOracleProviderServices.Instance);
        }
    }
}
