using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
{
    public class FascicleLogFinder : BaseWebAPIFinder<FascicleLog, FascicleLog>
    {
        #region [ Constructor ]

        public FascicleLogFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public FascicleLogFinder(IReadOnlyCollection<TenantModel> tenants)
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
