using System;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models
{
    public class ContactManualModel
    {
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string CertifiedMail { get; set; }
        public string FiscalCode { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public ContactManualAddressModel Address { get; set; }

        public string GetFirstname()
        {
            return Description.Split('|')[0];
        }

        public string GetSurname()
        {
            if (Description.Split('|').Length > 1)
            {
                return Description.Split('|')[1];
            }
            return string.Empty;
        }

        public string GetBirthDateFormatted()
        {
            return BirthDate.HasValue ? BirthDate.Value.ToString("yyyyMMdd") : string.Empty;
        }

        public string GetValidAddress()
        {
            return string.IsNullOrEmpty(CertifiedMail) ? EmailAddress : CertifiedMail;
        }
    }
}