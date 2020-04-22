using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class ArticlesModel : BaseZenDeskModel
    {
        public ICollection<ArticleModel> Articles { get; set; }
    }
}
