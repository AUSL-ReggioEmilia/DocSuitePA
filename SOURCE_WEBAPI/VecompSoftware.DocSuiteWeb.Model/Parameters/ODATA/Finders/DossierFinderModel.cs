using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class DossierFinderModel
    {
        public DossierFinderModel()
        {
            MetadataValues = new List<MetadataFinderModel>();
        }

        public int Skip { get; set; } 
        public int Top { get; set; }
        public short? Year { get; set; }
        public int? Number { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public short? IdContainer { get; set; }
        public DateTimeOffset? StartDateFrom { get; set; }
        public DateTimeOffset? StartDateTo { get; set; }
        public DateTimeOffset? EndDateFrom { get; set; }
        public DateTimeOffset? EndDateTo { get; set; }
        public Guid? IdMetadataRepository { get; set; }
        public string MetadataValue { get; set; }
        public ICollection<MetadataFinderModel> MetadataValues { get; set; }
        public short? IdCategory { get; set; }
        public DossierType? DossierType { get; set; }
        public DossierStatus? Status { get; set; }
    }
}
