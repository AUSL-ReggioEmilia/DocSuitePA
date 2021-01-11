using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.MassimariScarto
{
    public class MassimarioScartoFinder : BaseWebAPIFinder<MassimarioScarto, MassimarioScartoModel>
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public string Path { get; set; }
        #endregion

        #region [ Constructor ]

        public MassimarioScartoFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public MassimarioScartoFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            UniqueId = null;
            Path = string.Empty;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                odataQuery = odataQuery.Filter($"MassimarioScartoPath eq '{Path}'");
            }

            return base.DecorateFinder(odataQuery);
        }
        #endregion
    }
}
