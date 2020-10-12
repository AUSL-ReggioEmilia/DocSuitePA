using VecompSoftware.BPM.Integrations.Services.SignServices.Models;

namespace VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService.Models
{
    public class ArubaSignModel : ISignerParameter
    {
        public string ServiceName { get; set; }
        public string DelegatedDomain { get; set; }
        public string DelegatedPassword { get; set; }
        public string DelegatedUser { get; set; }
        public string OTPPassword { get; set; }
        public string OTPAuthType { get; set; }
        public string User { get; set; }
        public string CertificateId { get; set; }
    }
}
