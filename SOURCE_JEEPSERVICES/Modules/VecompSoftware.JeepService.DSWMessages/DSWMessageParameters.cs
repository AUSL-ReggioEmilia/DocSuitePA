using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VecompSoftware.GenericMailSender;
using VecompSoftware.JeepService.Common;

namespace VecompSoftware.JeepService
{
    public class DSWMessageParameters : JeepParametersBase
    {
        [Description("Corpo della mail usato nel caso si cerchi di inviare una Mail senza Body")]
        [CategoryAttribute("Valori di default")]
        [Editor(typeof (MultilineStringEditor), typeof (UITypeEditor))]
        [DefaultValue("In allegato i documenti spediti da DocSuiteWeb")]
        public string DefaultBody { get; set; }

        [Description("Oggetto della mail usato nel caso si cerchi di inviare una Mail senza Body")]
        [CategoryAttribute("Valori di default")]
        [Editor(typeof (MultilineStringEditor), typeof (UITypeEditor))]
        [DefaultValue("Invio documenti da DocSuiteWeb")]
        public string DefaultSubject { get; set; }

        [Description(
            "Nome del Server SMTP utilizzato per la spedizione delle EMail oppure indirizzo http del webservice di Microsoft Exchange"
            )]
        [Category("Server")]
        public string Server { get; set; }

        [Description("Definisce il tipo di autenticazione (standard/SSL/TLS) [solo SMTP]")]
        [Category("Server")]
        public MailClient.AuthenticationType AuthenticationType { get; set; }

        [Description("Definisce la porta da utilizzare per connettersi al server (solo SMTP): 25 porta standard, 465 e 587 eventuali porte alternative.")]
        [Category("Server")]
        [DefaultValue(25)]
        public int ServerPort { get; set; }

        [Category("Server")]
        [Description("Nome utilizzato per l'autenticazione sul Server SMTP")]
        public string UserName { get; set; }

        [Category("Server")]
        [Description("Password utilizzata per l'autenticazione sul Server SMTP")]
        public string UserPassword { get; set; }

        [Category("Server")]
        [Description("Dominio dell'utente per l'autenticazione sul Server SMTP")]
        public string UserDomain { get; set; }

        [Category("Server")]
        [Description("Indirizzo e-mail del mittente. Se non attivo verrà usato il mittente della mail in spedizione.")]
        public string Sender { get; set; }

        [DefaultValue(MailClient.MailClientType.Smtp)]
        [Category("Server")]
        [Description("Definisce il tipo di server da utilizzare per inviare le e-mail")]
        public MailClient.MailClientType ServerType { get; set; }

        [DefaultValue(5)]
        [Category("Behaviour")]
        [Description("Numero massimo di tentativi di spedizione prima che il messaggio venga parcheggiato.")]
        public int MaxErrorCount { get; set; }

        [Category("Debug")]
        [DefaultValue("emanuele.fabbri@vecompsoftware.it")]
        [Description("Definisce l'indirizzo da utilizzare per inviare qualunque mail/pec in caso di modalità DEBUG attiva.")]
        public string DebugModeAddress { get; set; }

        [Category("Debug")]
        [DefaultValue(false)]
        [Description("Abilita la modalità debug e pertanto invia soltanto all'indirizzo \"PecOutAddressDebugMode\" allegando l'email originale che sarebbe stata spedita.")]
        public bool DebugModeEnabled { get; set; }

        [Description("Cartella per elaborazione file temporaneri")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [Category("Configuration")]
        public string TempFolder { get; set; }

        [Description("Definisce il numero massimo di elementi da gestire per sessione di invio")]
        [DefaultValue(5)]
        [Category("Behaviour")]
        public int MaxMailsForSession { get; set; }

        [Description("Definisce il numero di secondi da attendere tra un invio e l'altro (per non sovraccaricare i server)")]
        [DefaultValue(5)]
        [Category("Behaviour")]
        public int SleepBetweenSends { get; set; }

        [DefaultValue(true)]
        [Category("Behaviour")]
        [Description("Indica se notificare il mancato invio anche al mittente dello stesso messaggio.")]
        public bool NotifySender { get; set; }
    }

}

