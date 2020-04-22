using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
{
    public class FascicleRoleFinder : BaseWebAPIFinder<FascicleRole, FascicleRole>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]


        public Guid? IdFascicle { get; set; }


        public bool? ExpandProperties { get; set; }

        #endregion

        #region [ Constructor ]

        public FascicleRoleFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {

        }

        public FascicleRoleFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {

        }

        #endregion

        #region [ Methods ]

        public override void ResetDecoration()
        {
            IdFascicle = null;
            ExpandProperties = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties.HasValue && ExpandProperties.Value)
            {
                odataQuery.Expand("Role");
            }

            if (IdFascicle.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Fascicle/UniqueId eq ", IdFascicle.Value));
            }

            return odataQuery;
        }
        #endregion
    }
}
