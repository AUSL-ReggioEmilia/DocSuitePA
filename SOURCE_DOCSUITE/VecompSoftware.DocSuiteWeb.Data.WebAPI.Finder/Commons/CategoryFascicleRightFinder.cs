using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
{
    public class CategoryFascicleRightFinder : BaseWebAPIFinder<CategoryFascicleRight, CategoryFascicleRight>
    {
        #region [ Properties ]
        public int IdCategoryFascicle { get; set; }
        public int? IdRole { get; set; }
        public int? IdContainer { get; set; }
        #endregion

        #region [ Constructor ]  
        public CategoryFascicleRightFinder(TenantModel tenant)
            : this(new List<TenantModel>() { tenant })
        {
        }

        public CategoryFascicleRightFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }

        #endregion

        #region [ Methods ]  
        public override void ResetDecoration()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}