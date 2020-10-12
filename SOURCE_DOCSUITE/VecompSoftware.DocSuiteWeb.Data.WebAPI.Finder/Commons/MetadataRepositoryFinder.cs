using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class MetadataRepositoryFinder : BaseWebAPIFinder<MetadataRepository, MetadataRepositoryModel>
    {
        #region [ Properties ]
        public string MetadataKeyName { get; set; }
        #endregion

        #region [ Constructor ]
        public MetadataRepositoryFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }
        public MetadataRepositoryFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if(!string.IsNullOrEmpty(MetadataKeyName))
            {
                odataQuery = odataQuery.Filter($"contains(JsonMetadata, '\"KeyName\":\"{MetadataKeyName}\"')");
            }
            return base.DecorateFinder(odataQuery);
        }
        public override void ResetDecoration()
        {
            MetadataKeyName = null;
        }
        #endregion
    }
}
