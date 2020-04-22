namespace VecompSoftware.StampaConforme.Models.ConversionParameters
{
    public class ConversionSecureParameter
    {
        public string User { get; set; }
        public string Password { get; set; }
        public bool AllowPrinting { get; set; }
        public bool AllowModifyContents { get; set; }
        public bool AllowCopy { get; set; }
        public bool AllowModifyAnnotations { get; set; }
        public bool AllowFillIn { get; set; }
        public bool AllowScreenReaders { get; set; }
        public bool AllowAssembly { get; set; }
        public bool AllowDegradedPrinting { get; set; }
    }
}
