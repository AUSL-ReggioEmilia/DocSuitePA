
using VecompSoftware.Sign.Models;

namespace VecompSoftware.Sign.ArubaSignService
{
    public class ArubaSignModel
    {
        public string ServiceName { get; set; }
        public string DelegatedDomain { get; set; }
        public string DelegatedPassword { get; set; }
        public string DelegatedUser { get; set; }
        public string OTPPassword { get; set; }
        public string OTPAuthType { get; set; }
        public string User { get; set; }
        public string UserPassword { get; set; }
        public string CertificateId { get; set; }
        public string TSAUser { get; set; }
        public string TSAPassword { get; set; }
        public SignRequestType RequestType { get; set; }
    }
}
