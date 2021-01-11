using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Configuration;
using VecompSoftware.DocSuiteWeb.API;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.DSWCollaborationAlert
{
    public class DSWCollaborationAlertModule : JeepModuleBase<DSWCollaborationAlertParameters>
    {
        #region Fields

        private Lazy<CollaborationConnector> _connector;
        private Lazy<MailConnector> _mailConnector;
        #endregion

        #region Properties

        private CollaborationConnector Connector
        {
            get { return _connector.Value; }
        }

        private MailConnector MailConnector
        {
            get { return _mailConnector.Value; }
        }

        private string CollaborationConnectorEndpoint
        {
            get
            {
                var section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
                foreach (ChannelEndpointElement endpoint in section.Endpoints.Cast<ChannelEndpointElement>().Where(endpoint => endpoint.Name.Eq("VecompSoftware.DocSuiteWeb.API.CollaborationService")))
                {
                    return endpoint.Address.AbsoluteUri.Remove(endpoint.Address.AbsoluteUri.Length - endpoint.Address.Segments.Last().Length);
                }
                return string.Empty;
            }
        }

        private string MailConnectorEndpoint
        {
            get
            {
                var section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
                foreach (ChannelEndpointElement endpoint in section.Endpoints.Cast<ChannelEndpointElement>().Where(endpoint => endpoint.Name.Eq("VecompSoftware.DocSuiteWeb.API.MailService")))
                {
                    return endpoint.Address.AbsoluteUri.Remove(endpoint.Address.AbsoluteUri.Length - endpoint.Address.Segments.Last().Length);
                }
                return string.Empty;
            }
        }
        #endregion

        #region Methods
        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);

            CheckFileTemplate(Parameters.PathExpiredMailTemplate);
            CheckFileTemplate(Parameters.PathWarningMailTemplate);

            this._connector = new Lazy<CollaborationConnector>(() => new CollaborationConnector(this.CollaborationConnectorEndpoint));
            this._mailConnector = new Lazy<MailConnector>(() => new MailConnector(this.MailConnectorEndpoint));
        }

        private void CheckFileTemplate(string pathTemplate)
        {
            if (!File.Exists(pathTemplate))
            {
                FileLogger.Error(Name, string.Format("Errore in [Initialize] - Il file al percorso {0} non esiste.", pathTemplate));
                throw new FileNotFoundException();
            }

            FileInfo fi = new FileInfo(pathTemplate);
            if (!fi.Extension.Eq(".htm"))
            {
                FileLogger.Error(Name, string.Format("Errore in [Initialize] - Il file al percorso {0} non corrisponde ad un file di template valido.", pathTemplate));
                throw new Exception(string.Format("Errore in [Initialize] - Il file al percorso {0} non corrisponde ad un file di template valido.", pathTemplate));
            }
        }

        private bool CancelRequest()
        {
            return Cancel;
        }

        public override void SingleWork()
        {
            FileLogger.Info(Name, "Inizio procedura di invio messaggi di notifica.");
            CheckWarningCollaboration();
            CheckExpiredCollaboration();
            FileLogger.Info(Name, "Fine procedura di invio messaggi di notifica.");
        }

        private void CheckWarningCollaboration()
        {
            FileLogger.Info(Name, "[CheckWarningCollaboration]. Inizio verifica collaborazioni da avvisare.");
            var response = Connector.GetCollaborationsToAlert(false);
            if (response.Exception != null)
            {
                FileLogger.Error(Name, "Errore in [CheckWarningCollaboration] - E' avvenuto un errore nel recupero delle collaborazioni");
                SendMessage(String.Format("Errore in [CheckWarningCollaboration]. \nErrore: {0} \nStacktrace: {1}", response.Exception.Message, response.Exception.StackTrace));
                return;
            }

            var listCollaboration = response.Argument;
            if (!listCollaboration.Any())
            {
                FileLogger.Info(Name, "Nessuna collaborazione trovata per l'invio.");
                return;
            }

            var warningTemplate = File.ReadAllText(Parameters.PathWarningMailTemplate)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty);

            FileLogger.Info(Name, String.Format("[CheckWarningCollaboration]. Trovate {0} collaborazioni da processare.", listCollaboration.Count()));
            foreach (var item in listCollaboration)
            {
                if (CancelRequest())
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                FileLogger.Debug(Name, String.Format("[CheckWarningCollaboration]. Processo la collaborazione {0}.", item.Id));
                var mailDetail = new MailTemplateDetails(item, Parameters);

                var mailBody = string.Format(new MailTemplateFormatter(), warningTemplate, mailDetail);
                try
                {
                    SendMail(mailBody, item);
                    FileLogger.Debug(Name, String.Format("[CheckWarningCollaboration]. Collaborazione {0} spedita correttamente.", item.Id));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(Name, String.Format("Errore in [CheckWarningCollaboration] - E' avvenuto un errore nella spedizione del messaggio per la collaborazione con ID {0}", item.Id), ex);
                    SendMessage(String.Format("Errore in [CheckWarningCollaboration]. \nErrore: {0} \nStacktrace: {1}", ex.Message, ex.StackTrace));
                }
            }
            FileLogger.Info(Name, "[CheckWarningCollaboration]. Fine procedura di notifica.");
        }

        private void CheckExpiredCollaboration()
        {
            FileLogger.Info(Name, "[CheckExpiredCollaboration]. Inizio verifica collaborazioni scadute.");
            var response = Connector.GetCollaborationsToAlert(true);
            if (response.Exception != null)
            {
                FileLogger.Error(Name, "Errore in [CheckExpiredCollaboration] - E' avvenuto un errore nel recupero delle collaborazioni");
                SendMessage(String.Format("Errore in [CheckExpiredCollaboration]. \nErrore: {0} \nStacktrace: {1}", response.Exception.Message, response.Exception.StackTrace));
                return;
            }

            var listCollaboration = response.Argument;
            if (!listCollaboration.Any())
            {
                FileLogger.Info(Name, "Nessuna collaborazione trovata per l'invio.");
                return;
            }

            var expiredTemplate = File.ReadAllText(Parameters.PathExpiredMailTemplate)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty);

            FileLogger.Info(Name, String.Format("[CheckExpiredCollaboration]. Trovate {0} collaborazioni da processare.", listCollaboration.Count()));
            foreach (var item in listCollaboration)
            {
                if (CancelRequest())
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                var mailDetail = new MailTemplateDetails(item, Parameters);

                var mailBody = string.Format(new MailTemplateFormatter(), expiredTemplate, mailDetail);
                try
                {
                    SendMail(mailBody, item);
                    FileLogger.Debug(Name, String.Format("[CheckExpiredCollaboration]. Collaborazione {0} spedita correttamente.", item.Id));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(Name, String.Format("Errore in [CheckExpiredCollaboration] - E' avvenuto un errore nella spedizione del messaggio per la collaborazione con ID {0}", item.Id), ex);
                    SendMessage(String.Format("Errore in [CheckExpiredCollaboration]. \nErrore: {0} \nStacktrace: {1}", ex.Message, ex.StackTrace));
                }
            }
            FileLogger.Info(Name, "[CheckExpiredCollaboration]. Fine procedura di notifica.");
        }

        private IEnumerable<IContactDTO> GetRecipients(ICollaborationDTO item)
        {
            var list = new List<IContactDTO>();

            list.Add(item.Signer);
            if (Parameters.SendToProposer)
            {
                if (item.Proposer != null)
                {
                    if (!list.Any(x => x.EmailAddress.Eq(item.Proposer.EmailAddress)))
                        list.Add(item.Proposer);
                }
            }
            if (!Parameters.SendToSecretaries || item.Secretaries == null) return list;
            foreach (var s in item.Secretaries.Where(s => !list.Any(x => x.EmailAddress.Eq(s.EmailAddress))))
            {
                list.Add(s);
            }

            return list;
        }

        private void SendMail(string body, ICollaborationDTO item)
        {
            var dto = new MailDTO();
            dto.Mailbox = new MailboxDTO { TypeName = "DSWMessage" };
            dto.Sender = new ContactDTO(Parameters.MailSender);
            dto.Body = body;
            dto.Subject = Parameters.MailSubject;

            if (Parameters.CreateEmailForEachRecipient)
            {
                var recipients = GetRecipients(item);
                foreach (var recipient in recipients)
                {
                    Array.Clear(dto.Recipients, 0, dto.Recipients.Length);
                    dto.AddRecipient(recipient);
                    MailConnector.Send(dto);
                }
            }
            else
            {
                var recipients = GetRecipients(item);
                dto.AddRecipients(recipients);
                MailConnector.Send(dto);
            }
        }
        #endregion
    }
}
