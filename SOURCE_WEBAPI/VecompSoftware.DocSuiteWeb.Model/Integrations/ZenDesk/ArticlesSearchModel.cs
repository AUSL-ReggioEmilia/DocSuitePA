using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class ArticlesSearchModel : BaseZenDeskModel
    {
        public ICollection<ArticleSearchModel> Results { get; set; }
    }
}
