using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Limilabs.Mail;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using Limilabs.Mail.Licensing;
using System.Collections.Specialized;
using System.ComponentModel;
using VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.Helpers.Signer.Security;
using VecompSoftware.JeepService.Pec.PecErrorManagementFiles;
using VecompSoftware.Core.Command.CQRS.Events.Models.PECMails;

namespace VecompSoftware.JeepService.Pec
{
    public class PecModule : JeepModuleBase<PecParams>
    {
        private MailStoreFacade _storeFacade;
        private JeepServiceHostFacade _jeepServiceHostFacade;
        private PECMailBoxFacade _PECMailBoxFacade;
        private List<PECMailBox> _boxes;
        private IList<PECMailBox> _incomingBoxes;
        private Dictionary<short, string> _passwordBoxes;
        private IList<PECMailBox> _outgoingBoxes;
        private PecErrorManager _pecErrorManager;
        private Dictionary<string, Action<object, PropertyChangedEventArgs>> _eventPerPropertyDictionary;

        private static EncryptionHelper _encryptionHelper;

        public static EncryptionHelper Encryption
        {
            get { return _encryptionHelper ?? (_encryptionHelper = new EncryptionHelper()); }
        }

        private IPAddress GetLocalIPAddress()
        {
            try
            {
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    return null;
                }

                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                return host
                    .AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void Initialize(List<Common.Parameter> parameters)
        {
            base.Initialize(parameters);
            
            if (Parameters.PecDirection == Direction.Watcher && Parameters.PECOldDay < 8)
            {
                string err = string.Format("Errore in [Parameters.PECOldDay] - {0} deve essere > 7", Parameters.PECOldDay);
                FileLogger.Error(Name, err);
                SendMessage(err);
                throw new ArgumentException(err);
            }
            _passwordBoxes = new Dictionary<short, string>();
            _storeFacade = new MailStoreFacade(Name, SendMessage);
            _jeepServiceHostFacade = new JeepServiceHostFacade();
            _PECMailBoxFacade = new PECMailBoxFacade();
            CheckLimilabsLicence();
            CreateAppFolders();

            //carica elenco delle caselle
            LoadMailBoxes(true);

            // Verifica che le PEC presenti in DB possiedano già l'originalRecipient
            // altrimenti rischio di generare PEC doppie
            if (Parameters.RecoverOriginalRecipient)
            {
                UpdateChecksums(_boxes);
                UpdateOriginalRecipients(_boxes);
            }
            //Assegno la una funzione dove posso gestire il cambiamento di una proprieta.
            PropertyChanged += OnPropertyChanged;
            
            //Dictionary[nome proprieta cambiata, funzione da beseguire]
            _eventPerPropertyDictionary = new Dictionary<string, Action<object, PropertyChangedEventArgs>>
            {
                {"Cancel", OnCancelPropertyChanged} // Nel caso della proprieta Cancel metto della logica nel momento in cui l'applicazione viene chiusa
            };

            //initializzo il manager che monitorizza la cartella Error. 
            _pecErrorManager = new PecErrorManager(Parameters.ErrorFolder, Parameters.FileSystemWatcherTreshold, Parameters.FileSystemWatcherRetryTimer, Name);

            //Initializza il task che si occupa dell'invio delle informazioni sulle pec in errore all'api
            _pecErrorManager.InitializeTaskForErrorPecsAsync();

        }

        /// <summary>
        /// Evento che ci indica la modifica di una proprieta del PecModule attraverso l'implementazione dell'interfaccia INotifyPropertyChanged del JeepModuleBase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //R.I.P - Replace If Pattern. 
            //Invece di usare tanti if e controllare il nome della proprieta' usiamo un dictionary[nome proprieta, funzione da eseguire]
            _eventPerPropertyDictionary[e.PropertyName](sender, e);
        }

        /// <summary>
        /// //Evento che scatta quando viene cambiata la proprieta Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _pecErrorManager.StopTask();

            _pecErrorManager.StopWatcher();
        }


        private void CreateAppFolders()
        {
            string folderName = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Parameters.TempFolder))
                {
                    Parameters.TempFolder = "temp";
                }

                //cartella temporanea
                folderName = Parameters.TempFolder;
                DirectoryInfo temporaryFolder = new DirectoryInfo(Parameters.TempFolder);
                if (!temporaryFolder.Exists)
                {
                    temporaryFolder.Create();
                }


                //cartella di ricezione ed elaborazione mail
                if (string.IsNullOrEmpty(Parameters.DropFolder))
                {
                    Parameters.DropFolder = "drop";
                }

                folderName = Parameters.DropFolder;
                DirectoryInfo dropFolder = new DirectoryInfo(Parameters.DropFolder);
                if (!dropFolder.Exists)
                {
                    dropFolder.Create();
                }

                //cartella file mail con errori
                if (!string.IsNullOrEmpty(Parameters.ErrorFolder))
                {
                    folderName = Parameters.ErrorFolder;
                    DirectoryInfo errorFolder = new DirectoryInfo(Parameters.ErrorFolder);
                    if (!errorFolder.Exists)
                    {
                        errorFolder.Create();
                    }
                }

                //cartella di backup prima della rimozione dal server
                if (!string.IsNullOrEmpty(Parameters.DumpFolder))
                {
                    folderName = Parameters.DumpFolder;
                    DirectoryInfo dumpFolder = new DirectoryInfo(Parameters.DumpFolder);
                    if (!dumpFolder.Exists)
                    {
                        dumpFolder.Create();
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("Errore in [CreateAppFolders] - Folder:{0}\nErrore: {1} \nStacktrace: {2}", folderName, ex.Message, FullStacktrace(ex));
                FileLogger.Error(Name, err, ex);
                SendMessage(err);
            }
        }

        private void LoadMailBoxes(bool showFolders)
        {
            //controllo se il modulo è registrato oppure no.
            if (!string.IsNullOrEmpty(Parameters.HostName))
            {
                if (!CheckJeepServiceRegistered() && (Parameters.PecDirection != Direction.Watcher))
                {
                    SetJeepServiceHost();
                }
            }

            switch (Parameters.PecDirection)
            {
                case Direction.In:
                    _incomingBoxes = _boxes = GetMailBoxes(Direction.In);
                    break;
                case Direction.Out:
                    _outgoingBoxes = _boxes = GetMailBoxes(Direction.Out);
                    break;
                case Direction.InOut:
                    _incomingBoxes = _boxes = GetMailBoxes(Direction.In);
                    _outgoingBoxes = GetMailBoxes(Direction.Out);
                    foreach (var pecBox in _outgoingBoxes)
                    {
                        if (!_boxes.Any(x => x.Id == pecBox.Id)) _boxes.Add(pecBox);
                    }
                    break;
                default:
                    _boxes = GetMailBoxes(Parameters.PecDirection);
                    break;
            }

            if (_boxes == null)
            {
                throw new Exception("Impossibile caricare le mailBox. Verificare eventuali problemi di mapping su DB.");
            }

            FileLogger.Debug(Name, string.Format("Caricate {0} caselle valide", _boxes.Count));
            if (showFolders)
            {
                foreach (PECMailBox box in _boxes.Where(box => box.IncomingServerProtocol == IncomingProtocol.Imap))
                {
                    CheckImapFolders(box, _passwordBoxes[box.Id]);
                }
            }
        }

        public override void SingleWork()
        {
            FileLogger.Debug(Name, "Avvio ciclo di lavoro.");

            if (!Parameters.PecMailBoxCache)
            {
                LoadMailBoxes(false);
            }

            if (_boxes == null || _boxes.Count == 0)
            {
                return;
            }

            //invia errori accumulati
            SendErrors();

            //invia mail
            if (Parameters.PecDirection == Direction.Out || Parameters.PecDirection == Direction.InOut)
            {
                SendMails(_outgoingBoxes, _passwordBoxes);
            }

            //riceve mail
            if (Parameters.PecDirection == Direction.In || Parameters.PecDirection == Direction.InOut)
            {
                ReceiveMails(_incomingBoxes, _passwordBoxes);
            }

            if (Parameters.PecDirection == Direction.Watcher)
            {
                ReceiveWatcherMails(_boxes);
            }

            if (Parameters.RecoverEnvelopeAttachment)
            {
                RecoverEnvelopeAttachments(_boxes);
            }

            FileLogger.Debug(Name, "Fine ciclo di lavoro.");
        }

        private bool CancelRequest()
        {
            return Cancel;
        }

        private void CheckLimilabsLicence()
        {
            try
            {
                String fileName = LicenseHelper.GetLicensePath();
                FileIOPermission x = new FileIOPermission(FileIOPermissionAccess.Read, LicenseHelper.GetLicensePath());
                x.Assert();
                var status = LicenseHelper.GetLicenseStatus();

                FileLogger.Debug(Name, string.Format("File di licenza Mail.dll [{0}] in stato [{1}].", fileName, status));
                if (!Parameters.IgnoreLimilabsLicence)
                {
                    switch (status)
                    {
                        case LicenseStatus.Invalid:
                            throw new Exception("Licenza Mail.dll non valida.");
                        case LicenseStatus.NoLicenseFile:
                            throw new Exception("Licenza Mail.dll non trovata: " + fileName);
                        case LicenseStatus.Valid:
                            FileLogger.Info(Name, "Licenza Mail.dll valida.");
                            break;
                    }
                }
                else
                {
                    FileLogger.Info(Name, string.Format("Stato licenza Mail.dll: [{0}] ignorato.", status));
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in [CheckLimilabsLicence].", ex);
                SendMessage(string.Format("Errore in [CheckLimilabsLicence]. \nErrore: {0} \nStacktrace: {1}", ex.Message, FullStacktrace(ex)));
            }
        }



        // Verifica se il jeepservice è già registrato 
        private bool CheckJeepServiceRegistered()
        {
            var jsHost = _jeepServiceHostFacade.GetByHostName(Parameters.HostName);
            if (jsHost != null)
            {
                FileLogger.Debug(Parameters.HostName, string.Format("Host di JeepService già registrato [{0}].", Parameters.HostName));
                return true;
            }
            FileLogger.Debug(Parameters.HostName, string.Format("Host di JeepService non registrato [{0}].", Parameters.HostName));
            return false;
        }

        //Registra il webservice nella tabella JeepServiceHosts
        private void SetJeepServiceHost()
        {
            var jsHost = new JeepServiceHost(Name);
            jsHost.IsActive = Convert.ToInt16(true);
            jsHost.IsDefault = false;
            jsHost.Hostname = Parameters.HostName;
            _jeepServiceHostFacade.Save(ref jsHost);
            FileLogger.Debug(Parameters.HostName, string.Format("Host di JeepService in registrazione [{0}].", Parameters.HostName));
        }


        private void SendErrors()
        {
            try
            {
                // Se esiste il file del giorno prima allora lo carico, lo invio e lo cancello
                String yesterdayFileNameFormat = string.Format("{0}_ServerExceptions.txt", DateTime.Today.AddDays(-1).ToString("yyyy_MM_dd"));
                String yesterdayPath = Path.Combine(Parameters.TempFolder, yesterdayFileNameFormat);
                if (!File.Exists(yesterdayPath))
                {
                    return;
                }

                SendMessage(string.Format("Riepilogo errori di connessione del giorno {0}{1}{2}", DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy"), Environment.NewLine, File.ReadAllText(yesterdayPath)));
                File.Delete(yesterdayPath);
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in [SendErrors].", ex);
                SendMessage(string.Format("Errore in [SendErrors]. \nErrore: {0} \nStacktrace: {1}", ex.Message, FullStacktrace(ex)));
            }
        }

        private List<PECMailBox> GetMailBoxes(Direction direction)
        {
            try
            {
                IList<PECMailBox> boxes = new List<PECMailBox>();
                if (string.IsNullOrEmpty(Parameters.HostName))
                {
                    boxes = GetAllMailBoxes();
                }

                else
                {
                    JeepServiceHost jeepServiceHost = _jeepServiceHostFacade.GetByHostName(Parameters.HostName);
                    if (jeepServiceHost == null)
                    {
                        boxes = GetAllMailBoxes();
                        FileLogger.Warn(Name, "Alert in [GetMailBoxes]. Nessun Host trovato per il nome indicato nei parametri.");
                    }
                    else
                    {
                        switch (direction)
                        {
                            case Direction.In:
                                boxes = _storeFacade.GetIngomingMailBoxesByHost(jeepServiceHost.Id, jeepServiceHost.IsDefault).Where(p => IsValidMailBox(p.Id)).ToList();
                                break;
                            case Direction.Out:
                                boxes = _storeFacade.GetOutgoingMailBoxesByHost(jeepServiceHost.Id, jeepServiceHost.IsDefault).Where(p => IsValidMailBox(p.Id)).ToList();
                                break;
                            default:
                                boxes = GetAllMailBoxes();
                                break;
                        }
                    }
                }

                Dictionary<PECMailBox, int> boxesToOrder = new Dictionary<PECMailBox, int>();
                string password;
                foreach (PECMailBox box in boxes)
                {
                    int countPecReceived = _PECMailBoxFacade.CountManyPECMailsReceived(box.Id, DateTime.Today.AddDays(-5));
                    boxesToOrder.Add(box, countPecReceived);
                    if (!_passwordBoxes.ContainsKey(box.Id))
                    {
                        password = box.Password;
                        if (DocSuiteContext.Current.ProtocolEnv.PECMailBoxPasswordEncriptionEnabled())
                        {
                            password = Encryption.DecryptString(box.Password, DocSuiteContext.Current.ProtocolEnv.PasswordEncryptionKey);
                        }
                        _passwordBoxes.Add(box.Id, password);
                    }

                }

                return boxesToOrder.OrderByDescending(x => x.Value).Select(s => s.Key).ToList();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in [GetMailBoxes].", ex);
                SendMessage(string.Format("Errore in [GetMailBoxes]. \nErrore: {0} \nStacktrace: {1}", ex.Message, FullStacktrace(ex)));
                return null;
            }
        }

        /// <summary>
        /// Ritorna tutte le caselle PEC valide configurate in DB.
        /// </summary>
        /// <remarks>Utilizzato se il JeepService non viene gestito da DSW</remarks>
        private IList<PECMailBox> GetAllMailBoxes()
        {
            return _storeFacade.GetMailBoxes().Where(p => IsValidMailBox(p.Id)).ToList();
        }

        private bool IsValidMailBox(int idPecMailBox)
        {
            return (Parameters.AllowedMailBoxes.Count == 0 || Parameters.AllowedMailBoxes.Contains(idPecMailBox)) &&
                   (Parameters.DisAllowedMailBoxes.Count == 0 || !Parameters.DisAllowedMailBoxes.Contains(idPecMailBox));
        }

        private void CheckImapFolders(PECMailBox box, string password)
        {
            String boxName = string.Empty;
            FileLogger.Debug(Name, string.Format("CheckImapFolders..."));
            try
            {
                boxName = string.Format("{0} - {1}", box.Id, box.MailBoxName);

                if (CancelRequest())
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                FileLogger.Debug(Name, string.Format("Verifica di {0}", boxName));

                if (box.IncomingServerProtocol == IncomingProtocol.Imap)
                {
                    Receiver receiver = new Receiver(_storeFacade, Parameters, Name, SendMessage, FullStacktrace, CancelRequest);
                    receiver.CheckImapFolder(box, password);
                }

                // Se tutto è andato bene (quindi non sono nel catch) allora libero le risorse NHibernate
                NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, string.Format("Errore in [CheckImapFolders] - Casella [{0}].", boxName), ex);
                SendMessage(string.Format("Errore in [CheckImapFolders] - Casella [{0}]. \nErrore: {1} \nStacktrace: {2}", boxName, ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                // Libero le risorse
                GC.Collect(GC.MaxGeneration);
            }
        }

        private void SendMails(IEnumerable<PECMailBox> boxes, Dictionary<short, string> passwords)
        {
            String boxName = string.Empty;
            try
            {
                Sender sender = new Sender(MailStoreFacade.Factory, Parameters, Name, SendMessage, FullStacktrace, CancelRequest);

                //controlla evenutali errori
                sender.CheckEmlToSend();

                foreach (PECMailBox box in boxes)
                {
                    boxName = string.Format("{0} - {1}", box.Id, box.MailBoxName);

                    if (CancelRequest())
                    {
                        FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                        return;
                    }

                    sender.Process(box, passwords[box.Id]);
                }

                // Se tutto è andato bene (quindi non sono nel catch) allora libero le risorse NHibernate
                NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, string.Format("Errore in [SendMails] - Casella [{0}].", boxName), ex);
                SendMessage(string.Format("Errore in [SendMails] - Casella [{0}]. \nErrore: {1} \nStacktrace: {2}", boxName, ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                // Libero le risorse
                GC.Collect(GC.MaxGeneration);
            }
        }

        /// <summary>
        /// 1. Elabora le mail scaricate in ProcessDropFolder.
        ///    In caso di riavvio del modulo come prima cosa prima di scaricare nuove mail processa quelle già scaricate
        /// 2. Per ciascuna casella di posta scarica le mail da processare
        /// 3. Elabora le mail appena scaricate nuovamente con ProcessDropFolder
        /// </summary>
        /// <param name="boxes">Elenco delle caselle di posta da controllare</param>
        private void ReceiveMails(IEnumerable<PECMailBox> boxes, Dictionary<short, string> passwords)
        {
            String boxName = string.Empty;

            try
            {

                // In caso di arresto inaspettato del js, prova ad elaborare quello che è rimasto in sospeso
                Receiver receiver = new Receiver(_storeFacade, Parameters, Name, SendMessage, FullStacktrace, CancelRequest);
                ProcessDropFolder(receiver, new SortedList<short, StatisticLog>());
                SortedList<short, StatisticLog> statistics = new SortedList<short, StatisticLog>();
                DateTime start, end;
                foreach (PECMailBox box in boxes)
                {
                    boxName = string.Format("{0} - {1}", box.Id, box.MailBoxName);
                    statistics.Add(box.Id, new StatisticLog()
                    {
                        ElaboratedTime = TimeSpan.Zero,
                        PECDone = 0,
                        PECError = 0,
                        PECReaded = 0
                    });
                    if (CancelRequest())
                    {
                        FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                        return;
                    }

                    //scarica una serie di file eml utilizzando pop3 o imap come richiesto dai parametri
                    //genera file _info.xml contenente informazioni dettagliate sulla mail
                    start = DateTime.Now;
                    statistics[box.Id].PECReaded = receiver.GetMail(box, passwords[box.Id]);
                    end = DateTime.Now;
                    statistics[box.Id].ElaboratedTime = statistics[box.Id].ElaboratedTime + (TimeSpan.FromTicks(end.Ticks) - TimeSpan.FromTicks(start.Ticks));
                }

                //elabora quello scaricato
                statistics = ProcessDropFolder(receiver, statistics);
                if (statistics != null)
                {
                    PECMailBox box;
                    DateTime logTime;
                    PECMailBoxLog logEntry;
                    IPAddress localIP = GetLocalIPAddress();
                    foreach (KeyValuePair<short, StatisticLog> item in statistics.Where(f => f.Value.ElaboratedTime.TotalSeconds > 0))
                    {
                        box = MailStoreFacade.Factory.PECMailboxFacade.GetById(item.Key, false);
                        logTime = DateTime.Now;
                        logEntry = new PECMailBoxLog()
                        {
                            Date = logTime,
                            Description = item.Value.ElaboratedTime.TotalSeconds.ToString(),
                            MailBox = box,
                            SystemComputer = localIP == null ? "127.0.0.1" : localIP.ToString(),
                            SystemUser = WindowsIdentity.GetCurrent().Name,
                            Type = PECMailBoxLogFacade.PecMailBoxLogType.TimeEval.ToString()
                        };
                        MailStoreFacade.Factory.PECMailboxLogFacade.Save(ref logEntry);

                        logEntry = new PECMailBoxLog()
                        {
                            Date = logTime,
                            Description = item.Value.PECDone.ToString(),
                            MailBox = box,
                            SystemComputer = localIP == null ? "127.0.0.1" : localIP.ToString(),
                            SystemUser = WindowsIdentity.GetCurrent().Name,
                            Type = PECMailBoxLogFacade.PecMailBoxLogType.PECDoneEval.ToString()
                        };
                        MailStoreFacade.Factory.PECMailboxLogFacade.Save(ref logEntry);

                        logEntry = new PECMailBoxLog()
                        {
                            Date = logTime,
                            Description = item.Value.PECError.ToString(),
                            MailBox = box,
                            SystemComputer = localIP == null ? "127.0.0.1" : localIP.ToString(),
                            SystemUser = WindowsIdentity.GetCurrent().Name,
                            Type = PECMailBoxLogFacade.PecMailBoxLogType.PECErrorEval.ToString()
                        };
                        MailStoreFacade.Factory.PECMailboxLogFacade.Save(ref logEntry);

                        logEntry = new PECMailBoxLog()
                        {
                            Date = logTime,
                            Description = item.Value.PECReaded.ToString(),
                            MailBox = box,
                            SystemComputer = localIP == null ? "127.0.0.1" : localIP.ToString(),
                            SystemUser = WindowsIdentity.GetCurrent().Name,
                            Type = PECMailBoxLogFacade.PecMailBoxLogType.PECReadedEval.ToString()
                        };
                        MailStoreFacade.Factory.PECMailboxLogFacade.Save(ref logEntry);
                    }
                }
                // Se tutto è andato bene (quindi non sono nel catch) allora libero le risorse NHibernate
                NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, string.Format("Errore in [ReceiveMails] - Casella [{0}].", boxName), ex);
                SendMessage(string.Format("Errore in [ReceiveMails] - Casella [{0}]. \nErrore: {1} \nStacktrace: {2}", boxName, ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                // Libero le risorse
                GC.Collect(GC.MaxGeneration);
            }
        }


        /// <summary>
        /// 1. Per ciascuna casella di posta scarica le mail da processare dal watcher
        /// 2. Elabora le mail appena scaricate nuovamente con ProcessDropFolder
        /// </summary>
        /// <param name="boxes">Elenco delle caselle di posta da controllare</param>
        private void ReceiveWatcherMails(IEnumerable<PECMailBox> boxes)
        {
            String boxName = string.Empty;

            try
            {

                Watcher watcher = new Watcher(_storeFacade, Parameters, Name, SendMessage, FullStacktrace, CancelRequest);

                foreach (PECMailBox box in boxes)
                {
                    boxName = string.Format("{0} - {1}", box.Id, box.MailBoxName);

                    if (CancelRequest())
                    {
                        FileLogger.Info(Name, "Chiusura modulo watcher invocata dall'utente.");
                        return;
                    }

                    //scarica una serie di file eml utilizzando pop3 o imap come richiesto dai parametri
                    //genera file _info.xml contenente informazioni dettagliate sulla mail
                    watcher.GetWatcherMail(box);
                }

                //elabora quello scaricato
                ProcessDropFolder(watcher);

                // Se tutto è andato bene (quindi non sono nel catch) allora libero le risorse NHibernate
                NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, string.Format("watcher - Errore in [ReceiveMails] - Casella [{0}].", boxName), ex);
                SendMessage(string.Format("watcher -Errore in [ReceiveMails] - Casella [{0}]. \nErrore: {1} \nStacktrace: {2}", boxName, ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                // Libero le risorse
                GC.Collect(GC.MaxGeneration);
            }
        }

        public void UpdateChecksums(IList<PECMailBox> boxes)
        {
            FileLogger.Info(Name, "Verifica valorizzazione \"Checksum\" in corso...");
            foreach (PECMailBox pecMailBox in boxes)
            {
                FileLogger.Debug(Name, string.Format("casella [{0} ({1})]", pecMailBox.MailBoxName, pecMailBox.Id));
                MailStoreFacade.Factory.PECMailFacade.CheckPecMailBoxHashes(pecMailBox.Id);
            }
            FileLogger.Info(Name, "Verifica valorizzazione \"Checksum\" conclusa.");
        }

        /// <summary>
        /// Verifica che gli original recipient siano già correttamente valorizzati
        /// </summary>
        public void UpdateOriginalRecipients(IList<PECMailBox> boxes)
        {
            // Aggiornamento per le PEC non spostate
            FileLogger.Info(Name, "Verifica valorizzazione \"OriginalRecipient\" in corso...");
            foreach (var pecMailBox in boxes)
            {
                FileLogger.Debug(Name, string.Format("casella [{0} ({1})]", pecMailBox.MailBoxName, pecMailBox.Id));
                MailStoreFacade.Factory.PECMailFacade.CalculateMissingOriginalRecipient(pecMailBox.Id);
            }
            FileLogger.Info(Name, "Verifica valorizzazione \"OriginalRecipient\" conclusa.");

            var elementsNotRecovered = boxes.SelectMany(pecMailBox => MailStoreFacade.Factory.PECMailFacade.GetElementsWithoutOriginalRecipient(pecMailBox.Id)).ToList();
            if (elementsNotRecovered.Count > 0)
            {
                var elements = new StringBuilder();
                foreach (var pecMailHeader in elementsNotRecovered)
                {
                    elements.AppendFormat("Pec id [{0}]{1}", pecMailHeader.Id, Environment.NewLine);
                }
                FileLogger.Warn(Name, string.Format("Gli elementi {0} risultano ancora da correggere. {1}Verificare manualmente il database.", elements, Environment.NewLine));
                // Blocco il modulo
                Cancel = true;
            }
        }

        /// <summary>
        /// Elabora la posta scaricata nel drop Folder
        /// </summary>
        /// <param name="receiver"></param>
        private SortedList<short, StatisticLog> ProcessDropFolder(Receiver receiver, SortedList<short, StatisticLog> statistics)
        {
            statistics = receiver.UpdateHeaderHash(statistics, _passwordBoxes);
            statistics = receiver.ProcessFiles(statistics);
            statistics = receiver.ArchiveFiles(statistics);
            statistics = receiver.ReleaseFiles(Parameters.DumpFolder, statistics, _passwordBoxes);
            return statistics;
        }

        /// <summary>
        /// Watcher Elabora la posta scaricata nel drop Folder
        /// </summary>
        /// <param name="receiver"></param>
        private void ProcessDropFolder(Watcher watcher)
        {
            watcher.UpdateHeaderHash();
            watcher.ProcessFiles();
            watcher.ArchiveFiles();
            watcher.ReleaseFiles(Parameters.DumpFolder);
        }



        private void RecoverEnvelopeAttachments(IEnumerable<PECMailBox> boxes)
        {
            var boxName = string.Empty;

            try
            {
                foreach (var box in boxes)
                {
                    boxName = string.Format("{0} - {1}", box.Id, box.MailBoxName);

                    if (CancelRequest())
                    {
                        FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                        return;
                    }

                    FileLogger.Info(Name, "Recupero Envelope in corso...");
                    // Recupero l'elenco delle PEC da riprocessare
                    NHibernatePECMailFinder finder = new NHibernatePECMailFinder
                    {
                        Actives = true,
                        MailDateFrom = !Parameters.RecoverEnvelopeAttachmentStartDate.Equals(default(DateTime)) ? (DateTime?)Parameters.RecoverEnvelopeAttachmentStartDate : null,
                        MailDateTo = !Parameters.RecoverEnvelopeAttachmentEndDate.Equals(default(DateTime)) ? (DateTime?)Parameters.RecoverEnvelopeAttachmentEndDate : null,
                        MailboxIds = new[] { box.Id },
                    };
                    List<PECMail> pecMails = finder.DoSearch().Where(pecMail =>
                        pecMail.Direction != (short)DocSuiteWeb.Data.PECMailDirection.Outgoing &&
                        pecMail.IDMailContent != Guid.Empty &&
                        !pecMail.LogEntries.Any(pml => pml.Type.Eq(PECMailLogType.Reprocessed.ToString()))).
                        Take(box.Configuration.MaxReadForSession).ToList();
                    FileLogger.Info(Name, string.Format("Trovate {0} pec da riprocessare", pecMails.Count));
                    foreach (PECMail pecMail in pecMails)
                    {
                        PECMail currentPecMail = pecMail;
                        // Recupero la PEC dal server di conservazione
                        BiblosDocumentInfo originalEml = FacadeFactory.Instance.PECMailFacade.GetPecMailContent(currentPecMail);
                        if (originalEml == null)
                        {
                            continue;
                        }
                        BiblosDocumentInfo originalEnvelope = new BiblosDocumentInfo(currentPecMail.Location.DocumentServer, currentPecMail.IDEnvelope);

                        FileLogger.Info(Name, string.Format("Estrazione eml originale GUID_Chain_EML {0} PEC [{1}] da riprocessare - EML name [{2}]", currentPecMail.IDMailContent, currentPecMail.Id, originalEml.Name));
                        try
                        {
                            Guid originalEnvelopeId = currentPecMail.IDEnvelope;
                            IMail envelope = new MailBuilder().CreateFromEml(originalEml.Stream);
                            FileLogger.Debug(Name, "Generata busta da EML");
                            envelope.RemoveAttachments();
                            FileLogger.Debug(Name, "Rimossi allegati con successo, archiviazione in corso...");
                            MailStoreFacade.Factory.PECMailFacade.ArchiveEnvelope(ref currentPecMail,
                                envelope.Render(), originalEnvelope.Name);
                            MailStoreFacade.Factory.PECMailFacade.UpdateNoLastChange(ref currentPecMail);
                            FileLogger.Info(Name,
                                string.Format(
                                    "Aggiornamento Envelope avvenuto correttamente: nuovo GUID_Chain [{0}]",
                                    currentPecMail.IDEnvelope));
                            MailStoreFacade.Factory.PECMailLogFacade.InsertLog(ref currentPecMail, string.Format("Ricalcolata busta: guidPrecedente [{0}] -> guidRicalcolato [{1}]", originalEnvelopeId, currentPecMail.IDEnvelope), PECMailLogType.Reprocessed);
                        }
                        catch (Exception ex)
                        {
                            FileLogger.Error(Name, "Errore in fase di ricalcolo busta.", ex);
                            SendMessage(
                                string.Format("Errore in fase di ricalcolo busta per la PEC {1} - Guid: [{2}].{0}Stacktrace: {3}", Environment.NewLine, pecMail.Id, pecMail.IDEnvelope, FullStacktrace(ex)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, string.Format("Errore in [RecoverEnvelopeAttachments] - Casella [{0}].", boxName), ex);
                SendMessage(string.Format("Errore in [RecoverEnvelopeAttachments] - Casella [{0}]. \nErrore: {1} \nStacktrace: {2}", boxName, ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                // Libero le risorse
                NHibernateManager.NHibernateSessionManager.Instance.CloseTransactionAndSessions();
                GC.Collect(GC.MaxGeneration);
            }
        }



    }
}
