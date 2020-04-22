using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Resolutions
{
    public class ResolutionChangeDSI
    {
        public int DocumentSeriesId { get; set; }
        public string DocumentSeriesName { get; set; }
        public int? DSItemId { get; set; }
        public int? ResolutionDSItemId { get; set; }
        public DateTimeOffset DSIRegistrationDate { get; set; }

    }
}
