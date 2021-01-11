using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Resolutions
{
    public class ResolutionKindTypeResults
    {
        public Guid IdResolutionKindDocumentSeries { get; set; }
        public Guid IdResolutionKind { get; set; }
        public string Name { get; set; } 
        public bool IsRequired { get; set; }
        public int DocumentSeriesId { get; set; }
        public string DocumentSeriesName { get; set; }
        public int ResolutionKindDocumentSeriesIsActive { get; set; }
        public int ResolutionKindIsActive { get; set; }
    }
}
