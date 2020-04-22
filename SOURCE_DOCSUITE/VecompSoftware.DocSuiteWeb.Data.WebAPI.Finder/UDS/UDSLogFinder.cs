using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSLogFinder : BaseWebAPIFinder<UDSLog, UDSLog>
    {
        #region [ Constructor ]
        public UDSLogFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSLogFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
        }
        #endregion


        #region [ Methods]
        public override void ResetDecoration()
        {
        }
        #endregion
    }
}
