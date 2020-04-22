using System;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class SectionModel
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public long? CategoryId { get; set; }
        public int? Position { get; set; }
        public string Sorting { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Locale { get; set; }
        public string SourceLocale { get; set; }
        public bool? Outdated { get; set; }
        public long? ParentSectionId { get; set; }
        public string ThemeTemplate { get; set; }
    }
}
