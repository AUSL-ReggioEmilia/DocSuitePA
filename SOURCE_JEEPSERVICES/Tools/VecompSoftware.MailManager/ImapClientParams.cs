using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.MailManager
{
    public class ImapClientParams : MailClientParams
    {
        public string ImapFolder { get; set; }
        public ImapFlag ImapSearchFlag { get; set; }
        public DateTime? ImapStartDate { get; set; }
        public DateTime? ImapEndDate { get; set; }

        public Func<bool> UserCanceled { get; set; }
        public Action<string> LogActionHandler { get; set; }
    }
}
