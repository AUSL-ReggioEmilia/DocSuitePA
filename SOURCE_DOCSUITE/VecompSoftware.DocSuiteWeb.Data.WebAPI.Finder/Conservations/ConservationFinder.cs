using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Conservations
{
    public class ConservationFinder : BaseWebAPIFinder<Conservation, ConservationModel>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public ConservationFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public ConservationFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
        }
        #endregion
    }
}
