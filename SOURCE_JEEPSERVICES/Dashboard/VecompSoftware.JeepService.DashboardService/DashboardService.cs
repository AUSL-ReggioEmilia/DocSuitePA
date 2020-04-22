using System;
using System.Configuration;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceProcess;
using VecompSoftware.GenericMailSender;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;
using Vecompsoftware.FileServer.Services;

namespace JeepService.JeepService.DashboardService
{
    public partial class DashboardService : ServiceBase
    {
        static ServiceHost _host;
        static FileRepositoryService _service;

        public DashboardService()
        {
            InitializeComponent();
        }

        public void Debug()
        {
            OnStart(null);
        }

        private const string LoggerName = "JeepService.DashboardService";

        public void DoWork(string path)
        { }

        protected override void OnStart(string[] args)
        {
            FileLogger.Info(LoggerName, "Avvio servizio DashboardService");
            var a = new MessageEventArgs("Avvio servizio DashboardService");
            SendMessage(LoggerName, a);
            Setup();
        }

        private static void Setup()
        {
            _service = new FileRepositoryService();

            _service.FileRequested += ServiceFileRequested;
            _service.FileUploaded += ServiceFileUploaded;
            _service.FileDeleted += ServiceFileDeleted;

            _host = new ServiceHost(_service);
            _host.Faulted += HostFaulted;

            try
            {
                _host.Open();
                var channelListener = _host.ChannelDispatchers[0].Listener;
                FileLogger.Info(LoggerName,
                                channelListener != null
                                    ? String.Format("Servizio correttamente avviato all'indirizzo {0}",
                                                    channelListener.Uri)
                                    : "Servizio correttamente avviato ma impossibile rilevare l'indirizzo.");
            }
            catch (Exception ex)
            {
                _host.Close();
                FileLogger.Error(LoggerName, "Errore in attivazione servizio Host", ex);
            }
        }

        protected override void OnStop()
        {
            var a = new MessageEventArgs("Stop servizio DashboardService");
            FileLogger.Info(LoggerName, a.Message);
            SendMessage(LoggerName, a);
        }

        #region "Metodi del servizio"
        static void HostFaulted(object sender, EventArgs e)
        {
            var a = new MessageEventArgs("Il servizio WCF ha avuto un errore grave");
            FileLogger.Error(LoggerName, a.Message);
            SendMessage(LoggerName, a);
            _host.Abort();
        }

        static void ServiceFileRequested(object sender, FileEventArgs e)
        {
            var a = new MessageEventArgs(String.Format("Richiesto file {0}", e.VirtualPath));
            FileLogger.Info(LoggerName, a.Message);
        }

        static void ServiceFileUploaded(object sender, FileEventArgs e)
        {
            var a = new MessageEventArgs(String.Format("Caricato file {0}", e.VirtualPath));
            FileLogger.Info(LoggerName, a.Message);
            SendMessage(LoggerName, a);
        }

        static void ServiceFileDeleted(object sender, FileEventArgs e)
        {
            var a = new MessageEventArgs(String.Format("Eliminato file {0}", e.VirtualPath));
            FileLogger.Info(LoggerName, a.Message);
            SendMessage(LoggerName, a);
        }
        #endregion

        /// <summary> Spedisce il messaggio del modulo alla mail indicata e al log. </summary>
        /// <param name="sender"> Nome del modulo. </param>
        /// <param name="args"> Messaggio. </param>
        private static void SendMessage(string sender, MessageEventArgs args)
        {
            FileLogger.Warn(LoggerName, string.Format("MESSAGGIO da modulo {0}: {1}", sender, args.Message));

            // Chiamata ricevuta da un modulo, spedisco l'email
            if (!bool.Parse(ConfigurationManager.AppSettings["NotificationMailEnabled"]))
            {
                FileLogger.Debug(LoggerName, "Messaggio da modulo non inviato,\"NotificationMailEnabled\" disabilitato.");
                return;
            }

            try
            {
                //Istanzio un client
                var client = new MailClient(
                    ConfigurationManager.AppSettings["NotificationMailType"],
                    ConfigurationManager.AppSettings["NotificationMailServer"],
                    !string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationMailServerPort"]) ? int.Parse(ConfigurationManager.AppSettings["NotificationMailServerPort"]) : 25,
                    !String.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationMailServerAuthenticationType"])
                        ? (MailClient.AuthenticationType)Enum.Parse(typeof(MailClient.AuthenticationType), ConfigurationManager.AppSettings["NotificationMailServerAuthenticationType"], true)
                        : MailClient.AuthenticationType.Plain,
                    ConfigurationManager.AppSettings["NotificationMailUserName"],
                    ConfigurationManager.AppSettings["NotificationMailUserPassword"],
                    ConfigurationManager.AppSettings["NotificationMailDomain"]
                    );
                var subject = string.Format("Chiamata da modulo {0} su {1}", sender, ConfigurationManager.AppSettings["Customer"]);
                var body = string.Format("MODULO: {0}{3}CLIENTE: {1}{3}MESSAGGIO: {2}", sender, ConfigurationManager.AppSettings["Customer"], args.Message, Environment.NewLine);
                var message = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["NotificationMailFrom"]),
                    Subject = subject,
                    Body = body
                };
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NotificationMailTo"]))
                {
                    foreach (var to in ConfigurationManager.AppSettings["NotificationMailTo"].Split(';'))
                    {
                        message.To.Add(to);
                    }
                }
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NotificationMailCc"]))
                {
                    foreach (var cc in ConfigurationManager.AppSettings["NotificationMailCc"].Split(';'))
                    {
                        message.CC.Add(cc);
                    }
                }
                client.Send(message);
                FileLogger.Info(LoggerName, "Spedizione CHIAMATA eseguita con successo");
            }
            catch (Exception exc)
            {
                FileLogger.Error(LoggerName, "Errore in fase di invio messaggio", exc);
            }
        }
    }
}
