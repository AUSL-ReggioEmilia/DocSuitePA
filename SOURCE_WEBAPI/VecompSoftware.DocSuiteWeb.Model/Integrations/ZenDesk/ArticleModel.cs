using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Integrations.ZenDesk
{
    public class ArticleModel
    {
        public ArticleModel()
        {
            LabelNames = new HashSet<string>();
            OutdatedLocales = new HashSet<string>();
        }

        public long? AuthorId { get; set; }
        public string Body { get; set; }
        public bool? CommentsDisabled { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public bool? Draft { get; set; }
        public DateTimeOffset? EditedAt { get; set; }
        public string HtmlUrl { get; set; }
        public long Id { get; set; }
        public ICollection<string> LabelNames { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
        public bool? Outdated { get; set; }
        public ICollection<string> OutdatedLocales { get; set; }
        public long? PermissionGroupId { get; set; }
        public int? Position { get; set; }
        public bool? Promoted { get; set; }
        public long? SectionId { get; set; }
        public string SourceLocale { get; set; }
        public string Title { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string Url { get; set; }
        public long? UserSegmentId { get; set; }
        public int? VoteCount { get; set; }
        public int? VoteSum { get; set; }
    }
}
