using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSContactFinder : BaseUDSRelationFinder<UDSContact, UDSContact>
    {
        #region [ Constructor ]
        public UDSContactFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSContactFinder(IReadOnlyCollection<TenantModel> tenant)
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
