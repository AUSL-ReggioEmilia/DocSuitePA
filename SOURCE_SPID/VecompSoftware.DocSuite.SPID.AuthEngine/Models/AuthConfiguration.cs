using VecompSoftware.DocSuite.SPID.Model.IDP;

namespace VecompSoftware.DocSuite.SPID.AuthEngine.Models
{
    public class AuthConfiguration
    {
        public const string CONFIGURATION_SECTION_NAME = "AuthConfiguration";
        public int IdpAuthLevel { get; set; }
        public string SPDomain { get; set; }
        public int AssertionConsumerServiceIndex { get; set; }
        public int? AttributeConsumingServiceIndex { get; set; }
        public string CertificateThumbprint { get; set; }
        public string CertificatePassword { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePrivateKey { get; set; }
        public bool CertificateFromFile { get; set; }
        public string LoginPath { get; set; }
        public string ACSCallback { get; set; }
        public string LogoutCallback { get; set; }
        public IdpType IdpType { get; set; }
    }
}
