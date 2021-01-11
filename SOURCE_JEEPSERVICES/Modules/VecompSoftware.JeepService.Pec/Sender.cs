using Limilabs.Client;
using Limilabs.Client.IMAP;
using Limilabs.Client.SMTP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.GenericMailSender.Extensions;
using VecompSoftware.Helpers;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Pec
{
    public class Sender
    {
        private readonly string _loggerName;
        private readonly FacadeFactory _factory;
        private readonly PecParams _parameters;
        private readonly Action<string> _sendMessage;
        private readonly Func<Exception, string> _fullStacktrace;
        private readonly Func<bool> _cancelRequest;
        private static readonly char[] _mailSeparator = new char[] { ';' };
        private static readonly Regex _mailDescritionExp = new Regex("[,.]");

        public Sender(FacadeFactory factory, PecParams pars, string loggerName, Action<string> sendMessage, Func<Exception, string> fullStacktrace, Func<bool> cancelRequest)
        {
            _factory = factory;
            _loggerName = loggerName;
            _parameters = pars;
            _sendMessage = sendMessage;
            _fullStacktrace = fullStacktrace;
            _cancelRequest = cancelRequest;
        }

        /// <summary> Esegue gli invii per la PECMailBox passata per parametro. </summary>
        /// <param name="box">PECMailBox in cui cercare i messaggi da spedire.</param>
        public void Process(PECMailBox box, string password)
        {
            FileLogger.Info(_loggerName, string.Format("Avvio ciclo di invio per la casella {0} - {1}", box.Id, box.MailBoxName));

            // Gestisco la duplicazione delle PEC marchiate tali
            GenerateMultiplePecs(box);

            // Recupero da DB l'elenco delle PEC in uscita non ancora spedite (MailDate NULL). Carcio anche le PEC in stato Processing.
            bool useStatusProcessing = true;
            IList<Int32> mail_ids = _factory.PECMailFacade.GetOutgoingMails(box.Id, box.Configuration.MaxSendForSession, useStatusProcessing);
            FileLogger.Info(_loggerName, string.Format("Trovate {0} PEC da spedire.", mail_ids.Count));

            // Eseguo un ciclo sulle PEC da spedire fino al numero massimo di PEC gestibili per sessione di lavoro
            int successSendMail = 0;
            int errorSendMail = 0;
            int counter = 0;
            for (int index = 0; index < mail_ids.Count && counter < box.Configuration.MaxSendForSession; index++)
            {
                if (_cancelRequest())
                {
                    FileLogger.Info(_loggerName, "Blocco invio PEC per chiusura modulo invocata dall'utente.");
                    return;
                }

                // Avvio watch per verificare quanto ci impiega per singola mail
                //TODO : da mettere in debug mode
                Stopwatch watch = Stopwatch.StartNew();
                PECMail mail = MailStoreFacade.Factory.PECMailFacade.GetById(mail_ids[index]);

                if (mail.IsActive != ActiveType.Cast(ActiveType.PECMailActiveType.Active) && mail.IsActive != ActiveType.Cast(ActiveType.PECMailActiveType.Processing))
                {
                    FileLogger.Info(_loggerName, string.Format("Errore in lettura pecMail [{0}]. Lo status doveva essere 1. Possibile aggiornamento dati non previsto.", mail.Id));
                    return;
                }

                MailStoreFacade.Factory.TaskHeaderFacade.ActivatePECTaskProcess(mail);

                mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Processing);
                _factory.PECMailFacade.Update(ref mail);
                FileLogger.Info(_loggerName, string.Format("Inizio processamento PEC [{0}] per invio. - Item #{1}", mail.Id, counter));

                // Preparo i dati per l'invio
                Guid guid = Guid.NewGuid();
                String fullName = string.Empty;
                IMail iMailPec = null;
                int i = 0;
                Exception lastException = null;
                String errorLastSendMail = string.Empty;
                while (iMailPec == null && i < 5)
                {
                    // Salvo solamente gli errori dell'ultimo step esecuitivo
                    StringBuilder errorMessage = new StringBuilder();

                    try
                    {
                        iMailPec = PrepareEmlData(mail, guid, ref fullName, ref errorMessage);
                        if (iMailPec == null)
                        {
                            FileLogger.Error(_loggerName, string.Format("Tentativo di creazione eml {0}/5 saltato per massimo numero di elementi in Temp raggiunto per la PEC [{1}]", i + 1, mail.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMsg = string.Format("Errore in fase di inserimento in Biblos per invio della PEC [{0}] - Tentativo {1}/5", mail.Id, i + 1);
                        errorMessage.Append(errMsg);
                        FileLogger.Error(_loggerName, errMsg, ex);
                        lastException = ex;

                        if (_cancelRequest())
                        {
                            String message = "Blocco invio PEC per chiusura modulo invocata dall'utente a causa di errore in fase di preparazione dati EML.";
                            errorMessage.Append(message);
                            _factory.PECMailLogFacade.Warning(ref mail, message);
                            FileLogger.Info(_loggerName, message);
                            return;
                        }

                        Thread.Sleep(1000 * 30);
                    }
                    finally
                    {
                        errorLastSendMail = errorMessage.ToString();
                        i++;
                    }
                }

                // Ho eseguito operazioni importanti per cui libero la memoria
                GC.Collect();

                // Se a questo punto non sono riuscito ad ottenere un oggetto iMail significa che ci sono problemi su Biblos o su FileSystem,
                // quindi notifico, blocco e procedo oltre
                if (iMailPec == null)
                {
                    String errorResult = string.Format("La PEC {0} della casella {1} - {2} non è stata correttamente inserita in Biblos per 5 volte. La PEC non è stata inviata ed è stata disattivata (isActive = 255) e dovrà essere riattivata previa verifica dell'errore.", mail.Id, box.Id, box.MailBoxName);
                    mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error);
                    _factory.PECMailFacade.UpdateOnly(ref mail);
                    _factory.PECMailLogFacade.Error(ref mail, string.Concat(errorResult, errorLastSendMail));
                    MailStoreFacade.SetPECMailTaskError(mail);
                    throw new Exception(errorResult, lastException);
                }

                // Se sono qui significa che tutti i dati su Biblos sono stati memorizzati
                // Spedisco la PEC via SMTP
                try
                {
                    if (SendMail(box, iMailPec, mail, guid, password))
                    {                        
                        // Se sono riuscito ad inviare allora salvo su Server via IMAP
                        successSendMail++;

                        try
                        {
                            //Aggiorno l'attributo MailDate per l'archivio di conservazione
                            BiblosDocumentInfo mainInfo = BiblosDocumentInfo.GetDocuments(mail.Location.ConservationServer, mail.IDMailContent).SingleOrDefault();
                            FileLogger.Info(_loggerName, "Tentativo di aggiornare attributo MailDate");
                            mainInfo.AddAttribute("MailDate", mail.MailDate.Value.ToString(new CultureInfo("it-IT") { DateTimeFormat = new DateTimeFormatInfo { ShortDatePattern = "dd/MM/yyyy" } }), true);
                            mainInfo.Update("BibloDS");
                            FileLogger.Info(_loggerName, string.Format("Attributi della PEC con ID {0} aggiornati.", mail.Id));
                        }
                        catch (Exception ex)
                        {
                            _factory.PECMailLogFacade.Error(ref mail, string.Format("Errore in fase di aggiornamento attributi conservazione: {0}", ex.Message));
                            _sendMessage(string.Format("Errore in fase di aggiornamento attributi conservazione: {0} - Stacktrace: {1}", ex.Message, _fullStacktrace(ex)));
                        }

                        try
                        {
                            mail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active);
                            _factory.PECMailFacade.UpdateOnly(ref mail);                            

                            SaveMail(box, iMailPec, mail, password);                            
                        }
                        catch (Exception ex)
                        {
                            _factory.PECMailLogFacade.Error(ref mail, string.Format("Errore in fase di salvataggio mail su IMAP: {0}", ex.Message));
                            _sendMessage(string.Format("Errore in fase di salvataggio mail su IMAP: {0} - Stacktrace: {1}", ex.Message, _fullStacktrace(ex)));
                        }

                        MailStoreFacade.CompletePECMailTask(mail);
                        // Pulisco eventuali dati sporchi rimanenti
                        CleanEmlData(mail.Id, _parameters.TempFolder);
                    }
                    else
                    {
                        errorSendMail++;
                    }
                }
                catch (Exception ex)
                {
                    _factory.PECMailLogFacade.Error(ref mail, string.Format("Errore in fase di invio mail: {0}", ex.Message));
                    _sendMessage(string.Format("Errore in fase di invio mail: {0} - Stacktrace: {1}", ex.Message, _fullStacktrace(ex)));
                    MailStoreFacade.SetPECMailTaskError(mail);
                }

                // Fermo il Watch e registro il LOG 
                watch.Stop();
                FileLogger.Info(_loggerName, string.Format("Fine processamento PEC [{0}] per invio. Gestione avvenuta in {1} millisecondi. Item #{2}", mail.Id, watch.ElapsedMilliseconds, counter));

                // Procedo con la PEC successiva
                counter++;

                // Fermo il thread per non sovraccaricare troppo i sistemi
                if (_parameters.SendSleep > 0)
                {
                    Thread.Sleep(1000 * _parameters.SendSleep);
                }
            }
            if (successSendMail > 0)
            {
                _factory.PECMailboxLogFacade.InsertLog(ref box, string.Format("PEC Inviate {0} con successo", successSendMail), PECMailBoxLogFacade.PecMailBoxLogType.Sent);
            }
            if (errorSendMail > 0)
            {
                _factory.PECMailboxLogFacade.InsertLog(ref box, string.Format("Ci sono {0} PecMail da controllare", errorSendMail), PECMailBoxLogFacade.PecMailBoxLogType.SentError);
            }

            FileLogger.Info(_loggerName, string.Format("Fine ciclo di invio per la casella {0} - {1}", box.Id, box.MailBoxName));
        }


        /// <summary>
        /// Gestisce la duplicazione delle PEC 
        /// </summary>
        /// <param name="box"></param>
        private void GenerateMultiplePecs(PECMailBox box)
        {
            var pecIds = _factory.PECMailFacade.GetOutgoingMailsToDuplicate(box.Id, box.Configuration.MaxSendForSession);

            if (pecIds.Count > 0)
            {
                ICollection<PECMail> pecs = pecIds
                    .Select(pecMailId => MailStoreFacade.Factory.PECMailFacade.GetById(pecMailId))
                    .ToList();

                FileLogger.Info(_loggerName, string.Format("Generazione delle PEC multiple per la casella {0}", box.Id));
                foreach (PECMail pecMailToSplit in pecs)
                {
                    FileLogger.Info(_loggerName, string.Format("Duplicazione PEC {0}", pecMailToSplit.Id));
                    _factory.PECMailFacade.SplitMultiPec(pecMailToSplit);
                }
            }
        }

        /// <summary>
        /// Questo metodo ha il compito di gestire la memorizzazione in Biblos e in DB di una PEC in uscita
        /// </summary>
        public IMail PrepareEmlData(PECMail pecMail, Guid guid, ref string fullname, ref StringBuilder sb)
        {
            if (fullname == null)
            {
                sb.Append("Fullname non settato.");
                throw new ArgumentNullException("fullname");
            }
            // Verifico sia definita una location, altrimenti non posso caricare gli allegati
            if (pecMail.Location == null)
            {
                pecMail.Location = pecMail.MailBox.Location;
                _factory.PECMailFacade.UpdateNoLastChange(ref pecMail);
            }

            // Salvo l'eml sul disco
            fullname = GetEmlFullName(_parameters.TempFolder, pecMail.Id, 5);
            if (!string.IsNullOrEmpty(fullname))
            {
                // Genero l'istanza di IMail da spedire via SMTP
                IMail simpleMail = GetMailMessage(guid, pecMail);
                simpleMail.Save(fullname);

                FileInfo generatedEml = new FileInfo(fullname);
                FileLogger.Debug(_loggerName, $"Envelope salvato in [{generatedEml.FullName}]. Check: {generatedEml.Exists}");

                if (pecMail.IDMailContent.Equals(Guid.Empty))
                {

                    PECMail lambdaProblemPec = pecMail;
                    HibernateSessionHelper.TryOrRollback(() =>
                    {
                        _factory.PECMailFacade.ArchiveInConservation(ref lambdaProblemPec,
                                Encoding.GetEncoding(1252).GetBytes(generatedEml.FullName), "Message.eml");
                        // Ulteriore check di coerenza
                        if (lambdaProblemPec.IDMailContent.Equals(Guid.Empty))
                        {
                            // Significa che l'archiviazione in Conservazione non è avvuta correttamente.
                            // Blocco tutto e lancio eccezione
                            throw new Exception("Non è stato possibile memorizzazione il blob in conservazione. Verificare di avere impostato un archivio di conservazione.");
                        }
                    }, _loggerName, "Impossibile memorizzare la PEC in conservazione. Possibile errore Biblos. Transazione annullata.", true);
                }

                // Verifico che già non sia stato fatto
                if (pecMail.IDPostacert == Guid.Empty)
                {
                    try
                    {
                        var dataB = File.ReadAllBytes(generatedEml.FullName);
                        _factory.PECMailFacade.ArchivePostacert(ref pecMail, dataB, "Message.eml");
                        _factory.PECMailFacade.UpdateNoLastChange(ref pecMail);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            sb.Append(ex.InnerException);
                        sb.Append(ex.Message);
                        throw new Exception(string.Format("Errore in fase di salvataggio PEC [{0}] su Biblos. Il processo procederà con la prossima PEC fino a 5 tentativi totali", pecMail.Id), ex);
                    }
                }
                else
                {
                    FileLogger.Debug(_loggerName,
                                     string.Format("La PEC {0} è già stata memorizzata in Biblos, passo saltato.", pecMail.Id));
                }

                return simpleMail;
            }

            // Se fosse già presente lo lascio lì perchè lo gestirò nella seconda parte del modulo
            // quindi proseguo con le restanti PEC
            return null;
        }

        /// <summary>
        /// Questo metodo si occupa di effettuare la bonifica di eventuali eml rimasti in sospeso
        /// </summary>
        public void CheckEmlToSend()
        {
            FileLogger.Info(_loggerName, "Inizio ciclo di verifica di eventuali EML orfani nella Temp");

            // Carico tutte gli EML della cartella Temp
            String[] emls = Directory.GetFiles(_parameters.TempFolder, "PecToSendId_*");

            FileLogger.Info(_loggerName, string.Format("Sono stati trovati {0} EML da analizzare.", emls.Count()));
            foreach (String emlFullPath in emls)
            {
                FileLogger.Info(_loggerName, string.Format("Analisi EML {0}", emlFullPath));

                // Verifico se ne frattempo il file fosse già stato analizzato
                // in tal caso procedo con il prossimo
                FileInfo checkExistence = new FileInfo(emlFullPath);
                if (!checkExistence.Exists)
                {
                    FileLogger.Info(_loggerName, string.Format("Il file {0} non esiste.", emlFullPath));
                    continue;
                }

                FileLogger.Info(_loggerName, string.Format("Il file {0} esiste.", emlFullPath));

                // Effettuo il parsing del testo
                String[] temp = emlFullPath.Split('_');
                int pecId = -1;
                if (!int.TryParse(temp[1], out pecId))
                {
                    FileLogger.Error(_loggerName, string.Format("L'EML {0} nel tenativoo di recuori della PEC in uscita sull'host {1} non è stato processato. Verrà ignorato ma il file dovrà essere spostato manualmente.", emlFullPath, Environment.MachineName));
                    _sendMessage(string.Format("L'EML {0} nel tenativoo di recuori della PEC in uscita sull'host {1} non è stato processato. Verrà ignorato ma il file dovrà essere spostato manualmente.", emlFullPath, Environment.MachineName));
                    continue;
                }

                DateTime elementDateTime = DateTime.ParseExact(temp[4], "yyyyMMdd-HHmm", CultureInfo.InvariantCulture);

                // Calcolo quanto vecchi sono i file
                TimeSpan timespan = DateTime.Now - elementDateTime;
                FileLogger.Info(_loggerName, string.Format("L'elemento è vecchio di {0} minuti.", timespan.TotalMinutes));

                // Se gli elementi memorizzati sono più vecchi di 15 minuti non ci provo più e procedo oltre
                if (timespan.TotalMinutes >= 15)
                {
                    FileLogger.Warn(_loggerName, string.Format("L'elemento {0} è vecchio di {1} minuti e quindi verrà cancellato.", emlFullPath, timespan.TotalMinutes));
                    _sendMessage(string.Format("L'elemento {0} è vecchio di {1} minuti e quindi verrà cancellato.", emlFullPath, timespan.TotalMinutes));
                    File.Delete(emlFullPath);
                    continue;
                }

                // Verifico se la PEC è stata archiviata in Biblos
                PECMail pecMail = _factory.PECMailFacade.GetById((pecId));
                if (pecMail != null)
                {
                    FileLogger.Debug(_loggerName, string.Format("PEC {0} esistente", pecMail.Id));

                    // Se sono riuscito a caricare la PEC, allora verifico 
                    if (pecMail.IDPostacert == Guid.Empty)
                    {
                        FileLogger.Debug(_loggerName,
                                         string.Format(
                                         "Una precedente memorizzazione su Biblos della PEC {0} non è andata a buon fine. Nuovo tentativo.",
                                          pecMail.Id));

                        // Significa che la memorizzazione in Biblos non è andata a buon fine
                        try
                        {
                            Byte[] dataB = File.ReadAllBytes(emlFullPath);
                            _factory.PECMailFacade.ArchivePostacert(ref pecMail, dataB, "Message.eml");
                            _factory.PECMailFacade.UpdateNoLastChange(ref pecMail);
                            FileLogger.Debug(_loggerName, string.Format(
                                "Inserimento in Biblos della PEC {0} avvenuto con successo.", pecMail.Id));
                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error(_loggerName, string.Format("Impossibile salvare su Biblos l'EML {0} per la PEC {1}", emlFullPath, pecMail.Id), ex);
                            _sendMessage(string.Format("Impossibile salvare su Biblos l'EML {0} per la PEC {1} - Stacktrace: {2}", emlFullPath, pecMail.Id, _fullStacktrace(ex)));

                            // Attendo 60 secondi, se si tratta di un problema momentaneo, in 60 secondi potrebbe risolversi
                            Thread.Sleep(1000 * 60);
                            continue;
                        }
                    }

                    // Se sono riuscito ad inserire in Biblos e la PEC non è ancora stata spedita e ancora risulta non attiva allora la riattiva
                    if (pecMail.IDPostacert != Guid.Empty && pecMail.MailDate == null && pecMail.IsActive == ActiveType.Cast(ActiveType.PECMailActiveType.Processed))
                    {
                        pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Active);
                        _factory.PECMailFacade.UpdateOnly(ref pecMail);
                        FileLogger.Debug(_loggerName, string.Format("PEC {0} riattivata per ritentare invio.", pecMail.Id));
                        _sendMessage(string.Format("PEC {0} riattivata per ritentare invio.", pecMail.Id));
                    }

                    CleanEmlData(pecMail.Id, _parameters.TempFolder);
                }
                else
                {
                    FileLogger.Error(_loggerName, string.Format("Errore in fase di gestione EML {0}", emlFullPath));
                    _sendMessage(string.Format("Errore in fase di verifica di gestione EML {0}", emlFullPath));

                    CleanEmlData(pecId, _parameters.TempFolder);
                }
            }

            FileLogger.Info(_loggerName, "Fine ciclo di verifica di eventuali EML orfani nella Temp");
        }


        /// <summary> Spedisce l'email. </summary>
        /// <param name="mailbox"> MailBox da dove spedire l'email. </param>
        /// <param name="email"> Email da spedire. </param>
        /// <param name="pecMail"> PEC da inviare </param>
        /// <param name="guid"> Guid della mail</param>
        public bool SendMail(PECMailBox mailbox, IMail email, PECMail pecMail, Guid guid, string password)
        {
            // In caso di modalità DEBUG modifico i destinatari con quello di default:
            if (_parameters.DebugModeEnabled)
            {
                // Creo una nuova mail alla quale aggiungo come allegato la mail originale
                MailBuilder debugBuilder = new MailBuilder();
                debugBuilder.From.Add(email.From.First());

                debugBuilder.Subject = string.Format("Inoltro messaggio per DEBUG -> {0}", email.Subject);

                // Crea il corpo del messaggio di default (non è richiesto dall'Interoperabilità) o lo legge da base dati, se indicato
                debugBuilder.Text = "In allegato la mail che sarebbe stata spedita.";

                // Aggiungo il destinatario di debug
                debugBuilder.To.Add(new MailBox(_parameters.PecOutAddressDebugMode, "DEBUG ADDRESS"));

                // Aggiungo la mail come allegatos
                debugBuilder.AddAttachment(email);

                // Sostituisco item con il debugMail
                email = debugBuilder.Create();

                FileLogger.Info(_loggerName, string.Format("Modificato l'indirizzo di invio della PEC con l'indirizzo {0}.", _parameters.PecOutAddressDebugMode));
            }

            // Eseguo in modo atomico il blocco di invio
            int i = 0;
            bool sent = false;
            ISendMessageResult sendResult = null;
            Exception lastException = null;

            while (!sent && i < 5)
            {
                try
                {
                    using (Smtp smtp = new Smtp())
                    {
                        smtp.ServerCertificateValidate += ServerCertificateValidateHandler;

                        // Utilizzo connessione SSL
                        if (mailbox.OutgoingServerUseSsl.HasValue && mailbox.OutgoingServerUseSsl.Value)
                        {
                            smtp.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                            smtp.ConnectSSL(mailbox.OutgoingServerName, mailbox.OutgoingServerPort.HasValue ? mailbox.OutgoingServerPort.Value : 465);
                        }
                        else
                        {
                            smtp.Connect(mailbox.OutgoingServerName, mailbox.OutgoingServerPort.HasValue ? mailbox.OutgoingServerPort.Value : 25);
                        }

                        // Utilizzo autenticazione
                        if (!string.IsNullOrEmpty(mailbox.Username) && !string.IsNullOrEmpty(password))
                        {
                            smtp.UseBestLogin(mailbox.Username, password);
                        }

                        sendResult = smtp.SendMessage(email);
                        sent = (sendResult.Status == SendMessageStatus.Success);
                        if (!sent)
                        {
                            string errors = string.Empty;
                            if (sendResult.GeneralErrors != null && sendResult.GeneralErrors.Any())
                            {
                                errors = string.Join(", ", sendResult.GeneralErrors
                                    .Select(f => string.Concat("Code: ", f.Code, " - EnhancedStatusCode: ", f.EnhancedStatusCode, " - Message: ", f.Message, " - Status : ", f.Status)));
                            }
                            FileLogger.Error(_loggerName, string.Format("Errore in spedizione PEC {0} / casella {1} - {2}. Stato spedizione: {3} - Errori:{4}.", pecMail.Id, mailbox.Id, mailbox.MailBoxName, sendResult.Status, errors));
                        }
                        smtp.Close(false);
                    }

                    if (!sent)
                    {
                        continue;
                    }

                    // Aggiorno immediatamente la PEC come spedita in modo da non avere dubbi che la PEC sia stata davvero spedita
                    pecMail.MailDate = DateTime.Now;
                    pecMail.XRiferimentoMessageID = string.Format("<{0}>", guid);
                    _factory.PECMailFacade.UpdateOnly(ref pecMail);
                    Protocol currentProtocol = _factory.PECMailFacade.GetProtocol(pecMail);
                    if (currentProtocol != null && (short)ProtocolKind.FatturePA == currentProtocol.IdProtocolKind)
                    {
                        currentProtocol.IdStatus = (int)ProtocolStatusId.PAInvoiceSent;
                        _factory.ProtocolFacade.UpdateOnly(ref currentProtocol);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    // Se mi trovo in questo status è avvenuto un errore in fase di spedizione, per cui posso riprocedere ad inviare
                    FileLogger.Error(_loggerName, string.Format("Non è stato possibile inviare la PEC {0} / casella {1} - {2} per un probabile errore di rete o di server PEC. La procedura verrà ritentata. - Tentativo {3}/5", pecMail.Id, mailbox.Id, mailbox.MailBoxName, i + 1), ex);
                    // Attendo 1 minuto prima di riprovare
#if !DEBUG
                        Thread.Sleep(1000 * 30);
#endif
                }
                finally
                {
                    // Procedo
                    i++;
                }
            }

            // Se dopo 5 tentativi ancora non ha ricevuto conferma di spedizione allora invio una mail e blocco l'invio
            if (sent)
            {
                _factory.PECMailboxLogFacade.SentMail(ref pecMail);
                return true;
            }
            // Errori
            String errorMessages = string.Format("Errori di invio:{0}", Environment.NewLine);
            errorMessages = sendResult.GeneralErrors.Aggregate(errorMessages, (current, generalError) => current + string.Format("Code:{1} - Message:{2} - Status:{3}{0}", Environment.NewLine, generalError.Code, generalError.Message, generalError.Status));
            _factory.PECMailboxLogFacade.SentErrorMail(ref pecMail, new Exception(errorMessages));

            pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error);
            _factory.PECMailFacade.UpdateOnly(ref pecMail);
            String errorResult = string.Format("La PEC {0} / casella {1} - {2} non è stata spedita dopo 5 tentativi falliti. \nE' stata pertanto disattivata (isActive = 255) per evitare ulteriori tentativi.", pecMail.Id, mailbox.Id, mailbox.MailBoxName);
            _factory.PECMailLogFacade.Error(ref pecMail, errorResult);
            _factory.PECMailLogFacade.Error(ref pecMail, string.Concat(errorResult, errorMessages));
            MailStoreFacade.SetPECMailTaskError(pecMail);
            throw new Exception(errorResult, lastException);
        }


        private void SaveMail(PECMailBox mailbox, IMail email, PECMail pecMail, string password)
        {
            // Upload su cartela Sent
            if (mailbox.IncomingServerProtocol != IncomingProtocol.Imap || !mailbox.Configuration.UploadSent || string.IsNullOrEmpty(mailbox.Configuration.FolderSent))
            {
                return;
            }
            try
            {
                using (Imap imap = new Imap())
                {
                    imap.ServerCertificateValidate += ServerCertificateValidateHandler;

                    // Utilizzo connessione SSL
                    if (mailbox.IncomingServerUseSsl.HasValue && mailbox.IncomingServerUseSsl.Value)
                    {
                        imap.ConnectSSL(mailbox.IncomingServerName, mailbox.IncomingServerPort.HasValue ? mailbox.IncomingServerPort.Value : 993);
                    }
                    else
                    {
                        imap.Connect(mailbox.IncomingServerName, mailbox.IncomingServerPort.HasValue ? mailbox.IncomingServerPort.Value : 143);
                    }

                    // Utilizzo autenticazione
                    if (!string.IsNullOrEmpty(mailbox.Username) && !string.IsNullOrEmpty(password))
                    {
                        imap.UseBestLogin(mailbox.Username, password);
                    }

                    imap.UploadMessage(mailbox.Configuration.FolderSent, email);
                    imap.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(
                    "Non è stato possibile salvare la PEC {0} / casella {1} - {2} nella cartella della posta inviata ({3}) per un probabile errore di rete o di server PEC.",
                    pecMail.Id, mailbox.Id, mailbox.MailBoxName, mailbox.Configuration.FolderSent), ex);
            }
        }


        /// <summary>
        /// Questo metodo pulisce la cartella temp da eventuali EML rimasti successivamente ad un corretto invio da parte di JS8
        /// </summary>
        /// <param name="pecMailId"> L'Id della pecMail da cercare per l'eliminazione</param>
        /// <param name="tempFolder"> La directory dove cercare gli eventuali EML</param>
        private void CleanEmlData(int pecMailId, string tempFolder)
        {
            // Ho eseguito tutte le operazioni richiesta, per cui posso cancellare l'eml completamente da disco
            bool lastEmlFound = false;
            int i = 0;
            while (!lastEmlFound && i < 5)
            {
                // Carico il primo tentativo di salvataggio
                String[] tempEmlFileNames = Directory.GetFiles(tempFolder, string.Format("PecToSendId_{0}_Try_{1}_*", pecMailId, i));
                foreach (FileInfo tempEmlFileInfo in tempEmlFileNames.Select(tempEmlFileName => new FileInfo(tempEmlFileName)))
                {
                    // Se esiste effettivamente allora lo rimuovo
                    if (tempEmlFileInfo.Exists)
                    {
                        tempEmlFileInfo.Delete();
                    }
                    // Se non esiste allora fermo il ciclo e risparmio tempo e risorse
                    else
                    {
                        lastEmlFound = true;
                    }
                }
                i++;
            }
        }

        private void ServerCertificateValidateHandler(object sender, ServerCertificateValidateEventArgs e)
        {
            e.IsValid = true;
        }


        /// <summary>
        /// Questo metodo calcola il primo nome disponibile per la memorizzazione di un file EML in temp
        /// </summary>
        /// <param name="tempFolder"> cartella dove andrà salvato il file</param>
        /// <param name="idPecMail"> id della PEC da salvare</param>
        /// <param name="totalRetry"> numero di tentativi da effettuare</param>
        /// <returns></returns>
        private string GetEmlFullName(string tempFolder, int idPecMail, int totalRetry)
        {
            int i = 0;
            String emlFullName = string.Empty;
            while (string.IsNullOrEmpty(emlFullName) && i < totalRetry)
            {
                // Calcolo un possibile nome
                String tempEmlName = Path.Combine(tempFolder, string.Format("PecToSendId_{0}_Try_{1}_{2}_.eml", idPecMail, i, DateTime.Now.ToString("yyyyMMdd-HHmm")));
                // Calcolo se ci sono stati già altri tentativi (senza considerare la data e l'ora)
                String[] tempEmlFileNames = Directory.GetFiles(tempFolder, string.Format("PecToSendId_{0}_Try_{1}_*", idPecMail, i));
                // Se il nome trovato non esiste già allora lo restituisco
                if (!tempEmlFileNames.Any())
                {
                    emlFullName = tempEmlName;
                }
                i++;
            }
            return emlFullName;
        }


        private IMail GetMailMessage(Guid guid, PECMail pecMail)
        {
            MailBuilder builder = new MailBuilder();

            foreach (MailBox mailBox in MailStringToList(pecMail.MailSenders))
            {
                builder.From.Add(mailBox);
            }
            foreach (MailBox mailBox in MailStringToList(pecMail.MailRecipients))
            {
                builder.To.Add(mailBox);
            }
            foreach (MailBox mailBox in MailStringToList(pecMail.MailRecipientsCc))
            {
                builder.Cc.Add(mailBox);
            }

            builder.Subject = string.IsNullOrEmpty(pecMail.MailSubject) ? string.Empty : StringHelper.ReplaceCrLf(pecMail.MailSubject);

            // Crea il corpo del messaggio di default (non è richiesto dall'Interoperabilità) o lo legge da base dati, se indicato
            builder.Text = (string.IsNullOrEmpty(pecMail.MailBody))
                               ? string.Format("Invio protocollo \"{0}\" ({1}/{2})", pecMail.MailSubject, pecMail.Number, pecMail.Year)
                               : pecMail.MailBody;

            builder.MessageID = guid.ToString();

            if (!string.IsNullOrEmpty(pecMail.Segnatura))
            {
                // Estrae la segnatura dalla base dati e la allega alla mail in uscita
                MimeData xmlSegnatura = builder.AddAttachment(Encoding.GetEncoding(1252).GetBytes(pecMail.Segnatura));
                xmlSegnatura.ContentType = new ContentType(MimeType.Text, MimeSubtype.Xml);
                xmlSegnatura.ContentId = "Segnatura.xml";
                xmlSegnatura.FileName = "Segnatura.xml";
            }

            // Estrae i relativi allegati da base dati (documento ed allegati)
            if ((pecMail.Attachments != null) && (pecMail.Attachments.Count > 0))
            {
                byte[] attachmentByteArray;
                foreach (PECMailAttachment attachment in pecMail.Attachments)
                {
                    try
                    {
                        attachmentByteArray = null;
                        string attachmentName = FileHelper.ReplaceUnicode(FileHelper.ConvertUnicodeToAscii(attachment.AttachmentName));
                        if (pecMail.Location != null && !string.IsNullOrEmpty(pecMail.Location.DocumentServer) && attachment.IDDocument != Guid.Empty)
                        {
                            // Allora utilizzo l'idDocument
                            BiblosDocumentInfo doc = new BiblosDocumentInfo(pecMail.Location.DocumentServer, attachment.IDDocument);
                            attachmentByteArray = doc.Stream;
                            FileLogger.Debug(_loggerName, string.Format("Caricamento allegato {0} della PEC {1} inserito da DSW8 utilizzando l'IDDocument", attachmentName, pecMail.Id));
                        }
                        else
                        {
                            throw new Exception("L'allegato non contiene nè un idBiblos valido nè uno stream valido e pertanto non può essere inviato.");
                        }
                        MimeData document = builder.AddAttachment(attachmentByteArray);
                        if (attachmentName.EndsWith(FileHelper.EML, StringComparison.InvariantCultureIgnoreCase))
                        {
                            document.ContentType = new ContentType(MimeType.Message, MimeSubtype.Rfc822);
                        }
                        else
                        {
                            document.ContentType = new ContentType(MimeType.Application, MimeSubtype.OctetStream);
                        }
                        document.ContentId = attachmentName;
                        document.FileName = attachmentName;
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(_loggerName, string.Format("Errore in aggiunta allegati alla PECMail [{0}].", pecMail.Id), ex);
                    }
                }
            }

            // Estrae il messaggio di ritorno dalla base dati, se presente e lo allega alla mail in uscita
            if (pecMail.MessaggioRitornoName != null && pecMail.MessaggioRitornoStream != null)
            {
                MimeData messaggioRitornoStream = builder.AddAttachment(Encoding.GetEncoding(1252).GetBytes(pecMail.MessaggioRitornoStream));
                messaggioRitornoStream.ContentType = new ContentType(MimeType.Text, MimeSubtype.Xml);
                messaggioRitornoStream.ContentId = pecMail.MessaggioRitornoStream;
                messaggioRitornoStream.FileName = pecMail.MessaggioRitornoStream;
            }

            if (pecMail.MailPriority.HasValue)
            {
                switch (pecMail.MailPriority)
                {
                    case -1:
                        builder.Priority = MimePriority.NonUrgent;
                        break;
                    case 1:
                        builder.Priority = MimePriority.Urgent;
                        break;
                    default:
                        builder.Priority = MimePriority.Normal;
                        break;
                }
            }
            else
            {
                builder.Priority = MimePriority.Normal;
            }

            return builder.Create();
        }

        /// <summary> Preleva contatti da una stringa. </summary>
        /// <param name="mailUsers">stringhe separate da virgole</param>
        private static IEnumerable<MailBox> MailStringToList(string mailUsers)
        {
            List<MailBox> list = new List<MailBox>();
            if (string.IsNullOrEmpty(mailUsers))
            {
                return list;
            }

            String name;
            String email;
            ICollection<MailBox> items = mailUsers
                .Split(_mailSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(item =>
                    {
                        name = RegexHelper.MatchName(item);
                        email = RegexHelper.MatchEmail(item);
                        return string.IsNullOrEmpty(name) ? new MailBox(email) : new MailBox(email, name);//_mailDescritionExp.Replace(name, string.Empty));
                    })
                    .ToList<MailBox>();

            list.AddRange(items);

            return list;
        }

    }
}
