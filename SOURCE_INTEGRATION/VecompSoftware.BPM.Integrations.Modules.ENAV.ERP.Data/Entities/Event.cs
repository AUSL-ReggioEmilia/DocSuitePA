using System;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Entities
{
    public class Event
    {
        public decimal EventId { get; set; }
        public decimal? CorrelationId { get; set; }
        public decimal? Year { get; set; }
        public decimal? Number { get; set; }
        public string Subject { get; set; }
        public string Verb { get; set; }
        public string DigitalSigners { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime? ProcessedTime { get; set; }
    }
}
