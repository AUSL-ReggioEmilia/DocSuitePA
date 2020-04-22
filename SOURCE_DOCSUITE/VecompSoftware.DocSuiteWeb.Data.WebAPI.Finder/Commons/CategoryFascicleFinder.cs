using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class CategoryFascicleFinder : BaseWebAPIFinder<CategoryFascicle, CategoryFascicle>
    {
        #region [ Properties ]
        public int? Environment { get; set; }
        public int? IdCategory { get; set; }
        public bool? ParentCategoryFascicle { get; set; }        
        public bool? ExpandProperties { get; set; }
        #endregion

        #region [ Constructor ]  
        public CategoryFascicleFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public CategoryFascicleFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override void ResetDecoration()
        {
            Environment = null;
            IdCategory = null;
            ParentCategoryFascicle = null;
            ExpandProperties = null;
            UniqueId = null;
        }

        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandProperties.HasValue && ExpandProperties.Value)
            {
                odataQuery = odataQuery.Expand("Manager")
                                       .Expand("FasciclePeriod")
                                       .Expand("Category");
            }

            if (ParentCategoryFascicle.HasValue && ParentCategoryFascicle.Value)
            {
                odataQuery = odataQuery.Function(string.Format(CommonDefinition.OData.CategoryFascicleService.FX_GetAuthorizedTemplates, IdCategory, Environment));
                return odataQuery;
            }            

            if (Environment.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("DSWEnvironment eq ", Environment.Value));
            }

            if (IdCategory.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Concat("Category/EntityShortId eq ", IdCategory.Value));
            }

            return odataQuery;
        }
        #endregion
    }
}
