using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Protocols
{
    public class ProtocolLogFinder : BaseWebAPIFinder<ProtocolLog, ProtocolLog>
    {
        #region [ Constructor ]

        public ProtocolLogFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public ProtocolLogFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {

        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            
        }
        #endregion

    }
}
