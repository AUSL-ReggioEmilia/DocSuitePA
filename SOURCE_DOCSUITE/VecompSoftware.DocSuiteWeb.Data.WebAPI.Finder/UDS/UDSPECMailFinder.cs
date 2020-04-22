using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSPECMailFinder : BaseUDSRelationFinder<UDSPECMail, UDSPECMail>
    {
        #region [ Constructor ]
        public UDSPECMailFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSPECMailFinder(IReadOnlyCollection<TenantModel> tenant)
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
