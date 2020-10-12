using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Models
{
    public class ArubaSignModel
    {
        public string DelegatedDomain { get; set; }
        public string DelegatedPassword { get; set; }
        public string DelegatedUser { get; set; }
        public string OTPPassword { get; set; }
        public string OTPAuthType { get; set; }
        public string User { get; set; }
        public string CertificateId { get; set; }
        public string TSAUser { get; set; }
        public string TSAPassword { get; set; }
    }
}