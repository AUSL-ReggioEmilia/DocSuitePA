using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.DTO.Messages
{
    public class JobSMSNotification
    {
        public JobSMSNotification() { }

        public String MobileNumber { get; set; }

        public ICollection<SMSReport> Reports { get; set; }
    }
}
