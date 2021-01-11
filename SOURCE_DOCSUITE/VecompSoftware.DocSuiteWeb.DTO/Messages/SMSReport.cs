using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Messages
{
    public class SMSReport
    {
        public SMSReport() { }

        public String MailBoxName { get; set; }

        public Guid SMSNotificationId { get; set; }

        public Int32 PECMailId { get; set; }
    }
}
