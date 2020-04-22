using System;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class CategoryModel
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public string Description { get; set; }
        public string HtmlUrl { get; set; }
        public long Id { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public bool? Outdated { get; set; }
        public int? Position { get; set; }
        public string SourceLocale { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Url { get; set; }
    }
}
