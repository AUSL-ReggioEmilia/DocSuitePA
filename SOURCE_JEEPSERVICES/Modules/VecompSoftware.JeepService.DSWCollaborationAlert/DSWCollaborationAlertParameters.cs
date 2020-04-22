using System.ComponentModel;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService.DSWCollaborationAlert
{
    public class DSWCollaborationAlertParameters : JeepParametersBase
    {
        [Category("Opzioni")]
        [DefaultValue(false)]
        [Description("Indica se la mail di avviso deve essere inviata anche al proponente della collaborazione.")]
        [DisplayName("Invia a proponente")]
        public bool SendToProposer { get; set; }

        [Category("Opzioni")]
        [DefaultValue(false)]
        [Description("Indica se la mail di avviso deve essere inviata anche alle segreterie della collaborazione.")]
        [DisplayName("Invia a segreterie")]
        public bool SendToSecretaries { get; set; }

        [Category("Opzioni")]
        [DefaultValue(false)]
        [Description("Indica se generare una mail di notifica per ogni destinatario della collaborazione. Di default invia una unica mail per tutti i destinatari.")]
        [DisplayName("Crea una mail per ogni destinatario")]
        public bool CreateEmailForEachRecipient { get; set; }

        [Category("Templates")]
        [Description("Indica il percorso del file template (htm) utilizzato per l'invio della mail per le collaborazioni non ancora scadute ma con Data Avviso inferiore ad oggi.")]
        [DisplayName("Path Warning template")]
        public string PathWarningMailTemplate { get; set; }

        [Category("Templates")]
        [Description("Indica il percorso del file template (htm) utilizzato per l'invio della mail per le collaborazioni scadute.")]
        [DisplayName("Path Expired template")]
        public string PathExpiredMailTemplate { get; set; }

        [Category("Valori di default della mail")]
        [DefaultValue("Notifica scadenza collaborazione")]
        [Description("Indica l'oggetto da utilizzare per l'invio delle mail.")]
        [DisplayName("Oggetto della mail")]
        public string MailSubject { get; set; }

        [Category("Valori di default della mail")]
        [Description("Indica il mittente da utilizzare per l'invio delle mail.")]
        [DisplayName("Mittente della mail")]
        public string MailSender { get; set; }

        [Category("Opzioni")]
        [Description("Indirizzo della pubblicazione DSW.")]
        [DisplayName("Indirizzo DSW")]
        public string DswPath { get; set; }
    }
}
