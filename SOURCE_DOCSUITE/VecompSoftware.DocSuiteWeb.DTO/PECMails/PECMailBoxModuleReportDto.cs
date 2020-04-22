using System;

namespace VecompSoftware.DocSuiteWeb.DTO.PECMails
{
    public class PECMailBoxModuleReportDto
    {
        #region [ Constructor ]
        public PECMailBoxModuleReportDto() { }
        #endregion

        #region [ Properties ]
        public String ModuleName { get; set; }

        public String MailBoxName { get; set; }

        public DateTime? LastJSProcessed { get; set; }

        public Double EvalTime { get; set; }

        public String Result { get; set; }

        #endregion
    }
}
