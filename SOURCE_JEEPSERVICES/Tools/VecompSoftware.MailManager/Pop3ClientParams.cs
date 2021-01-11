using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VecompSoftware.MailManager
{
    public class Pop3ClientParams : MailClientParams
    {
        public Func<bool> UserCanceled { get; set; }
        public Action<string> LogActionHandler { get; set; }
    }
}
