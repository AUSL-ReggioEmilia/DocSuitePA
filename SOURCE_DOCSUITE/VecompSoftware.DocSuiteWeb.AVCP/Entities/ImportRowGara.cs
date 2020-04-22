using System;

namespace VecompSoftware.DocSuiteWeb.AVCP.Import
{
    public class ImportRowGara
    {
        public int AnnoProvvedimento { get; set; }
        public string CodiceProvvedimento { get; set; }
        public int NumeroProvvedimento { get; set; }
        public DateTime DataProvvedimento { get; set; }
        public int AnnoRiferimento { get; set; }
        public string CIG { get; set; }
        public string CodiceFiscaleProponente { get; set; }
        public string DenominazioneProponente { get; set; }
        public string Oggetto { get; set; }
        public sceltaContraenteType SceltaContraente { get; set; }
        public string Partecipante { get; set; }
        public string Aggiudicatario { get; set; }
        public DateTime DurataDal { get; set; }
        public DateTime DurataAl { get; set; }
        public decimal ImportoAggiudicazione { get; set; }
        public bool IsValid { get; set; }

        //chiave di aggregazione
        public string DocumentKey { get; set; }
        public DateTime DataAggiornamento { get; set; }
    }
}
