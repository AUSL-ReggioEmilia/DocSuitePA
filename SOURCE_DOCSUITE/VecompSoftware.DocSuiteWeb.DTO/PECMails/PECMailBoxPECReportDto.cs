using System;

namespace VecompSoftware.DocSuiteWeb.DTO.PECMails
{
    public class PECMailBoxPECReportDto
    {
        #region [ Constructor ]
        public PECMailBoxPECReportDto() { }
        #endregion

        #region [ Properties ]

        public String MailBoxName { get; set; }

        public DateTime? LastJSProcessed { get; set; }

        public DateTime? LastJSErrorProcessed { get; set; }

        public Double PECInDropCount { get; set; }

        public Double PECErrorCount { get; set; }

        #endregion
    }
}
