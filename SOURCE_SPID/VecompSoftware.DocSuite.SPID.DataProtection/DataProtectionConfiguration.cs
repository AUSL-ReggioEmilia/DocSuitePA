using VecompSoftware.DocSuite.SPID.Model.Common;

namespace VecompSoftware.DocSuite.SPID.DataProtection
{
    public class DataProtectionConfiguration
    {
        public string SecretKeyStoragePath { get; set; }
        public TokenKeysProtectionType ProtectionType { get; set; }
        public string X509CertificateThumbprint { get; set; }
    }
}
