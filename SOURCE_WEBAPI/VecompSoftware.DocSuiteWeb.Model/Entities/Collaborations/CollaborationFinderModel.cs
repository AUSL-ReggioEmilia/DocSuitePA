using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationFinderModel
    {
        public bool? IsRequired { get; set; }
        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public int? EntityId { get; set; }
        public string DocumentType { get; set; }
        public DateTimeOffset? MemorandumDate { get; set; }
        public string Object { get; set; }
        public string Note { get; set; }
        public string RegistrationName { get; set; }
        public int Skip { get; set; }
        public int Top { get; set; }
    }
}
