using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
{
    public class UDSTypologyFinder : BaseWebAPIFinder<UDSTypology, UDSTypology>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public UDSTypologyStatus? Status { get; set; }
        #endregion

        #region [ Constructor ]

        public UDSTypologyFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public UDSTypologyFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            odataQuery = odataQuery.Expand("UDSRepositories");

            if (Status.HasValue && Status != UDSTypologyStatus.Invalid)
            {
                odataQuery = odataQuery.Filter(string.Concat("Status eq VecompSoftware.DocSuiteWeb.Entity.UDS.UDSTypologyStatus'", (short)Status.Value, "'"));
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            Status = null;
        }

        #endregion
    }
}
