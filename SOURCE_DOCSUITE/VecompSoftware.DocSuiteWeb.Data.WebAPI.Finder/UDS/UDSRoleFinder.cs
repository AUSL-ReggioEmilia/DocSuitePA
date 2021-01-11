using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSRoleFinder : BaseUDSRelationFinder<UDSRole, UDSRole>
    {
        #region [ Constructor ]
        public UDSRoleFinder(TenantModel tenant)
            :this(new List<TenantModel>() { tenant })
        {
        }

        public UDSRoleFinder(IReadOnlyCollection<TenantModel> tenant)
            : base(tenant)
        {
        }
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        #endregion
    }
}
