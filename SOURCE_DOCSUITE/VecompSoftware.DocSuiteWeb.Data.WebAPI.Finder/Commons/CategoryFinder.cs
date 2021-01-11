using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class CategoryFinder : BaseWebAPIFinder<Category, Category>
    {
        #region [ Properties ]
        public int? IdCategory { get; set; }
        #endregion

        #region [ Constructor ]  
        public CategoryFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public CategoryFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            UniqueId = null;
            IdCategory = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if(IdCategory.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("EntityShortId eq ", IdCategory.Value));
            }

            return odataQuery;
        }
        #endregion
    }
}
