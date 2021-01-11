using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class RoleUserFinder : BaseWebAPIFinder<RoleUser, RoleUser>
    {
        #region [ Constructor ]  
        public RoleUserFinder(TenantModel tenant)
            :this(new List<TenantModel>() { tenant })
        {
        }

        public RoleUserFinder(IReadOnlyCollection<TenantModel> tenants)
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
