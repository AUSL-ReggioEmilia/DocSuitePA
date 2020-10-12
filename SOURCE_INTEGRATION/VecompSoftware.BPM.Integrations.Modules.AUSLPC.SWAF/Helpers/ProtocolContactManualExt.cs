using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Helpers
{
    public static class ProtocolContactManualExt
    {
        public static string GetFirstname(this ProtocolContactManual contactManual)
        {
            return contactManual.Description.Split('|')[0];
        }

        public static string GetSurname(this ProtocolContactManual contactManual)
        {
            if (contactManual.Description.Split('|').Length > 1)
            {
                return contactManual.Description.Split('|')[1];
            }
            return string.Empty;
        }

        public static string GetBirthDateFormatted(this ProtocolContactManual contactManual)
        {
            return contactManual.BirthDate.HasValue ? contactManual.BirthDate.Value.ToString("yyyyMMdd") : string.Empty;
        }

        public static string GetValidAddress(this ProtocolContactManual contactManual)
        {
            return string.IsNullOrEmpty(contactManual.CertifydMail) ? contactManual.EMailAddress : contactManual.CertifydMail;
        }
    }
}
