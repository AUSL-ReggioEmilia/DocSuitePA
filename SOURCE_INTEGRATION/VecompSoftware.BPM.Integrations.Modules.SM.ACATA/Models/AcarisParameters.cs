namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models
{
    public class AcarisParameters
    {
        public long IdAOO { get; set; }
        public long IdNode { get; set; }
        public string AppKey { get; set; }
        public long IdStructure { get; set; }
        public string FiscalCode { get; set; }
        public string InstitutionCode { get; set; }
        public string InstitutionDescription { get; set; }
        public string DestinatarioInternoCodiceNodo { get; set; }
        public string DestinatarioInternoDescrizioneNodo { get; set; }
        public string RepositoryName => $"{InstitutionCode} {InstitutionDescription}";

    }
}
