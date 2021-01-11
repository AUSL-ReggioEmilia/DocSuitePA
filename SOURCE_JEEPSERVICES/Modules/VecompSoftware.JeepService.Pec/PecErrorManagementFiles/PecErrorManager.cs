using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.PECMails;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Model.Entities.PECMails;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.MailManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Pec.PecErrorManagementFiles
{
    public class PecErrorManager
    {
        private const string EmlExtension = ".eml";
        private const string EmlTermination = "_mail.eml";
        private const string XmlTermination = "_info.xml";
        private const string XmlExtension = ".xml";
        private readonly FileSystemWatcher _watcher;
        private readonly string _errorFolder;
        private readonly string _moduleName;
        private readonly int _fileThreshold;
        private readonly int _retryIntervalParameter;
        private CancellationTokenSource _tokenSource;

        public PecErrorManager(string errorFolder, int thresholdIntervalParameter, int retryIntervalParameter, string moduleName)
        {
            _errorFolder = errorFolder;
            _moduleName = moduleName;
            _fileThreshold = thresholdIntervalParameter;
            _retryIntervalParameter = retryIntervalParameter;

            _watcher = new FileSystemWatcher(errorFolder);
            _watcher.Created += OnFileCreatedAsync;
        }

        /// <summary>
        /// Evento che gestisce l'arrivo di un file nella cartella delle pec in errore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileCreatedAsync(object sender, FileSystemEventArgs e)
        {
            try
            {
                //aspetta che il file finisca di essere copiato. Facciamo questo perche non viene copiato sempre tutto in un instante e l'evento scatta al primo bit copiato
                while (!IsFileReady(e.FullPath) && File.Exists(e.FullPath)) { }

                if (OtherHalfExists(e.FullPath))
                {
                    Guid correlationId = new Guid();

                    string otherHalfPath = OtherHalfPath(e.FullPath);

                    PECMailErrorStreamModel streamModel = new PECMailErrorStreamModel()
                    {
                        CorrelatedId = correlationId,
                        Stream = Path.GetExtension(e.FullPath) == EmlExtension ? File.ReadAllBytes(e.FullPath) : File.ReadAllBytes(otherHalfPath)
                    };

                    MailInfo mailInfo = Path.GetExtension(e.FullPath) == EmlExtension ? MailInfo.Load(otherHalfPath) : MailInfo.Load(e.FullPath);
                    PECMailErrorSummaryModel summaryModel = new PECMailErrorSummaryModel()
                    {

                        CorrelatedId = correlationId,
                        ProcessedErrorMessages = string.Join(Environment.NewLine, mailInfo.Errors),
                        Subject = mailInfo.Subject,
                        Body = mailInfo.Body,
                        Sender = mailInfo.Sender,
                        Recipients = mailInfo.Recipients,
                        ReceivedDate = mailInfo.Date,
                        Priority = mailInfo.Priority,
                    };

                    //effettua la chiamata all api
                    bool requestWasSuccessfull = CallWebApi(summaryModel, streamModel);

                    if (requestWasSuccessfull)
                    {
                        File.Delete(e.FullPath);
                        File.Delete(otherHalfPath);
                    }
                }
            }
            catch (Exception exception)
            {
                FileLogger.Error(_moduleName, $"Error occured: {exception.Message} ");
            }
        }

        /// <summary>
        /// Funzione che fa aspettare finche il file e' pronto per l'uso
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool IsFileReady(String filename)
        {
            // Se il file puo essere aperto anche solamente per lettura vuol dire
            // che non e' piu bloccato da un altro processo
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ferma il watcher
        /// </summary>
        public void StopWatcher()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose(); ;
        }

        /// <summary>
        /// Funzione che effettua la scansione della cartella delle pec in errore e rielabora i tentativi falliti
        /// </summary>
        private void Scan()
        {
            try
            {
                Dictionary<string, bool> filesToElaborate = Directory.GetFiles(_errorFolder)
                    .Where(filePath => ThresholdIsValid(filePath) && OtherHalfExists(filePath))
                    .ToDictionary(x => x, x => false);

                while (filesToElaborate.Any(x => !x.Value))
                {
                    Guid correlationId = Guid.NewGuid();

                    //mi prendo il primo (prossimo) eml da elaborare
                    string currentEmlPath = filesToElaborate.First(x => x.Key.EndsWith(EmlExtension)).Key;

                    //cerco il suo file info
                    string currentXmlPath = filesToElaborate.First(x => x.Key.EndsWith(OtherHalfName(currentEmlPath))).Key;

                    MailInfo mailInfo = MailInfo.Load(currentXmlPath);
                    PECMailErrorSummaryModel summaryModel = new PECMailErrorSummaryModel()
                    {
                        CorrelatedId = correlationId,
                        ProcessedErrorMessages = string.Join(Environment.NewLine, mailInfo.Errors),
                        Subject = mailInfo.Subject,
                        Body = mailInfo.Body,
                        Sender = mailInfo.Sender,
                        Recipients = mailInfo.Recipients,
                        ReceivedDate = mailInfo.Date,
                        Priority = mailInfo.Priority,
                    };
                    PECMailErrorStreamModel streamModel = new PECMailErrorStreamModel()
                    {
                        CorrelatedId = correlationId,
                        Stream = File.ReadAllBytes(currentEmlPath)
                    };

                    //gli segno come elaborati anche se magari la richiesta all api schianta, perche' non posso processari gli stessi all infinito, ma gli riprocesso al prossimo scan
                    filesToElaborate[currentXmlPath] = true;
                    filesToElaborate[currentEmlPath] = true;

                    bool requestWasSuccessfull = CallWebApi(summaryModel, streamModel);

                    if (requestWasSuccessfull)
                    {
                        File.Delete(currentEmlPath);

                        File.Delete(currentXmlPath);
                    }
                }
            }
            catch (Exception e)
            {
                FileLogger.Error(_moduleName, $"Error occured: {e.Message}");
            }
        }

        /// <summary>
        /// Verifica se il tempo che e' passato dalla creazione del file fino ad ora e' maggiore del parametro Threshold configurato
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool ThresholdIsValid(string path)
        {
            return (DateTime.Now - File.GetCreationTime(path)).TotalMilliseconds >= _fileThreshold;
        }

        /// <summary>
        /// Dato il del file eml ad esempio questa funzione ci dice se il file info esiste o meno.
        /// Funziona ugualmente se passiamo il file info in ingresso, ovvero ci direbbe se il file eml esite o meno.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool OtherHalfExists(string path)
        {
            return Directory.GetFiles(_errorFolder).Any(x => x.EndsWith(OtherHalfName(path)));
        }

        /// <summary>
        /// Dato il del file eml ad esempio questa funzione restituisce il nome del file info. 
        /// Funziona ugualmente se passiamo il file info in ingresso, ovvero restituirebbe il nome del file eml
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string OtherHalfName(string path)
        {
            FileInfo fInfo = new FileInfo(path);
            return fInfo.Extension == XmlExtension
                ? fInfo.Name.Replace(XmlTermination, EmlTermination)
                : fInfo.Name.Replace(EmlTermination, XmlTermination);
        }

        /// <summary>
        /// Dato il del file eml ad esempio questa funzione restituisce il path del file info. 
        /// Funziona ugualmente se passiamo il file info in ingresso, ovvero restituirebbe la path del file eml
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string OtherHalfPath(string path)
        {
            FileInfo fInfo = new FileInfo(path);
            return Path.Combine(fInfo.DirectoryName, OtherHalfName(fInfo.Name));
        }

        /// <summary>
        /// Metodo che gestisce il watcher della cartella error. Ad ogni intervallo di tempo (configurato) effettua una scansione della cartella Error
        /// per riprovare a mandare quei file che non sono stato inviati al web api.
        /// </summary>
        public async Task InitializeTaskForErrorPecsAsync()
        {
            //Task with cancellation token
            _tokenSource = new CancellationTokenSource();
            CancellationToken stopThreadThoken = _tokenSource.Token;

            Action task = () =>
            {
                while (!stopThreadThoken.IsCancellationRequested)
                {
                    Scan();

                    //riprendo l'attivita del watcher
                    _watcher.EnableRaisingEvents = true;

                    /*Attendo l'intervallo di tempo configurato facendo il thread "respirare" ogni 5 secondi */
                    for (int i = 0; i < TimeSpan.FromMilliseconds(_retryIntervalParameter).Minutes * 12 && !stopThreadThoken.IsCancellationRequested; i++)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }

                    //metto in pausa il watcher perche segue la scansione
                    _watcher.EnableRaisingEvents = false;
                }
            };

            await Task.Run(task, stopThreadThoken);
        }

        /// <summary>
        /// Metodo che cancella il tokenSource quindi che ferma il task
        /// </summary>
        public void StopTask()
        {
            //ferma il thread che gestisce le pec in errore cancellando il token
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        /// <summary>
        /// //Metodo che chiama la web api per inviare gli eventi. I due parametri verrano usati per creare I due eventi
        /// </summary>
        /// <param name="summaryModel"></param>
        /// <param name="streamModel"></param>
        /// <returns></returns>

       
        private bool CallWebApi(PECMailErrorSummaryModel summaryModel, PECMailErrorStreamModel streamModel)
        {
            try
            {
                EventErrorStreamPECMail eventStream = new EventErrorStreamPECMail(streamModel.CorrelatedId,
                    DocSuiteContext.Current.CurrentTenant.TenantName, DocSuiteContext.Current.CurrentTenant.TenantId,
                    new IdentityContext(DocSuiteContext.Current.User.FullUserName), streamModel);

                EventErrorSummaryPECMail eventSummary = new EventErrorSummaryPECMail(streamModel.CorrelatedId,
                    DocSuiteContext.Current.CurrentTenant.TenantName, DocSuiteContext.Current.CurrentTenant.TenantId,
                    new IdentityContext(DocSuiteContext.Current.User.FullUserName), summaryModel);

                var webApiHelper = new WebAPIHelper();

                #region Spedizione Evento summary alle web api

                FileLogger.Info(_moduleName, "Spedizione dell\'evento summary alle WebAPI");
                bool sended = webApiHelper.SendRequest(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, eventSummary, string.Empty);
                if (!sended)
                {
                    FileLogger.Warn(_moduleName, "La fase di invio dell\'evento alle Web API non è avvenuta correttamente. Vedere log specifico per maggiori dettagli");
                    FileLogger.Info(_moduleName, "E' avvenuto un errore durante la fase di invio dell'evento EventErrorSummaryPecMail alle WebAPI");
                }
                FileLogger.Info(_moduleName, "Spedizione dell\'evento EventErrorSummaryPecMail alle WebAPI avvenuto correttamente");

                #region old (working) implementation
                //using (HttpClient _client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
                //{
                //    var content = new ObjectContent<EventErrorSummaryPECMail>(eventSummary, new JsonMediaTypeFormatter()
                //    {
                //        SerializerSettings = DocSuiteContext.DefaultWebAPIJsonSerializerSettings
                //    });
                //    var httpResponseMessage = _client.PostAsync("http://10.11.1.65:90/DSW.WebAPI/api/sb/Topic", content).Result;
                //};
                #endregion

                #endregion

                #region Spedizione Evento stream alle web api

                FileLogger.Info(_moduleName, "Spedizione dell\'evento stream alle WebAPI");
                sended = webApiHelper.SendRequest(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, eventStream, string.Empty);
                if (!sended)
                {
                    FileLogger.Warn(_moduleName, "La fase di invio dell\'evento alle Web API non è avvenuta correttamente. Vedere log specifico per maggiori dettagli");
                    FileLogger.Info(_moduleName, "E' avvenuto un errore durante la fase di invio dell'evento EventErrorStreamPecMail alle WebAPI");
                }
                FileLogger.Info(_moduleName, "Spedizione dell\'evento EventErrorStreamPecMail alle WebAPI avvenuto correttamente");

                #endregion

                return true;


            }
            catch (Exception e)
            {
                FileLogger.Error(_moduleName, $"Error occured: " + e.Message);
                return false;
            }
        }
    }
}
