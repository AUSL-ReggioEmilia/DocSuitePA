using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class CategoriesModel : BaseZenDeskModel
    {
        public ICollection<CategoryModel> Categories { get; set; }
    }
}
