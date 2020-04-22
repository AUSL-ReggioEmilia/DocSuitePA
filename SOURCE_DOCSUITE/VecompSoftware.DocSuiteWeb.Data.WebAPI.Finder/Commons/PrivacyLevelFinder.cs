using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class PrivacyLevelFinder : BaseWebAPIFinder<PrivacyLevel, PrivacyLevel>
    {
        #region [ Properties ]
        public int? MinimumLevel { get; set; }
        public int? MaximumLevel { get; set; }
        public int? Level { get; set; }
        #endregion

        #region [ Constructor ]
        public PrivacyLevelFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public PrivacyLevelFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]

        public override void ResetDecoration()
        {
            MinimumLevel = null;
            MaximumLevel = null;
            Level = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {

            if (MinimumLevel.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Level ge ", MinimumLevel));
                odataQuery = odataQuery.Sorting("Level");
            }
            if (MaximumLevel.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Level le ", MaximumLevel));
            }
            if (Level.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Level eq ", Level));
            }
            return base.DecorateFinder(odataQuery);
        }
        #endregion
    }
}
