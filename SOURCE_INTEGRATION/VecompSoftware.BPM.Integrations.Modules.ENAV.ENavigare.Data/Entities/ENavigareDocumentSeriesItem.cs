using System;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ENavigare.Data.Entities
{
    public class ENavigareDocumentSeriesItem
    {
        public Guid UniqueId { get; set; }
        public DateTime? DataValiditaScheda { get; set; }
        public DateTimeOffset? DataUltimoAggiornamento { get; set; }
        public DateTime? DataPubblicazione { get; set; }
        public DateTime? DataRitiro { get; set; }
        public string Oggetto { get; set; }
        public string Abstract { get; set; }
        public string Codice { get; set; }
        public string ProceduraNomiFile { get; set; }
        public string ProceduraPosizioni { get; set; }
        public string LineeGuidaNomiFile { get; set; }
        public string LineeGuidaPosizioni { get; set; }
        public string ModulisticaNomiFile { get; set; }
        public string ModulusticaPosizioni { get; set; }
        public string Url { get; set; }
        public bool? InEvidenza { get; set; }

    }
}
