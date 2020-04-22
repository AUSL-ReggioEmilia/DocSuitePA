using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles
{
    public class FascicleFinderModel
    {
        public FascicleFinderModel()
        {
            Roles = new List<int>();
        }

        public int Skip { get; set; }
        public int Top { get; set; }
        public bool ApplySecurity { get; set; }
        public short? Year { get; set; }
        public DateTimeOffset? StartDateFrom { get; set; }
        public DateTimeOffset? StartDateTo { get; set; }
        public DateTimeOffset? EndDateFrom { get; set; }
        public DateTimeOffset? EndDateTo { get; set; }
        public int? FascicleStatus { get; set; }
        public string Manager { get; set; }
        public string Name { get; set; }
        public bool? ViewConfidential { get; set; }
        public bool? ViewAccessible { get; set; }
        public string Subject { get; set; }
        public int? SubjectSearchStrategy { get; set; }
        public string Note { get; set; }
        public string Rack { get; set; }
        public Guid? IdMetadataRepository { get; set; }
        public string MetadataValue { get; set; }
        public string Classifications { get; set; }
        public bool? IncludeChildClassifications { get; set; }
        public ICollection<int> Roles { get; set; }
        public short? Container { get; set; }
    }
}
