using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.SMS
{
    public class SMSParameters : JeepParametersBase
    {
        [Description("Testo globale SMS")]
        [Category("SMS")]
        [DefaultValue("{0} {1}")]
        public string GlobalSMSText { get; set; }

        [Description("Testo di join vector SMS")]
        [Category("SMS")]
        [DefaultValue("Hai ricevuto nuovi messaggi pec nella casella {0}")]
        public string JoinStringText { get; set; }

        [Description("Numero massimo di caratteri presi nella Casella PEC da inserire nel messaggio SMS")]
        [Category("SMS")]
        [DefaultValue(12)]
        public Int16 TakeMailBoxName { get; set; }

        
        [Description("Specificare il numero internazionale di default del numero")]
        [Category("SMS")]
        [DefaultValue("39")]
        public String InternationalPrefixNumber { get; set; }

    }
}
