using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using VecompSoftware.MailManager;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Logging;
using VecompSoftware.DocSuiteWeb.Facade;
using Limilabs.Client;
using VecompSoftware.JeepService.Pec.IterationTrackerFiles;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.JeepService.Pec
{
    public class Receiver
    {
        private readonly MailStoreFacade _storeFacade;
        private readonly string _loggerName;
        private readonly PecParams _parameters;
        private readonly Action<string> _sendMessage;
        private readonly Func<Exception, string> _fullStacktrace;
        private readonly Func<bool> _cancelRequest;
        private const int MaxRetry = 5;

        public Receiver(MailStoreFacade facade, PecParams pars, string loggerName, Action<string> sendMessage, Func<Exception, string> fullStacktrace, Func<bool> cancelRequest)
        {
            _storeFacade = facade;
            _parameters = pars;
            _loggerName = loggerName;
            _sendMessage = sendMessage;
            _fullStacktrace = fullStacktrace;
            _cancelRequest = cancelRequest;

            if (!Path.IsPathRooted(_parameters.DropFolder))
            {
                _parameters.DropFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _parameters.DropFolder);
            }
        }

        public int GetMail(PECMailBox box, string password)
        {
            String step = string.Empty;
            String recipient = string.Empty;
            short boxId = 0;
            int counter = 0;

            try
            {
                boxId = box != null ? box.Id : (short)0;
                recipient = box != null
                    ? string.Format("{0} [{1}]", box.MailBoxName, box.IncomingServerName)
                    : "Casella sconosciuta";
                if (box == null)
                {
                    throw new ApplicationException();
                }

                FileLogger.Info(_loggerName, string.Format("Avvio ciclo di lettura per la casella {0} - {1}", boxId, recipient));

                IMailClient client = null;
                //legge nuove mail
                if (box.IncomingServerProtocol == IncomingProtocol.Imap)
                {
                    step = "ReceiveImap";
                    client = new ImapClient(CreateImapParams(box, password));
                }
                else
                {
                    step = "ReceivePop3";
                    client = new Pop3Client(CreatePop3Params(box, password));
                }
                counter = client.GetMails(box.Id, box.IsProtocolBox.HasValue ? false : box.IsProtocolBox.Value, box.MailBoxName, _storeFacade.HeaderHashExists, box.Configuration.NoSubjectDefaultText);

                FileLogger.Info(_loggerName, string.Format("{0} - Eseguito Download di {1} mail. Casella {2} - {3}", step, counter, boxId, recipient));
                FileLogger.Info(_loggerName, string.Format("Fine ciclo di lettura per la casella {0} - {1}", boxId, recipient));

                // Segno sul log il numero di elementi da elaborare
                MailStoreFacade.Factory.PECMailboxLogFacade.IncomingMails(ref box, counter);
                return counter;
            }
            catch (ServerException sEx)
            {
                FileLogger.Warn(_loggerName, string.Format("Errore in connessione casella {0}:{1} - Function: {2}",
                  boxId, recipient, step), sEx);

                UpdateServerException(string.Format("Errore in connessione casella {0}:{1} - Function: {2} - Exception: {3} - Stacktrace: {4}",
                  boxId, recipient, step, sEx.Message, _fullStacktrace(sEx)));

                return counter;
            }
            catch (NHibernate.AssertionFailure af)
            {
                //Se mi trovo qui significa che è stato fatto il flush della sessione e allora esco dal modulo
                FileLogger.Error(_loggerName, "Errore NHibernate: esco dal modulo", af);
                _sendMessage(string.Format("Il modulo PEC ha rilevato l'errore grave {0} e verrà pertanto invocata la chiusura del JeepService.", af.Message));
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, string.Format("Errore in lettura casella {0}:{1} - Function: {2}",
                  boxId, recipient, step), ex);

                _sendMessage(string.Format("Errore in lettura casella {0}:{1} - Function: {2} - Exception: {3} - Stacktrace: {4}", boxId, recipient, step, ex.Message, _fullStacktrace(ex)));
                return counter;
            }
            return counter;
        }


        public void CheckImapFolder(PECMailBox box, string password)
        {
            string step = string.Empty;
            var recipient = string.Empty;
            short boxId = 0;

            try
            {
                boxId = box != null ? box.Id : (short)0;
                recipient = box != null ? box.MailBoxName : "Casella sconosciuta";
                if (box == null)
                {
                    throw new ApplicationException();
                }

                FileLogger.Info(_loggerName, string.Format("CheckImapFolder per la casella {0} - {1}", boxId, recipient));
                IList<String> folders = new ImapClient(CreateImapParams(box, password)).GetFolders();

                FileLogger.Debug(_loggerName, string.Format("Informazioni sulle cartelle IMAP della casella {0} - {1}", box.Id, box.MailBoxName));
                foreach (String folder in folders)
                {
                    FileLogger.Debug(_loggerName, folder);
                }

                FileLogger.Info(_loggerName, string.Format("Fine CheckImapFolder per la casella {0} - {1}", boxId, recipient));
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, string.Format("Errore in CheckImapFolder casella {0}:{1} - Function: {2}",
                  boxId, recipient, step), ex);

                _sendMessage(string.Format("Errore in CheckImapFolder casella {0}:{1} - Function: {2} - Exception: {3} - Stacktrace: {4}",
                  boxId, recipient, step, ex.Message, _fullStacktrace(ex)));
            }
        }

        private Pop3ClientParams CreatePop3Params(PECMailBox box, string password)
        {
            return new Pop3ClientParams
            {
                DebugModeEnabled = _parameters.DebugModeEnabled,
                DropFolder = _parameters.DropFolder,
                MaxMailsForSession = box.Configuration.MaxReadForSession,
                IncomingServer = box.IncomingServerName,
                Username = box.Username,
                Password = password,
                UseSsl = box.IncomingServerUseSsl.HasValue && box.IncomingServerUseSsl.Value,
                Port = box.IncomingServerPort.HasValue ? box.IncomingServerPort.Value : (box.IncomingServerUseSsl.HasValue && box.IncomingServerUseSsl.Value ? 995 : 110),
                UserCanceled = _cancelRequest
            };
        }

        private ImapClientParams CreateImapParams(PECMailBox box, string password)
        {
            try
            {
                return new ImapClientParams
                {
                    DebugModeEnabled = _parameters.DebugModeEnabled,
                    DropFolder = _parameters.DropFolder,
                    MaxMailsForSession = box.Configuration.MaxReadForSession,
                    IncomingServer = box.IncomingServerName,
                    Username = box.Username,
                    Password = password,
                    UseSsl = box.IncomingServerUseSsl.HasValue && box.IncomingServerUseSsl.Value,
                    Port = box.IncomingServerPort.HasValue ? box.IncomingServerPort.Value : (box.IncomingServerUseSsl.HasValue && box.IncomingServerUseSsl.Value ? 993 : 143),
                    ImapFolder = box.Configuration.InboxFolder,
                    ImapStartDate = box.Configuration.ImapStartDate,
                    ImapEndDate = box.Configuration.ImapEndDate,
                    ImapSearchFlag = box.Configuration.ImapSearchFlag,
                    UserCanceled = _cancelRequest,
                    LogActionHandler = LogDebugAction
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void LogDebugAction(string str)
        {
            FileLogger.Debug(_loggerName, str);
        }

        /// <summary>
        /// Verifica se la mail scaricata utilizzando l'hash md5 dell'intero Eml se questa è già presente nel db, e quindi non deve essere ri-elaborata.
        /// Allo stesso tempo aggiorna il campo Header Hash calcolato dalla nuova versione della gestione pec nella tabella PecMail per 
        /// poter individuare da subito utilizzando il solo header se si tratta di una mail già processata
        /// Se la mail esiste lo status viene già portata ad Archivied affinche venga gestito la release con evenutale cancellazione e backup della mail.
        /// </summary>
        public SortedList<short, StatisticLog> UpdateHeaderHash(SortedList<short, StatisticLog> statistics, Dictionary<short, string> passwordBoxes)
        {
            FileLogger.Info(_loggerName, "Verifica checksum mail scaricate");

            try
            {
                String[] files = MailInfo.GetInfoFiles(_parameters.DropFolder);
                List<int> isActiveIn = new List<int> { 0, 1, 2 };
                //rimuove o sposta le mail processate
                MailInfo mailInfo = null;
                DateTime start, end;
                foreach (String filename in files)
                {
                    try
                    {
                        mailInfo = null;
                        start = DateTime.Now;
                        if (_cancelRequest())
                        {
                            FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                            return statistics;
                        }

                        mailInfo = MailInfo.Load(filename);

                        if (mailInfo.Status != MailInfo.ProcessStatus.Downloaded)
                        {
                            end = DateTime.Now;
                            if (statistics != null && mailInfo != null && mailInfo.IDPECMailBox > 0 && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                            {
                                statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                            }
                            continue;
                        }

                        //ok, la mail non esiste bisogna processarla
                        if (!MailStoreFacade.Factory.PECMailFacade.ChecksumExists(mailInfo.EmlHash, mailInfo.MailBoxRecipient))
                        {
                            end = DateTime.Now;
                            if (statistics != null && mailInfo != null && mailInfo.IDPECMailBox > 0 && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                            {
                                statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                            }
                            continue;
                        }

                        //se esiste e mi trovo a questo punto, aggiorno il valore dell'headerChecksum
                        IList<PECMail> results = MailStoreFacade.Factory.PECMailFacade.GetByChecksum(mailInfo.EmlHash, mailInfo.MailBoxRecipient, isActiveIn);
                        foreach (PECMail pecMail in results.Where(item => string.IsNullOrEmpty(item.HeaderChecksum)))
                        {
                            PECMail temp = pecMail;
                            temp.HeaderChecksum = mailInfo.HeaderHash;
                            MailStoreFacade.Factory.PECMailFacade.Update(ref temp);
                            FileLogger.Info(_loggerName, string.Format("HeaderHash aggiornato per la Mail Id:{0}.", temp.Id));
                        }

                        //Old: Non serve più importare la mail perchè esiste già, verra rimossa nella fase di Release, quindi la segno come archiviata
                        mailInfo.UpdateStatus(MailInfo.ProcessStatus.Archived);
                        mailInfo.Save();
                        WashOnlinePecNotErased(mailInfo, passwordBoxes[mailInfo.IDPECMailBox]);
                        end = DateTime.Now;

                        if (statistics != null && mailInfo != null && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                        {
                            statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                        }
                    }
                    catch (Exception ex_int)
                    {
                        String pecName = filename;
                        if (mailInfo != null)
                        {
                            pecName = mailInfo.Subject;
                            try
                            {
                                PECMailBox box = MailStoreFacade.Factory.PECMailboxFacade.GetById(mailInfo.IDPECMailBox);
                                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, "ERRORE - Verifica checksum", PECMailBoxLogFacade.PecMailBoxLogType.ErrorEval);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        FileLogger.Error(_loggerName, string.Format("Errore in verifica checksum mail scaricate della mail {0}. Il flusso normale proseguirà con le successive mail.", pecName), ex_int);
                        _sendMessage(string.Format("Errore in verifica checksum mail scaricate della mail {0}. Il flusso normale proseguirà con le successive mail. Exception: {1} - Stacktrace: {2}", pecName, ex_int.Message, _fullStacktrace(ex_int)));

                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, "Errore in verifica checksum mail scaricate.", ex);
                _sendMessage(string.Format("Errore in verifca checksum mail scaricate. Exception:{0} - Stacktrace: {1}", ex.Message, _fullStacktrace(ex)));
            }
            FileLogger.Info(_loggerName, "Fine checksum mail scaricate");
            return statistics;
        }

        /// <summary>
        /// Elabora i file _info.xml ed i relativi file eml scaricati.
        /// L'elaborazione produce una cartella con tutti gli allegati per ciascun file.
        /// Vengono anche elaborati ed espansi i file compressi
        /// Al termine del processo viene generato un file _index.xml che contiene l'elenco dei file allegati con marcatura di quelli speciali per la PEC
        /// es. Postacert.eml, daticert.xml ecc...
        /// </summary>
        public SortedList<short, StatisticLog> ProcessFiles(SortedList<short, StatisticLog> statistics)
        {
            FileLogger.Info(_loggerName, "Avvio elaborazione mail scaricate");
            try
            {
                String[] files = MailInfo.GetInfoFiles(_parameters.DropFolder);
                MailInfo mailInfo = null;
                DateTime start, end;
                foreach (String filename in files)
                {
                    start = DateTime.Now;
                    try
                    {
                        if (_cancelRequest())
                        {
                            FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                            return statistics;
                        }

                        mailInfo = MailInfo.Load(filename);
                        if (mailInfo.HasError() || mailInfo.Status != MailInfo.ProcessStatus.Downloaded)
                        {
                            continue;
                        }

                        mailInfo.Process(_parameters.DropFolder, _parameters.BiblosMaxLength);
                        end = DateTime.Now;
                        if (statistics != null && mailInfo != null && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                        {
                            statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                        }
                    }
                    catch (Exception ex_int)
                    {
                        String pecName = filename;
                        if (mailInfo != null)
                        {
                            pecName = mailInfo.Subject;
                            try
                            {
                                end = DateTime.Now;
                                if (statistics != null && mailInfo != null && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                                {
                                    statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                                    statistics[mailInfo.IDPECMailBox].PECError++;
                                }
                                PECMailBox box = MailStoreFacade.Factory.PECMailboxFacade.GetById(mailInfo.IDPECMailBox);
                                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, "ERRORE - Elaborazione EML", PECMailBoxLogFacade.PecMailBoxLogType.ErrorEval);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        FileLogger.Error(_loggerName, string.Format("Errore in elaborazione della mail {0}. Il flusso normale proseguirà con le successive mail.", pecName), ex_int);
                        _sendMessage(string.Format("Errore in elaborazione mail {0}. Il flusso normale proseguirà con le successive mail. Exception: {1} - Stacktrace: {2}", pecName, ex_int.Message, _fullStacktrace(ex_int)));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, "Errore globale in elaborazione delle mails.", ex);
                _sendMessage(string.Format("Errore globale in elaborazione delle mails. Exception:{0} - Stacktrace: {1}", ex.Message, _fullStacktrace(ex)));
            }
            FileLogger.Info(_loggerName, "Fine elaborazione mail scaricate");
            return statistics;
        }

        /// <summary>
        /// Archiviazione dei file generati dalla ProcessFolder.
        /// Questa funzione racchiude tutte le funzioni di archiviazione su storage della mail (Biblos e DocSuite)
        /// I file vengono salvati in Biblos secondo la gerarchia indicata nel file ..._index.xml
        /// Genera il record PecMail e tutte i record di supporto per i file contenuti nella Pec (postacert.eml, daticert.xml, ecc...)
        /// Genera i record per PecMailAttachments
        /// </summary>
        public SortedList<short, StatisticLog> ArchiveFiles(SortedList<short, StatisticLog> statistics)
        {
            FileLogger.Info(_loggerName, "Avvio archiviazione mail elaborate");
            PECMail pecMail = null;
            int nRetry = MaxRetry;
            String errMsg = string.Empty;
            int waitingTime = 30000;
            try
            {
                String[] files = MailInfo.GetInfoFiles(_parameters.DropFolder);
                DateTime start, end;
                foreach (String filename in files)
                {
                    start = DateTime.Now;
                    try
                    {
                        if (_cancelRequest())
                        {
                            FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                            return statistics;
                        }

                        nRetry = MaxRetry;
                        errMsg = string.Empty;
                        waitingTime = 30000;
                        IterationTracker iterationTracker = new IterationTracker();
                        while (--nRetry > 0)
                        {
                            IterationDescriptor currentIterationInfo = iterationTracker.AddIteration();
                            if (_storeFacade.ArchiveMail(_parameters.DropFolder, filename, _parameters.DebugModeEnabled, true, out pecMail, out errMsg, currentIterationInfo))
                            {
                                FileLogger.Info(_loggerName, $"Archiviazione di MailInfo [{filename}] avvenuta con successo, passaggio ad elemento successivo.");
                                if (pecMail?.MailBox != null && statistics?.Keys != null && statistics.Keys.Any(f => f == pecMail.MailBox.Id))
                                {
                                    statistics[pecMail.MailBox.Id].PECDone++;
                                }
                                break;
                            }

                            FileLogger.Warn(_loggerName, $"E' avvenuto un errore di archiviazione, potenzialmente temporaneo. Verrà ritentata l'archiviazione altre {nRetry} volte dopo una pausa di {waitingTime / 1000} sec.");
                            
                            //Errore di archiviazione, attendo 150 secondi diviso il tentativo prima di riprovare
                            if (nRetry > 0)
                            {
#if !DEBUG
                                System.Threading.Thread.Sleep(waitingTime);
#endif
                                waitingTime += waitingTime / nRetry;
                            }
                        }

                        iterationTracker.Log(_loggerName);

                        // Se mi trovo qui significa che ho esaurito i tentativi di archiviazione
                        if (nRetry <= 0)
                        {
                            String errorMessage = $"{errMsg} - Eseguiti {MaxRetry} tentativi di archiviazione senza successo.";
                            if (pecMail != null && pecMail.Id > 0)
                            {
                                String errorResult = $"La PEC {pecMail.Id} non è stata correttamente inserita ed è stata disattivata (isActive = 255) e dovrà essere riattivata previa verifica dell'errore. {errorMessage}";
                                pecMail.IsActive = ActiveType.Cast(ActiveType.PECMailActiveType.Error);
                                try
                                {
                                    MailStoreFacade.Factory.PECMailFacade.UpdateOnly(ref pecMail);
                                    MailStoreFacade.Factory.PECMailLogFacade.Error(ref pecMail, errorResult);
                                    if (pecMail != null && pecMail.MailBox != null && statistics != null &&
                                        statistics.Keys != null && statistics.Keys.Any(f => f == pecMail.MailBox.Id))
                                    {
                                        statistics[pecMail.MailBox.Id].PECError++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessage = string.Concat(errorMessage, "\n", ex.Message);
                                }
                            }
                            //throw new ApplicationException(errorMessage);
                        }
                        end = DateTime.Now;
                        if (pecMail?.MailBox != null && statistics?.Keys != null && statistics.Keys.Any(f => f == pecMail.MailBox.Id))
                        {
                            statistics[pecMail.MailBox.Id].ElaboratedTime = statistics[pecMail.MailBox.Id].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                        }
                    }
                    catch (Exception ex_int)
                    {
                        if (pecMail != null && pecMail.Id > 0)
                        {
                            try
                            {
                                PECMailBox box = MailStoreFacade.Factory.PECMailboxFacade.GetById(pecMail.MailBox.Id);
                                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, "ERRORE - Archiviazione mail", PECMailBoxLogFacade.PecMailBoxLogType.ErrorEval);
                                end = DateTime.Now;
                                if (pecMail != null && pecMail.MailBox != null && statistics != null &&
                                    statistics.Keys != null && statistics.Keys.Any(f => f == pecMail.MailBox.Id))
                                {
                                    statistics[pecMail.MailBox.Id].ElaboratedTime = statistics[pecMail.MailBox.Id].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                                    statistics[pecMail.MailBox.Id].PECError++;
                                }
                            }
                            catch (Exception ex_log)
                            {
                                FileLogger.Error(_loggerName, string.Format("Errore in archiviazione mail {0} -> logger. Il flusso normale proseguirà con le successive mail.", filename), ex_log);
                                _sendMessage(string.Format("Errore in archiviazione mail -> logger. Il flusso normale proseguirà con le successive mail.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex_log.Message, _fullStacktrace(ex_log)));

                            }
                        }

                        FileLogger.Error(_loggerName, string.Format("Errore in archiviazione mail {0}. Il flusso normale proseguirà con le successive mail.", filename), ex_int);
                        _sendMessage(string.Format("Errore in archiviazione mail. Il flusso normale proseguirà con le successive mail.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex_int.Message, _fullStacktrace(ex_int)));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, "Errore in archiviazione globale mail.", ex);
                _sendMessage(string.Format("Errore in archiviazione globale mail.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex.Message, _fullStacktrace(ex)));
            }
            FileLogger.Info(_loggerName, "Fine archiviazione mail scaricate e processate");
            return statistics;
        }

        /// <summary>
        /// Pulizia dei file generati nel drop folder per consentire l'elaborazione del contenuto della mail e l'archiviazione della stessa
        /// Inoltre si occupa di spostare le mail con errori e quelle invece archiviate con successo
        /// Se richiesto elimina la mail dal server e crea un backup su file system
        /// </summary>
        /// <param name="dumpFolder"></param>
        public SortedList<short, StatisticLog> ReleaseFiles(string dumpFolder, SortedList<short, StatisticLog> statistics, Dictionary<short, string> passwordBoxes)
        {
            FileLogger.Info(_loggerName, "Avvio pulizia file temporanei e mail archiviate");

            try
            {
                String[] files = MailInfo.GetInfoFiles(_parameters.DropFolder);
                MailInfo mailInfo = null;
                DateTime start, end;
                //rimuove o sposta le mail processate
                foreach (String filename in files)
                {
                    try
                    {
                        mailInfo = null;
                        start = DateTime.Now;
                        if (_cancelRequest())
                        {
                            FileLogger.Info(_loggerName, "Chiusura modulo invocata dall'utente.");
                            return statistics;
                        }

                        mailInfo = MailInfo.Load(filename);
                        PECMailBox box = _storeFacade.GetMailBox(mailInfo.IDPECMailBox);
                        ImapClient imapClient = new ImapClient(CreateImapParams(box, passwordBoxes[box.Id]));

                        //ERRORE DI ELABORAZIONE
                        if (mailInfo.HasError())
                        {
                            //invia warnings
                            PECMail pecMail = mailInfo.IDPECMail > 0 ? _storeFacade.GetMail(mailInfo.IDPECMail) : null;
                            if (pecMail != null && pecMail.Id > 0)
                            {
                                IList<PECMailLog> logs = MailStoreFacade.Factory.PECMailLogFacade.GetByPec(pecMail);
                                if (logs.Count > 0)
                                {
                                    foreach (PECMailLog pecMailLog in logs.Where(pecMailLog =>
                                              pecMailLog.Type == PECMailLogType.Warning.ToString() ||
                                              pecMailLog.Type == PECMailLogType.Error.ToString()))
                                    {
                                        _sendMessage(string.Format("PEC[{0}] {1}: {2}", pecMail.Id, pecMailLog.Type, pecMailLog.Description));
                                    }
                                }
                            }
                            //se indicato sposta in cartella di errore
                            if (mailInfo.Client == MailInfo.ClientType.Imap && !string.IsNullOrEmpty(box.Configuration.MoveErrorToFolder))
                            {

                                imapClient.MoveByUid(long.Parse(mailInfo.MailUID), box.Configuration.MoveErrorToFolder);
                                FileLogger.Warn(_loggerName, string.Format("Mail Uid:{0} 'SPOSTATA SU CARTELLA:{1}' nel Server {2} [{3}]", mailInfo.MailUID, box.Configuration.MoveErrorToFolder, box.MailBoxName, box.IncomingServerName));
                            }

                            //se indicato una cartella di errore sposta la mail scaricata e i relativi file
                            if (!string.IsNullOrEmpty(_parameters.ErrorFolder))
                            {
                                mailInfo.MoveFiles(_parameters.DropFolder, _parameters.ErrorFolder);
                            }
                        }

                        //ELABORATA CON SUCCESSO
                        if (mailInfo.Status == MailInfo.ProcessStatus.Archived)
                        {
                            // Verifico se è stata richiesta la cancellazione dal server
                            if (box.Configuration.DeleteMailFromServer.GetValueOrDefault(false))
                            {
                                //esegui backup prima della cancellazione
                                if (!string.IsNullOrEmpty(_parameters.DumpFolder))
                                {
                                    BackupToFolder(mailInfo, box, dumpFolder);
                                    if (!_parameters.DebugModeEnabled)
                                    {
                                        DeleteFromServer(mailInfo, box, passwordBoxes[box.Id]);
                                    }
                                    else
                                    {
                                        FileLogger.Info(_loggerName,
                                            "Cancellazione da server non effettuata per modalità debug attiva.");
                                    }
                                }
                                else throw new Exception("Impossibile effettuare il backup prima della cancellazione da server a causa della mancanza del parametro \"DumpFolder\"");
                            }
                            else
                            {
                                //nel caso di connessione Imap
                                if (mailInfo.Client == MailInfo.ClientType.Imap)
                                {
                                    if (box.Configuration.MarkAsRead && !_parameters.DebugModeEnabled)
                                    {
                                        imapClient.MarkMessageSeen(long.Parse(mailInfo.MailUID));
                                        FileLogger.Info(_loggerName, string.Format("Mail Uid:{0} 'IMPOSTATA COME LETTA' nel Server {1} [{2}]", mailInfo.MailUID, box.MailBoxName, box.IncomingServerName));
                                    }

                                    if (!string.IsNullOrEmpty(box.Configuration.MoveToFolder) && !_parameters.DebugModeEnabled)
                                    {
                                        imapClient.MoveByUid(long.Parse(mailInfo.MailUID), box.Configuration.MoveToFolder);
                                        FileLogger.Info(_loggerName, string.Format("Mail Uid:{0} 'SPOSTATA SU CARTELLA:{1}' nel Server {2} [{3}]", mailInfo.MailUID, box.Configuration.MoveToFolder, box.MailBoxName, box.IncomingServerName));
                                    }
                                }
                            }

                            //Se tutto è andato a buon fine allora procedo a svuotare la cartella di drop
                            FileLogger.Debug(_loggerName, "Cancellazione file elaborati in DropFolder");
                            mailInfo.RemoveFiles(_parameters.DropFolder);
                        }

                        end = DateTime.Now;
                        if (mailInfo != null && statistics != null && statistics.Keys != null && statistics.Keys.Any(f => f == mailInfo.IDPECMailBox))
                        {
                            statistics[mailInfo.IDPECMailBox].ElaboratedTime = statistics[mailInfo.IDPECMailBox].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                        }
                    }
                    catch (Exception ex_int)
                    {
                        String pecName = filename;
                        if (mailInfo != null)
                        {
                            pecName = mailInfo.Subject;
                            try
                            {
                                PECMailBox box = MailStoreFacade.Factory.PECMailboxFacade.GetById(mailInfo.IDPECMailBox);
                                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, "ERRORE - Pulizia EML", PECMailBoxLogFacade.PecMailBoxLogType.ErrorEval);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        FileLogger.Error(_loggerName, string.Format("Errore in pulizia file temporanei e mail archiviate della mail {0}. Il flusso normale proseguirà con le successive mail.", pecName), ex_int);
                        _sendMessage(string.Format("Errore in pulizia file temporanei e mail archiviate della mail {0}. Il flusso normale proseguirà con le successive mail. Exception: {1} - Stacktrace: {2}", pecName, ex_int.Message, _fullStacktrace(ex_int)));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(_loggerName, "Errore pulizia file temporanei e mail archiviate.", ex);
                _sendMessage(string.Format("Errore pulizia file temporanei e mail archiviate.{0}Exception: {1}{0}Stacktrace: {2}", Environment.NewLine, ex.Message, _fullStacktrace(ex)));
            }
            FileLogger.Info(_loggerName, "Fine pulizia file temporanei e mail archiviate");
            return statistics;
        }

        private void BackupToFolder(MailInfo mailInfo, PECMailBox box, string dumpFolder)
        {
            if (!File.Exists(mailInfo.EmlFilename))
                return;

            var dumpFile = string.Format("{0:000000}_{1}_{2}", box.Id, mailInfo.MailUID, Path.GetFileName(mailInfo.EmlFilename));
            var outpathEml = Path.Combine(dumpFolder, dumpFile);

            File.Copy(mailInfo.EmlFilename, outpathEml, true);
            File.Delete(mailInfo.EmlFilename);

            FileLogger.Info(_loggerName, string.Format("Mail Uid:{0} salvata in '{1}'.", mailInfo.MailUID, outpathEml));
            MailStoreFacade.Factory.PECMailboxLogFacade.Info(ref box, string.Format("Mail Uid:{0} salvata in '{1}'.", mailInfo.MailUID, outpathEml));
        }

        private void DeleteFromServer(MailInfo mailInfo, PECMailBox box, string password)
        {
            FileLogger.Debug(_loggerName, string.Format("Mail Uid:{0} - Richiesta cancellazione dal server '{1}'", mailInfo.MailUID, box.IncomingServerName));

            // Eseguo rimozione della Mail dal Server
            try
            {
                if (mailInfo.Client == MailInfo.ClientType.Imap)
                {
                    new ImapClient(CreateImapParams(box, password)).DeleteMail(long.Parse(mailInfo.MailUID));
                }
                else
                {
                    new Pop3Client(CreatePop3Params(box, password)).DeleteMail(mailInfo.MailUID);
                }

                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, string.Format("Mail Uid:{0} rimossa dal Server.", mailInfo.MailUID), PECMailBoxLogFacade.PecMailBoxLogType.ServerRemoved);
                FileLogger.Info(_loggerName, string.Format("Mail Uid:{0} rimossa dal Server.", mailInfo.MailUID));
            }
            catch (Exception ex)
            {
                MailStoreFacade.Factory.PECMailboxLogFacade.InsertLog(ref box, string.Format("Errore in rimozione Mail Uid:{0} - {1}.", mailInfo.MailUID, ex.Message), PECMailBoxLogFacade.PecMailBoxLogType.Warn);
                FileLogger.Error(_loggerName, string.Format("Errore in rimozione Mail Uid:{0}.", mailInfo.MailUID), ex);
            }
        }

        private void UpdateServerException(string text)
        {
            // Aggiungo la riga al file
            var fileNameFormat = string.Format("{0}_ServerExceptions.txt", DateTime.Today.ToString("yyyy_MM_dd"));
            var path = Path.Combine(_parameters.TempFolder, fileNameFormat);
            // Se il file non esiste allora lo creo
            if (!File.Exists(path)) File.Create(path).Dispose();
            // Apro il file e ci scrivo
            using (var sw = File.AppendText(path))
            {
                sw.WriteLine("{0} -> {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text);
            }
        }

        public bool IsEmlArchiviedInBiblos(MailInfo mailInfo)
        {
            FileLogger.Info(_loggerName, "Verifica presenza PECMail in biblos.");

            PECMail pecMail = FacadeFactory.Instance.PECMailFacade.GetById(mailInfo.IDPECMail);
            BiblosDocumentInfo originalEml = FacadeFactory.Instance.PECMailFacade.GetPecMailContent(pecMail);
            if (originalEml != null)
            {
                string originalEmlChecksum = GetString(originalEml.Checksum);
                if (originalEmlChecksum.CompareTo(mailInfo.EmlHash) == 0)
                {
                    return true;
                }
            }
            return false;
        }



        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


        /// <summary>
        /// Elimina dal server online le PECMail che non sono state cancellate a seguito di un  precedente tentativo di cancellazione fallito
        /// Se e' attiva la cancellazione online, scarica l'INTERO eml nella drop folder. Successivamente confronta l'eml con quello già archiviato in biblos. 
        /// Nel caso in cui i due file coincidono cancella la PECMail e pulisce la drop.
        /// Nel caso in cui invece i due file non coincidono prosegue col processo normale come se fosse una nuova PECMail.
        /// </summary>
        private void WashOnlinePecNotErased(MailInfo mailInfo, string password)
        {
            PECMailBox box = _storeFacade.GetMailBox(mailInfo.IDPECMailBox);
            if (!box.Configuration.DeleteMailFromServer.GetValueOrDefault(false))
            {
                if (IsEmlArchiviedInBiblos(mailInfo))
                {
                    DeleteFromServer(mailInfo, box, password);
                    mailInfo.RemoveFiles(_parameters.DropFolder);
                }
            }
        }



    }
}
