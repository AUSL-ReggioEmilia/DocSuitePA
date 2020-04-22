using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.GenericMailSender;
using VecompSoftware.GenericMailSender.Extensions;
using VecompSoftware.Helpers;
using VecompSoftware.JeepService.Common;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using Attachment = System.Net.Mail.Attachment;

namespace VecompSoftware.JeepService
{
    public class DSWMessages : JeepModuleBase<DSWMessageParameters>
    {
        #region [ Fields ]
        private FacadeFactory _facade;
        private MailClient _mailClient;
        #endregion

        #region [ Properties ]
        private FacadeFactory Facade
        {
            get { return _facade ?? (_facade = new FacadeFactory()); }
        }

        private Dictionary<int, int> _errorCounter;
        private Dictionary<int, int> ErrorCounter
        {
            get { return _errorCounter ?? (_errorCounter = new Dictionary<int, int>()); }
        }

        #endregion

        #region [ Methods ]

        private void OnError(DSWMessage message, Exception ex)
        {
            if (ErrorCounter.ContainsKey(message.Id))
            {
                ErrorCounter[message.Id]++;

                if (ErrorCounter[message.Id] >= Parameters.MaxErrorCount)
                {
                    // Imposto il messaggio a errore
                    message.Status = DSWMessage.MessageStatusEnum.Error;
                    Facade.MessageFacade.Update(ref message);
                    // Salvo il Log e segnalo il problema
                    FileLogger.Error(Name, string.Format("Errore in fase spedizione messaggio [{0}].", message.Id), ex);
                    Facade.MessageLogFacade.InsertLog(message, string.Format("Errore Spedizione: {0}", ex.Message), MessageLog.MessageLogType.Error);

                    StringBuilder s = new StringBuilder();
                    s.AppendFormat("Messaggio [{0}] disattivato per errore in fase spedizione: {1}. Collegarsi per verificare e riattivare il messaggio.", message.Id, ex.Message);

                    foreach (MessageEmail m in message.Emails)
                    {
                        s.AppendFormat("{0}{0}Riferimento Mail Inviata{0} Data: {1}{0} Oggetto: {2}{0} ", Environment.NewLine, DateTime.Now, m.Subject);

                        foreach (MessageContactEmail contact in m.GetRecipients())
                        {
                            s.AppendFormat("Destinatari: {0}", contact.Email);
                        }
                    }

                    MessageEventArgs args = new MessageEventArgs(s.ToString());

                    if (this.Parameters.NotifySender)
                        args.Recipients.Add(message.GetSender().Email);

                    this.SendMessage(args);
                }
            }
            else
            {
                ErrorCounter.Add(message.Id, 1);
            }

            FileLogger.Error(Name, string.Format("Error n. {0} in fase spedizione messaggio [{1}].", ErrorCounter[message.Id], message.Id), ex);
        }

        private void OnSent(DSWMessage message)
        {
            if (ErrorCounter.ContainsKey(message.Id))
            {
                ErrorCounter.Remove(message.Id);
            }

            message.Status = DSWMessage.MessageStatusEnum.Sent;
            Facade.MessageFacade.Update(ref message);
            Facade.MessageLogFacade.InsertLog(message, "Messaggio spedito", MessageLog.MessageLogType.Sent);
        }

        public override void SingleWork()
        {
            IList<DSWMessage> messages = Facade.MessageFacade.GetMessagesToSend(DSWMessage.MessageTypeEnum.Email, DSWMessage.MessageStatusEnum.Active).Take(Parameters.MaxMailsForSession).ToList();
            FileLogger.Info(Name, string.Format("Trovati {0} messaggi da spedire.", messages.Count));
            foreach (DSWMessage message in messages)
            {
                if (Cancel)
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                FileLogger.Info(Name, String.Format("Inizio elaborazione messaggio [{0}]", message.Id));
                try
                {
                    MailMessage item = new MailMessage();
                    MailAddress originalSender = null;
                    IList<MessageContact> mittente = Facade.MessageContactFacade.GetByMessage(message, MessageContact.ContactPositionEnum.Sender) ?? new List<MessageContact>();
                    FileLogger.Debug(Name, String.Format("Numero mittenti: {0}", mittente.Count));
                    MessageContactEmail mittenteEmail = null;

                    if (mittente.Any())
                    {
                        mittenteEmail = Facade.MessageContactEmailFacade.GetByContact(mittente.First());
                    }

                    if (mittenteEmail != null)
                    {
                        item.From = new MailAddress(mittenteEmail.Email, mittenteEmail.Description);
                        originalSender = item.From;
                    }

                    FileLogger.Debug(Name, String.Format("Parameters.Sender: {0}", Parameters.Sender));
                    if (!string.IsNullOrEmpty(Parameters.Sender))
                    {
                        item.From = new MailAddress(Parameters.Sender);
                        if (mittenteEmail != null)
                        {
                            item.ReplyToList.Add(new MailAddress(mittenteEmail.Email, mittenteEmail.Description));
                        }
                    }
                    FileLogger.Debug(Name, String.Format("item.From = {0}", item.From));
                    if (item.From == null)
                    {
                        throw new ArgumentNullException("Email sender is not specified");
                    }


                    IList<MessageContact> destinatari = Facade.MessageContactFacade.GetByMessage(message) ?? new List<MessageContact>();
                    FileLogger.Debug(Name, string.Format("Numero destinatari: {0}", destinatari.Count));
                    MessageContactEmail messageContactEmail = null;
                    foreach (MessageContact messageContact in destinatari)
                    {
                        messageContactEmail = Facade.MessageContactEmailFacade.GetByContact(messageContact);
                        if (!string.IsNullOrEmpty(messageContactEmail.Email) && messageContact.ContactPosition != MessageContact.ContactPositionEnum.Sender)
                        {
                            switch (messageContact.ContactPosition)
                            {
                                case MessageContact.ContactPositionEnum.Recipient:
                                    {
                                        item.To.Add(new MailAddress(messageContactEmail.Email, messageContactEmail.Description));
                                        break;
                                    }
                                case MessageContact.ContactPositionEnum.RecipientCc:
                                    {
                                        item.CC.Add(new MailAddress(messageContactEmail.Email, messageContactEmail.Description));
                                        break;
                                    }
                                case MessageContact.ContactPositionEnum.RecipientBcc:
                                    {
                                        item.Bcc.Add(new MailAddress(messageContactEmail.Email, messageContactEmail.Description));
                                        break;
                                    }
                                default:
                                    break;
                            }
                            FileLogger.Debug(Name, $"Aggiunto destinatario {messageContactEmail.Description} ({messageContactEmail.Email}) in {messageContact.ContactPosition}");
                        }
                    }


                    MessageEmail email = Facade.MessageEmailFacade.GetByMessage(message);

                    item.Subject = !string.IsNullOrEmpty(email.Subject) ? email.Subject : Parameters.DefaultSubject;
                    item.Subject = string.IsNullOrEmpty(item.Subject) ? item.Subject : item.Subject.Replace("\r\n", String.Empty).Replace("\n", String.Empty);
                    FileLogger.Debug(Name, "item.Subject = " + item.Subject);

                    item.Body = !string.IsNullOrEmpty(email.Body) ? email.Body : Parameters.DefaultBody;
                    FileLogger.Debug(Name, "item.Body = " + item.Body);

                    item.IsBodyHtml = true;

                    var allegati = Facade.MessageAttachmentFacade.GetByMessageAsDocumentInfoListForceStream(message);

                    FileLogger.Debug(Name, String.Format("Numero allegati: {0}", allegati.Count));
                    foreach (var allegato in allegati)
                    {
                        FileLogger.Debug(Name, string.Format("Signature documento {0}.", allegato.Signature));
                        item.Attachments.Add(new Attachment(new MemoryStream(allegato.Stream), FileHelper.ReplaceUnicode(FileHelper.ConvertUnicodeToAscii(allegato.Name)), allegato.IsCompliantPrint ? MediaTypeNames.Application.Pdf : MediaTypeNames.Application.Octet));
                        FileLogger.Debug(Name, string.Format("Allegato {0} aggiunto.", FileHelper.ReplaceUnicode(FileHelper.ConvertUnicodeToAscii(allegato.Name))));
                    }

                    // Impostazione Priority
                    item.Priority = email.Priority;
                    _mailClient = new MailClient(
                        Parameters.ServerType.ToString(),
                        Parameters.Server, Parameters.ServerPort,
                        Parameters.AuthenticationType,
                        Parameters.UserName, Parameters.UserPassword, Parameters.UserDomain);

                    // In caso di modalità DEBUG modifico i destinatari con quello di default:
                    if (Parameters.DebugModeEnabled)
                    {
                        // Creo una nuova mail alla quale aggiungo come allegato la mail originale
                        var debugMail = new MailMessage
                        {
                            Subject = String.Format("Inoltro messaggio per DEBUG {0} -> ", email.Subject),
                            From = item.From,
                            Body = "In allegato la mail che sarebbe stata spedita."
                        };

                        // Aggiungo il destinatario di debug
                        debugMail.To.Add(new MailAddress(Parameters.DebugModeAddress, "DEBUG ADDRESS"));

                        // Aggiungo la mail come allegato
                        debugMail.Attachments.Add(item.ToAttachment(new DirectoryInfo(Parameters.TempFolder)));

                        // Sostituisco item con il debugMail
                        item = debugMail;

                        FileLogger.Info(Name, string.Format("Modificato l'indirizzo di invio della mail con l'indirizzo {0}.", Parameters.DebugModeAddress));
                    }
                    var emlData = _mailClient.Send(item,
                        !String.IsNullOrEmpty(Parameters.TempFolder) ? new DirectoryInfo(Parameters.TempFolder) : null, true,
                        email.IsDispositionNotification, originalSender == null ? item.From : originalSender);
                    // Salvo in Biblos l'eml inviato
                    if (message.Location != null)
                    {
                        var emlDocument = new MemoryDocumentInfo(emlData, "Messaggio.eml") { Signature = "*" }.ArchiveInBiblos(message.Location.DocumentServer, message.Location.ProtBiblosDSDB);
                        email.EmlDocumentId = emlDocument.DocumentId;
                    }
                    email.SentDate = DateTime.Now;
                    Facade.MessageEmailFacade.Update(ref email);

                    OnSent(message);

                    FileLogger.Info(Name, string.Format("Fine elaborazione messaggio [{0}].", message.Id));
                }
                catch (Exception ex)
                {
                    OnError(message, ex);
                }
                finally
                {
                    // Attendo sempre 15 secondi tra un invio e il successivo
                    Thread.Sleep(Parameters.SleepBetweenSends * 1000);
                }
            }
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            _mailClient = null;
        }

        #endregion
    }
}
