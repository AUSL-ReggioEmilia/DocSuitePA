using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Preservation.Indice;
using BiblosDS.Library.Common.Preservation.ObjectsXml;
using BiblosDS.Library.Common.Preservation.Properties;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using VecompSoftware.BiblosDS.Model.Parameters;
using VecompSoftware.BiblosDS.WCF.Common;
using VecompSoftware.ServiceContract.BiblosDS.Documents;
using BiblosStorageService = VecompSoftware.BiblosDS.Service.Storage;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public partial class PreservationService
    {
        private readonly bool _useSha256 = true;
        public BindingList<PreservationTask> PreviousTaskToExecute(PreservationTask task, Guid idArchive)
        {
            try
            {
                logger.DebugFormat("PreviousTaskToExecute {0}, {1}", task.IdPreservationTask, idArchive);
                return DbProvider.PreviousTaskToExecute(task, idArchive);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Il processo di versamento si svolge nel seguente modo:
        /// - Preparazione dei documenti da conservare partendo dalla definizione
        ///   di un task di conservazione specifico.
        /// - Creazione del PdV (Pacchetto di versamento) per il versamento dei documenti
        ///   in conservazione.
        /// - Validazione del PdV, fase di analisi dei documenti da portare in conservazione.
        /// - TODO: generare un RdV (Rapporto di versamento) specifico per la fase di conservazione,
        ///   tale rapporto dovrà contenere l'esito, per singolo documento, della fase di validazione.
        ///   Attualmente viene riportato l'XML del PdV.
        /// - Assemblaggio PdA (Pacchetto di archiviazione), copia dei documenti sul supporto di conservazione
        ///   definito nella configurazione dell'archivio.
        /// - Creazione file di supporto PdA (file IPdA, file di chiusura, file di indice per la visualizzazione del supporto e
        ///   file di lotto).
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public PreservationInfoResponse CreatePreservation(PreservationTask task)
        {
            bool persistVerifyPreservation = false;
            bool isVerifyTask = false;
            bool isCloseYearPreservation = false;
            string exceptions = string.Empty;
            Guid preservationId = Guid.Empty;
            PreservationTaskStatus taskStatus = PreservationTaskStatus.Done;
            PreservationInfoResponse result = new PreservationInfoResponse();

            if (task.IdPreservationTask == Guid.Empty)
            {
                throw new ArgumentException("il parametro task non è un argomento valido per la conservazione");
            }
            task = GetPreservationTask(task.IdPreservationTask);
            logger.InfoFormat($"CreatePreservation - esecuzione del task di conservazione con id:{task.IdPreservationTask}");

            if (!ValidatePreservationTask(task, out string errorMessage))
            {
                throw new Generic_Exception(errorMessage);
            }

            isVerifyTask = task.TaskType.Type == PreservationTaskTypes.Verify;
            isCloseYearPreservation = task.TaskType.Type == PreservationTaskTypes.CloseAnnualPreservation;
            persistVerifyPreservation = WCFUtility.GetSettingValue("PersistVerifyPreservation") != null && WCFUtility.GetSettingValue("PersistVerifyPreservation").ToString().ToLower() == "true";
            DocumentArchive archive = task.Archive;
            if (string.IsNullOrEmpty(archive.PreservationConfiguration))
            {
                new PreservationError($"Non è stata definita la configurazione per l'archivio {archive.Name}({archive.IdArchive})", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
            }
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(archive.PreservationConfiguration);
            archiveConfiguration.VerifyPreservationIncrementalEnabled = archiveConfiguration.VerifyPreservationIncrementalEnabled && (!isCloseYearPreservation &&
                                                                            (task.CorrelatedTasks == null || !task.CorrelatedTasks.Any(x => x.TaskType.Type == PreservationTaskTypes.CloseAnnualPreservation)));
            string archivePathPreservation = archive.PathPreservation;
            ArchiveCompany archiveCompany = GetArchiveCompanies(archive.IdArchive).SingleOrDefault();
            if (archiveCompany?.Company == null)
            {
                new PreservationError("Parametri insufficienti per eseguire la conservazione. Verificare i dati della \"Company\".", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
            }

            Company company = archiveCompany.Company;
            Objects.Preservation currentPreservation = null;
            PreservationFileManager fileManager = null;
            try
            {
                preservationId = GetOrCreateIdPreservationByTask(task);
                result.IdPreservation = preservationId;
                Pulse(nameof(CreatePreservation), $"Esecuzione processo di { (isVerifyTask ? "verifica" : "conservazione sostitutiva") } per la conservazione con id {preservationId}.", 1);

                UpdateCompanyTemplates(company, archiveCompany);
                Pulse(nameof(CreatePreservation), "Verifica ed inizializzazione dei documenti da portare in conservazione.", 1);

                logger.Info($"Inizializzazione documenti per la conservazione {preservationId}");
                PreservationInfoResponse documentInitializedInfoResponse = InitializePreservationDocuments(task, preservationId, archiveConfiguration);
                DateTime effectiveTaskStartDate = documentInitializedInfoResponse.StartDocumentDate.Value;
                DateTime effectiveTaskEndDate = documentInitializedInfoResponse.EndDocumentDate.Value;
                DbProvider.UpdatePreservationTaskPreservation(task, preservationId);

                ICollection<Document> documents = DbProvider.PrepareDocumentsForPreservation(archive, task, preservationId, (archiveConfiguration.VerifyPreservationDateEnabled || archiveConfiguration.ForceAutoInc));
                if (documents == null || documents.Count == 0)
                {
                    new PreservationError("Nessun documento disponibile per la conservazione.", PreservationErrorCode.E_NO_DOCUMENT_EX).ThrowsAsFaultException();
                }

                if (!isVerifyTask)
                {
                    Pulse(nameof(CreatePreservation), $"Preparazione PdV (pacchetto di versamento) per la conservazione {preservationId}", 1);
                    ICollection<Objects.AwardBatch> batches = PreservationService.GetPreservationAwardBatches(preservationId);
                    result.AwardBatchesXml = new Dictionary<Guid, string>();
                    foreach (Objects.AwardBatch awardBatch in batches)
                    {
                        DocumentService.MoveDocumentsAwardBatch(awardBatch.IdAwardBatch, false, true);
                        if (!awardBatch.IdPDVDocument.HasValue || !awardBatch.IdRDVDocument.HasValue)
                        {
                            Pulse(nameof(CreatePreservation), $"Inizio creazione pacchetto di versamento per lotto {awardBatch.IdAwardBatch}", 1);
                            result.AwardBatchesXml.Add(awardBatch.IdAwardBatch, CreateAwardBatchPDVXml(awardBatch, currentPreservation));
                            Pulse(nameof(CreatePreservation), $"Creazione pacchetto di versamento per lotto {awardBatch.IdAwardBatch} terminata correttamente", 100);
                        }
                    }
                    Pulse(nameof(CreatePreservation), $"Fine preparazione PdV per la conservazione {preservationId}", 100);
                }

                logger.Debug("Verifica eccezioni.");
                Pulse(nameof(CreatePreservation), $"Validazione documenti per la conservazione, l'attività potrebbe richiedere alcuni minuti", 1);
                if (!CheckPreservationExceptions(preservationId, out List<string> errors, documents, out currentPreservation, archiveConfiguration))
                {
                    //Nel caso di eccezione non va cancellata la conservazione                    
                    //exceptions = string.Join(Environment.NewLine, errors.Take(20));
                    if (currentPreservation != null)
                    {
                        currentPreservation.StartDate = effectiveTaskStartDate;
                        currentPreservation.EndDate = effectiveTaskEndDate;
                        DbProvider.UpdatePreservationModifyField(currentPreservation);
                    }
                    logger.Debug(string.Join(Environment.NewLine, errors));
                    new PreservationError(JsonConvert.SerializeObject(errors), PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }
                Pulse(nameof(CreatePreservation), $"Validazione documenti per la conservazione terminata correttamente", 100);

                currentPreservation.StartDate = effectiveTaskStartDate;
                currentPreservation.EndDate = effectiveTaskEndDate;

                if (currentPreservation.Documents.Count == 0)
                {
                    new PreservationError("Attenzione: non risultano presenti documenti da conservare.", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }

                //JOURNALING
                logger.Info("Inizio scrittura PreservationJournaling");
                PreservationJournalingActivity createPreservationJournalingActivity = DbProvider.GetCreatePreservationJournalingActivity();
                if (createPreservationJournalingActivity == null)
                {
                    new PreservationError("Attenzione: nessuna chiave di JournalActivity definita con codice \"CreazioneConservazione\".", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }

                PreservationJournaling journal = new PreservationJournaling
                {
                    DateActivity = DateTime.Now,
                    DateCreated = DateTime.Now,
                    User = currentPreservation.User,
                    Preservation = currentPreservation,
                    DomainUser = currentPreservation.User.DomainUser,
                    PreservationJournalingActivity = createPreservationJournalingActivity
                };

                AddPreservationJournaling(journal);
                logger.Info("PreservationJournaling scritto correttamente");

                string preservationName = PreservationFileManager.GetPreservationName(currentPreservation, isCloseYearPreservation);
                Pulse(nameof(CreatePreservation), $"Nome conservazione da utilizzare {preservationName}", 100);

                currentPreservation.Name = preservationName;
                currentPreservation.Label = preservationName;

                fileManager = new PreservationFileManager(archive, company);                
                string preservationDirectory = fileManager.CheckPreservationWritableDirectory(currentPreservation, isVerifyTask, isCloseYearPreservation);
                currentPreservation.Path = preservationDirectory;
                try
                {
                    if (Directory.Exists(preservationDirectory))
                    {
                        Directory.Delete(preservationDirectory, true);
                    }

                    Directory.CreateDirectory(preservationDirectory);
                }
                catch (Exception ex)
                {
                    new PreservationError(ex, PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
                }

                Pulse(nameof(CreatePreservation), "Inizio assemblamento PdA e copia documenti nel percorso di conservazione, l'attività potrebbe richiedere alcuni minuti.", 1);
                logger.Debug("Lettura AttributesValues completi per generazione file di conservazione");
                IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes = currentPreservation.Documents.ToDictionary(k => k.IdDocument, k => DbProvider.GetFullDocumentAttributeValues(k.IdDocument));

                long preservationSize = 0;
                int progress = 1;
                decimal percentage;
                Document document;
                decimal pulsePercentageLimit = 20;
                for (int i = 0; i < currentPreservation.Documents.Count; i++)
                {
                    document = currentPreservation.Documents[i];
                    preservationSize += document.Size.GetValueOrDefault(0);
                    MoveDocumentToPreservationFolder(preservationDirectory, document, fullDocumentAttributes);
                    percentage = ((decimal)progress / currentPreservation.Documents.Count) * 100.0m;
                    logger.Info($"Copia documenti conservati ({progress} di {currentPreservation.Documents.Count}) - {Math.Ceiling(percentage)}%.");
                    if (Math.Ceiling(percentage) > pulsePercentageLimit)
                    {
                        Pulse(nameof(CreatePreservation), $"Copia documenti nel percorso di conservazione ({pulsePercentageLimit}%).", 1);
                        pulsePercentageLimit += 20;
                    }
                    progress++;
                }

                fileManager.CheckTemplateFile(company);

                Pulse(nameof(CreatePreservation), "Copia documenti nel percorso di conservazione terminata.", 100);

                Pulse(nameof(CreatePreservation), "Inizio creazione file indice.", 1);
                CreatePreservationIndexFile_v1(currentPreservation, archiveCompany, preservationDirectory, preservationName, fullDocumentAttributes);
                Pulse(nameof(CreatePreservation), "Creazione file indice terminata correttamente.", 100);

                Pulse(nameof(CreatePreservation), "Inizio creazione file di chiusura.", 1);
                var sRet = CreatePreservationClosingFile_v1(currentPreservation, company, preservationDirectory, preservationName, exceptions, true);
                Pulse(nameof(CreatePreservation), "Creazione file di chiusura terminata.", 100);

                if (sRet.Length > 0)
                {
                    new PreservationError("Errore Creazione File Chiusura " + sRet).ThrowsAsFaultException();
                }

                Pulse(nameof(CreatePreservation), "Inizio creazione rapporto lotti di versamento.", 1);
                sRet = CreateAwardBatchRptFile(currentPreservation, company, preservationDirectory, preservationName, exceptions, fullDocumentAttributes);
                Pulse(nameof(CreatePreservation), "Creazione rapporto lotti di versamento terminata.", 100);

                if (sRet.Length > 0)
                {
                    new PreservationError("Errore Creazione rapporto lotti di versamento " + sRet).ThrowsAsFaultException();
                }

                Pulse(nameof(CreatePreservation), "Inizio creazione file Ipda.", 1);
                sRet = CreatePreservationIpdaFile(currentPreservation, preservationDirectory, preservationName, exceptions, fullDocumentAttributes);
                Pulse(nameof(CreatePreservation), "Creazione file Ipda terminata.", 100);

                if (sRet.Length > 0)
                {
                    new PreservationError("Errore Creazione File Ipda " + sRet).ThrowsAsFaultException();
                }

                if (!isVerifyTask)
                {
                    logger.Info("Caricamento file nell'archivio Biblos.");
                    currentPreservation.PathHash = UtilityService.GetHash(currentPreservation.Path + "|" + company.IdCompany, true);
                    preservationSize += AddDocumentToPreservationArchive(currentPreservation, preservationName, preservationDirectory, preservationId, archive.IdArchive);
                    if (archiveConfiguration.PreservationAutoClose)
                    {
                        currentPreservation.CloseDate = DateTime.Now;
                    }
                    //Creazione StorageDevice
                    logger.Info("Creazione device per la conservazione.");
                    string storageDeviceLabel = string.Format("01-01-{0}_31-12-{0}", currentPreservation.StartDate.GetValueOrDefault().Year);
                    var idPreservationStorageDevice = DbProvider.GetPreservationStorageForPreservation(currentPreservation, storageDeviceLabel, company.IdCompany);
                    if (!idPreservationStorageDevice.HasValue)
                    {
                        logger.InfoFormat("Create new Preservation storageDevice");
                        idPreservationStorageDevice = DbProvider.AddPreservationStorageDevice(new PreservationStorageDevice
                        {
                            DateCreated = DateTime.Now,
                            DateStorageDevice = DateTime.Now,
                            MinDate = new DateTime(currentPreservation.StartDate.GetValueOrDefault().Year, 01, 01),
                            MaxDate = new DateTime(currentPreservation.StartDate.GetValueOrDefault().Year, 12, 31),
                            Label = storageDeviceLabel,
                            IdCompany = company.IdCompany,
                            User = currentPreservation.User
                        }).IdPreservationStorageDevice;
                    }
                    DbProvider.AddPreservationInStorageDevice(new PreservationInStorageDevice
                    {
                        Device = new PreservationStorageDevice { IdPreservationStorageDevice = idPreservationStorageDevice.Value },
                        Preservation = currentPreservation
                    });
                    if (!archiveConfiguration.PreservationAutoClose)
                    {
                        CreateArchivePreservationMark_v1(idPreservationStorageDevice.Value, archive, company, fileManager);
                    }
                }
                Pulse(nameof(CreatePreservation), "Conclusione processo.", 90);
                currentPreservation.PreservationSize += preservationSize;
                currentPreservation.LockOnDocumentInsert = true;
                DbProvider.UpdatePreservationModifyField(currentPreservation);
                result.TotalRecords = currentPreservation.Documents.Count();
                if (persistVerifyPreservation)
                {
                    DbProvider.SavePreservationVerify(preservationId, null);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                try
                {
                    try
                    {
                        currentPreservation.ClearModifiedField();
                        if (persistVerifyPreservation)
                        {
                            if (ex is FaultException<ResponseError> && (ex as FaultException<ResponseError>).Detail.ErrorCode == (int)PreservationErrorCode.E_PRESERVATION_VERIFY_EX)
                            {
                                logger.InfoFormat("Preservation in VERIFY: {0}", preservationId);
                            }
                            else
                            {
                                DbProvider.SavePreservationVerify(preservationId, ex);
                            }

                            currentPreservation.LockOnDocumentInsert = true;
                        }
                        else
                        {
                            currentPreservation.LockOnDocumentInsert = false;
                        }

                        DbProvider.UpdatePreservationModifyField(currentPreservation);
                    }
                    catch (Exception abortError)
                    {
                        logger.Fatal(abortError);
                    }
                }
                finally
                {
                    taskStatus = PreservationTaskStatus.Error;
                    if (ex is FaultException<ResponseError> && (ex as FaultException<ResponseError>).Detail is PreservationError)
                    {
                        FaultException<ResponseError> faultException = ex as FaultException<ResponseError>;
                        logger.Error(faultException);
                        result.Error = (PreservationError)faultException.Detail;
                        switch ((PreservationErrorCode)faultException.Detail.ErrorCode)
                        {
                            case PreservationErrorCode.E_NO_DOCUMENT_EX:
                                {
                                    taskStatus = PreservationTaskStatus.NoDocuments;
                                }
                                break;
                            case PreservationErrorCode.E_EXIST_NO_CONSERVATED_DOCUMENT:
                                {
                                    taskStatus = PreservationTaskStatus.ExistNoConservatedDocuments;
                                }
                                break;
                        }
                    }
                    else
                    {
                        logger.Error(ex);
                        result.Error = new PreservationError(ex.Message, PreservationErrorCode.E_SYSTEM_EXCEPTION);
                    }
                }
            }
            finally
            {
                if (fileManager != null)
                {
                    fileManager.Dispose();
                }

                try
                {
                    if (!isVerifyTask)
                    {
                        var dir = fileManager.CheckPreservationWritableDirectory(currentPreservation, true, isCloseYearPreservation);
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            SavePreservationTaskStatus(task, taskStatus, result.HasErros, result.HasErros ? result.Error.Message : null);
            result.Documents = null;
            logger.InfoFormat("{0} - ritorno al chiamante.", nameof(CreatePreservation));

            return result;
        }


        public bool CreateArchivePreservationMark_v1(Guid idStorageDevice, DocumentArchive archive, Company company, PreservationFileManager fileManager = null)
        {
            logger.InfoFormat("CreateArchivePreservationMark_v1 - id supporto {0}", idStorageDevice);

            var retval = false;

            try
            {
                if (fileManager == null)
                {
                    fileManager = new PreservationFileManager(archive, company);
                }

                var device = DbProvider.GetPreservationStorageDevice(idStorageDevice);

                if (device == null)
                {
                    new PreservationError($"Non ci sono archivi informatici aventi ID { idStorageDevice }", PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();
                }

                if (string.IsNullOrEmpty(company.TemplateADEFile))
                {
                    company.TemplateADEFile = Resources.Template;
                    DbProvider.UpdateCompany(company);
                }

                ICollection<Guid> preservationIds = GetIdPreservationsInStorageDevice(idStorageDevice);
                ICollection<Objects.Preservation> preservations = new List<Objects.Preservation>();
                foreach (Guid id in preservationIds)
                {
                    preservations.Add(GetPreservation(id, false));
                }
                var conservazioni = preservations.OrderBy(x => x.Archive.Name).ThenBy(x => x.StartDate);

                var buffer = company.TemplateADEFile;
                var marca = string.Empty;

                buffer = buffer.Replace("%PeriodoImposta%", device.MinDate.HasValue ? device.MinDate.Value.ToString("yyyy") : DateTime.Now.ToString("yyyy"));
                buffer = buffer.Replace("%MarcaTemporale%", marca);
                buffer = buffer.Replace("%NumDocumenti%", conservazioni.GroupBy(x => x.IdArchive).Count().ToString());

                var numDocumenti = new Dictionary<Guid, int>();
                var dataInizio = new Dictionary<Guid, List<DateTime>>();
                var dataFine = new Dictionary<Guid, List<DateTime>>();
                var sbImpronte = new StringBuilder();
                FileInfo fileChiusura;
                DirectoryInfo pathConservazione;
                string dummy;
                string[] tmp;
                List<string> lines;
                var listaArchivi = new List<DocumentArchive>();

                string adeTxtFile = fileManager.CheckADETxtFile(device);
                //var fileDeiFiles = new FileInfo(Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "txt")));                

                if (File.Exists(adeTxtFile))
                {
                    File.Delete(adeTxtFile);
                }

                foreach (var cons in conservazioni)
                {
                    var directoryOfPreservation = cons.Path;
                    pathConservazione = new DirectoryInfo(directoryOfPreservation);

                    if (!pathConservazione.Exists)
                    {
                        new PreservationError("Percorso conservazione inesistente per conservazione avente ID " + cons.IdPreservation).ThrowsAsFaultException();
                    }

                    fileChiusura = pathConservazione.GetFiles("CHIUSURA*.txt").SingleOrDefault();

                    if (fileChiusura == null || !fileChiusura.Exists)
                    {
                        new PreservationError("File chiusura conservazione inesistente per conservazione avente ID " + cons.IdPreservation).ThrowsAsFaultException();
                    }

                    cons.Archive = ArchiveService.GetArchive(cons.IdArchive);

                    if (!listaArchivi.Any(x => x.IdArchive == cons.IdArchive))
                    {
                        listaArchivi.Add(cons.Archive);
                    }

                    lines = new List<string>(File.ReadAllLines(fileChiusura.FullName));

                    #region NUMERO DOCUMENTI

                    dummy = lines.Where(x => x.Trim().StartsWith("Numero Documenti:")).SingleOrDefault();

                    if (!numDocumenti.ContainsKey(cons.IdArchive))
                    {
                        numDocumenti[cons.IdArchive] = 0;
                    }

                    numDocumenti[cons.IdArchive] += (string.IsNullOrWhiteSpace(dummy)) ? 0 : int.Parse(dummy.Replace("Numero Documenti:", string.Empty).Trim());

                    #endregion numero documenti
                    #region DATA INIZIO VALIDITA'

                    dummy = lines.Where(x => x.Trim().StartsWith("Data Primo Documento:")).SingleOrDefault();

                    if (!dataInizio.ContainsKey(cons.IdArchive))
                    {
                        dataInizio[cons.IdArchive] = new List<DateTime>();
                    }

                    if (string.IsNullOrWhiteSpace(dummy))
                    {
                        dataInizio[cons.IdArchive].Add(DateTime.MinValue);
                    }
                    else
                    {
                        tmp = dummy.Replace("Data Primo Documento:", string.Empty).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        dataInizio[cons.IdArchive].Add(new DateTime(int.Parse(tmp[2]), int.Parse(tmp[1]), int.Parse(tmp[0])));
                    }

                    #endregion data inizio validità
                    #region DATA FINE VALIDITA'

                    dummy = lines.Where(x => x.Trim().StartsWith("Data Ultimo Documento:")).SingleOrDefault();

                    if (!dataFine.ContainsKey(cons.IdArchive))
                    {
                        dataFine[cons.IdArchive] = new List<DateTime>();
                    }

                    if (string.IsNullOrWhiteSpace(dummy))
                    {
                        dataFine[cons.IdArchive].Add(DateTime.MinValue);
                    }
                    else
                    {
                        tmp = dummy.Replace("Data Ultimo Documento:", string.Empty).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        dataFine[cons.IdArchive].Add(new DateTime(int.Parse(tmp[2]), int.Parse(tmp[1]), int.Parse(tmp[0])));
                    }

                    #endregion data fine validità
                    #region HASH FILE CHIUSURA + PERCORSO

                    lines.Insert(0, string.Format("Impronta File SHA1: {0}", GetHashFromFile(fileChiusura.FullName, false)));
                    lines.Insert(0, string.Format("Percorso File: {0}", Path.Combine(fileChiusura.Directory.Name, fileChiusura.Name)));

                    #endregion
                    //Hash del file di chiusura della conservazione corrente.
                    File.AppendAllLines(adeTxtFile, lines);
                }

                var isSha2 = ConfigurationManager.AppSettings["FormatoSuperImprontaSHA2"] ?? "True";

                buffer = buffer.Replace("%Impronta%", UtilityService.GetHash(adeTxtFile, isSha2.Equals("True", StringComparison.InvariantCultureIgnoreCase)));

                Documento docXml;
                StringBuilder sbListaDocumenti = new StringBuilder();
                DateTime dataVal;
                TipoDocumento enumTipoDoc;
                foreach (var itemArchive in listaArchivi)
                {
                    docXml = new Documento();

                    docXml.Numero = numDocumenti[itemArchive.IdArchive];

                    dataVal = dataInizio[itemArchive.IdArchive].Min();
                    docXml.DataInizioVal = new DataImpegno { Anno = dataVal.Year, Mese = dataVal.Month, Giorno = dataVal.Day };

                    dataVal = dataFine[archive.IdArchive].Max();
                    docXml.DataFineVal = new DataImpegno { Anno = dataVal.Year, Mese = dataVal.Month, Giorno = dataVal.Day };

                    docXml.TipoDocumento = Enum.TryParse<TipoDocumento>(archive.FiscalDocumentType ?? string.Empty, out enumTipoDoc) ? enumTipoDoc : TipoDocumento.AltriDocumenti;

                    sbListaDocumenti.Append(docXml.GetSerializedForm());
                }

                buffer = buffer.Replace("%ListaDocumenti%", sbListaDocumenti.ToString());

                var path = fileManager.CheckADEFile(device);

                var xmlOut = new XmlDocument();
                xmlOut.LoadXml(buffer);
                xmlOut.Save(path);
                retval = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException(ex.Message);
            }

            logger.InfoFormat("CreateArchivePreservationMark - ritorno al chiamante");

            return retval;
        }


        private Guid CreatePreservationByTask(PreservationTask task)
        {
            Guid? idPreservation;
            if ((idPreservation = DbProvider.GetPreservationIdByTask(task.IdPreservationTask)) != null)
            {
                return idPreservation.Value;
            }
            else
            {
                return DbProvider.CreatePreservation(task);
            }
        }

        private string CreatePreservationClosingFile_v1(Objects.Preservation preservation, Company company, string workingDir, string preservationName, string exceptions, bool useSHA256)
        {
            logger.InfoFormat("CreatePreservationClosingFile - work dir {0} eccezioni {1}", workingDir, exceptions);

            try
            {
                if (string.IsNullOrEmpty(workingDir))
                {
                    throw new Exception("Working directory non configurata correttamente.");
                }

                if (preservation.Documents == null)
                {
                    throw new Exception(string.Format("Nessun documento associato alla conservazione con id {0}", preservation.IdPreservation));
                }

                if (exceptions == null)
                {
                    exceptions = string.Empty;
                }

                var user = preservation.User;
                var sCognomeNomeCf = string.Format("{0} {1} C.F. {2}", user.Name, user.Surname, user.FiscalId);
                var nDoc = preservation.Documents.Count();

                var dtHlp = preservation.Documents.Select(x => x.DateMain).Min();
                var sDataMinima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                dtHlp = preservation.Documents.Select(x => x.DateMain).Max();
                var sDataMassima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                var sFileName = Path.Combine(workingDir, "CHIUSURA_" + preservationName + ".txt");
                var sFileIndice = Path.Combine(workingDir, "INDICE_" + preservationName + ".txt");
                var sFileIndiceXML = Path.Combine(workingDir, "INDICE_" + preservationName + ".xml");
                var sFileIndiceXsl = Path.Combine(workingDir, "INDICE_" + preservationName + ".xsl");


                var sFileIndiceNome = "INDICE_" + preservationName + ".txt";
                var sHash = UtilityService.GetHash(sFileIndice, useSHA256);
                var sHashXML = UtilityService.GetHash(sFileIndiceXML, useSHA256);
                var sHashXsl = UtilityService.GetHash(sFileIndiceXsl, useSHA256);

                var sElencoCampi = "  - NomeFile\r\n";

                var attrs = DbProvider.GetAttributeByPreservationPosition(preservation.IdArchive);

                foreach (var item in attrs)
                {
                    sElencoCampi += string.Format("  - {0}\r\n", item.Description);
                }

                sElencoCampi += "  - Impronta SHA " + (useSHA256 ? "256" : "1") + " (formato Hex)\r\n";


                var sectional = EstraiGruppi_v1(preservation);
                var groupsOutput = new StringBuilder();
                AddProcessedDoc(groupsOutput, sectional);

                var sStream = company.TemplateCloseFile;

                sStream = sStream.Replace("<%PERCORSO%>", preservationName);
                sStream = sStream.Replace("<%FILE_INDICE%>", sFileIndiceNome);
                sStream = sStream.Replace("<%DATA_ORA%>", DateTime.Now.Date.ToString("d") + " alle " + DateTime.Now.ToString("HH:mm"));
                sStream = sStream.Replace("<%EVIDENZA_INFORMATICA%>", useSHA256 ? "impronta SHA256" : "impronta SHA1");
                sStream = sStream.Replace("<%EVIDENZA_INDICE_TXT%>", sHash);
                sStream = sStream.Replace("<%EVIDENZA_INDICE_XML%>", sHashXML);
                sStream = sStream.Replace("<%EVIDENZA_INDICE_XSL%>", sHashXsl);
                sStream = sStream.Replace("<%AZIENDA%>", company.CompanyName);
                sStream = sStream.Replace("<%PIVA_AZIENDA%>", company.FiscalCode);
                sStream = sStream.Replace("<%RESPONSABILE%>", sCognomeNomeCf);
                sStream = sStream.Replace("<%ARCHIVIO%>", preservation.Archive.Name);
                sStream = sStream.Replace("<%TIPO_DOCUMENTI%>", preservation.Archive.FiscalDocumentType);
                sStream = sStream.Replace("<%NUMERO_DOCUMENTI%>", nDoc.ToString());
                sStream = sStream.Replace("<%DATA_PRIMO_DOCUMENTO%>", sDataMinima);
                sStream = sStream.Replace("<%DATA_ULTIMO_DOCUMENTO%>", sDataMassima);
                sStream = sStream.Replace("<%ELENCO_CAMPI%>", sElencoCampi);
                sStream = sStream.Replace("<%ID_CONSERVAZIONE%>", preservation.IdPreservation.ToString());
                sStream = sStream.Replace("<%GRUPPI%>", groupsOutput.Length > 0 ? groupsOutput.ToString() : "");

                if (exceptions.Length > 0)
                {
                    exceptions = "Si sono verificate eccezioni non bloccanti\r\n" + exceptions;
                    sStream = sStream.Replace("<%ECCEZIONI%>", exceptions);
                }
                else
                {
                    sStream = sStream.Replace("<%ECCEZIONI%>", "");
                }

                preservation.CloseContent = Encoding.ASCII.GetBytes(sStream);

                DbProvider.UpdatePreservationModifyField(preservation);

                File.WriteAllText(sFileName, sStream);

                logger.Info("CreatePreservationClosingFile - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message + " ===> " + e.Source;
            }
        }

        private void CreatePreservationIndexFile_v1(Objects.Preservation preservation, ArchiveCompany archiveCompany, string workingDir, string preservationName, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            if (preservation == null)
            {
                throw new ArgumentNullException("Non è stata definita la conservazione da eseguire");
            }

            if (string.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentNullException("Non è stato definito il percorso di conservazione");
            }

            DataTable textualIndexDataTable = new DataTable();
            textualIndexDataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "Num."
            });

            textualIndexDataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "NomeFileInArchivio"
            });

            IEnumerable<DocumentAttribute> attributes = DbProvider.GetAttributesFromArchive(preservation.IdArchive)
                .Where(x => !x.ConservationPosition.HasValue || x.ConservationPosition.Value > 0);
            foreach (DocumentAttribute attr in attributes.OrderBy(x => x.ConservationPosition))
            {
                textualIndexDataTable.Columns.Add(new DataColumn()
                {
                    ColumnName = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description
                });
            }

            textualIndexDataTable.Columns.Add(new DataColumn()
            {
                ColumnName = "ImprontaFileSHA256"
            });

            var fileName = Path.Combine(workingDir, $"INDICE_{preservationName}.txt");
            var fileNameXML = Path.Combine(workingDir, $"INDICE_{preservationName}.xml");
            var fileNameXsl = Path.Combine(workingDir, $"INDICE_{preservationName}.xsl");

            File.WriteAllText(fileNameXsl, archiveCompany.TemplateXSLTFile);

            IndiceReport indice = new IndiceReport();
            indice.File = new IndiceFile[] { };
            IndiceFile indiceFile;
            ICollection<IndiceFileAttributo> indiceFileAttributi;
            foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
            {
                BindingList<DocumentAttributeValue> docAttributes = fullDocumentAttributes[doc.IdDocument];

                DataRow textualIndexRow = textualIndexDataTable.NewRow();
                indiceFile = new IndiceFile();
                indiceFileAttributi = new List<IndiceFileAttributo>();
                textualIndexRow.SetField("Num.", doc.PreservationIndex.GetValueOrDefault());
                indiceFile.Progressivo = doc.PreservationIndex.GetValueOrDefault();

                string fileNameOnArchive = string.Format("{0}{1}", PurgeFileName(docAttributes, string.IsNullOrEmpty(doc.PrimaryKeyValue) ? doc.PreservationIndex.GetValueOrDefault().ToString() : doc.PrimaryKeyValue), Path.GetExtension(doc.Name));
                textualIndexRow.SetField("NomeFileInArchivio", fileNameOnArchive);
                indiceFileAttributi.Add(new IndiceFileAttributo()
                {
                    Nome = "NomeFileInArchivio",
                    Value = fileNameOnArchive
                });

                string currentAttributeValue;
                DocumentAttributeValue documentAttributeValue;
                foreach (DocumentAttribute attr in attributes)
                {
                    documentAttributeValue = docAttributes.SingleOrDefault(x => x.Attribute.IdAttribute == attr.IdAttribute);
                    if (documentAttributeValue == null)
                    {
                        documentAttributeValue = new DocumentAttributeValue();
                    }

                    currentAttributeValue = documentAttributeValue.Value != null ? documentAttributeValue.Value.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(attr.Format))
                    {
                        try
                        {
                            currentAttributeValue = string.Format(attr.Format, documentAttributeValue.Value);
                        }
                        catch (Exception) { }
                    }

                    string attributeName = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description;
                    textualIndexRow.SetField(attributeName, currentAttributeValue);
                    if (!string.IsNullOrEmpty(currentAttributeValue))
                    {
                        indiceFileAttributi.Add(new IndiceFileAttributo()
                        {
                            Nome = attributeName,
                            Value = currentAttributeValue
                        });
                    }
                }

                textualIndexRow.SetField("ImprontaFileSHA256", doc.DocumentHash);
                indiceFileAttributi.Add(new IndiceFileAttributo()
                {
                    Nome = "ImprontaFileSHA256",
                    Value = doc.DocumentHash
                });
                indiceFile.Attributo = indiceFileAttributi.ToArray();
                indice.File = indice.File.Concat(new[] { indiceFile }).ToArray();
                textualIndexDataTable.Rows.Add(textualIndexRow);
            }

            StringBuilder stringBuilder = new StringBuilder();
            string headerLine = string.Empty;
            foreach (DataColumn columnHeader in textualIndexDataTable.Columns)
            {
                int columnMaxLenght = textualIndexDataTable.AsEnumerable().Select(row => row.Field<string>(columnHeader.ColumnName)).Max(v => v?.Length).Value;
                if (columnMaxLenght < columnHeader.ColumnName.Length)
                {
                    columnMaxLenght = columnHeader.ColumnName.Length;
                }
                headerLine += columnHeader.ColumnName.PadRight(columnMaxLenght + 1);
            }
            stringBuilder.AppendLine(headerLine);

            string rowLine = string.Empty;
            foreach (DataRow row in textualIndexDataTable.Rows)
            {
                rowLine = string.Empty;
                foreach (DataColumn columnHeader in textualIndexDataTable.Columns)
                {
                    int columnMaxLenght = textualIndexDataTable.AsEnumerable().Select(r => r.Field<string>(columnHeader.ColumnName)).Max(v => v?.Length).Value;
                    if (columnMaxLenght < columnHeader.ColumnName.Length)
                    {
                        columnMaxLenght = columnHeader.ColumnName.Length;
                    }
                    rowLine += row[columnHeader.ColumnName].ToString().PadRight(columnMaxLenght + 1);
                }
                stringBuilder.AppendLine(rowLine);
            }

            File.WriteAllText(fileName, stringBuilder.ToString());
            indice.SaveTo(fileNameXML, $"INDICE_{preservationName}.xsl");

            var hexValue = CalculateHashCode(fileName, true);
            preservation.IndexHash = hexValue;
            DbProvider.UpdatePreservationModifyField(preservation);
        }



        /// <summary>
        /// Add Document to BiblosDS Archive
        /// Default Archive Name = Preservation
        /// Change default value key: set preservation parameter "PreservationArchiveId"
        /// </summary>
        /// <param name="preservation"></param>
        /// <param name="preservationName"></param>
        /// <param name="workingDir"></param>
        /// <param name="idPreservation"></param>
        /// <param name="idArchive"></param>
        /// <returns>
        /// Total Bytes of the files
        /// </returns>
        private long AddDocumentToPreservationArchive(Objects.Preservation preservation, string preservationName, string workingDir, Guid idPreservation, Guid idArchive)
        {
            var fileChiusura = Path.Combine(workingDir, "CHIUSURA_" + preservationName + ".txt");
            var fileIndice = Path.Combine(workingDir, "INDICE_" + preservationName + ".txt");
            var fileIndiceXML = Path.Combine(workingDir, "INDICE_" + preservationName + ".xml");
            var fileIndiceXSLT = Path.Combine(workingDir, "INDICE_" + preservationName + ".xsl");

            Guid? idChain = null;
            var presPrms = GetPreservationParameter(idArchive);
            DocumentArchive archive = null;
            if (!presPrms.ContainsKey("PreservationArchiveId"))
            {
                if ((archive = ArchiveService.GetArchiveByName("Preservation")) == null)
                {
                    return 0;
                }
            }
            else
            {
                archive = new DocumentArchive(new Guid(presPrms["PreservationArchiveId"]));
            }

            var documentChiusura = new Document
            {
                Archive = archive,
                AttributeValues = new BindingList<DocumentAttributeValue>
                    {
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdPreservation"},
                            Value = idPreservation
                        },
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdArchive"},
                            Value = idArchive
                        },
                    },
                Content = new DocumentContent(File.ReadAllBytes(fileChiusura)),
                Name = Path.GetFileName(fileChiusura)
            };
            var documentIndice = new Document
            {
                Archive = archive,
                AttributeValues = new BindingList<DocumentAttributeValue>
                    {
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdPreservation"},
                            Value = idPreservation
                        },
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdArchive"},
                            Value = idArchive
                        },
                    },
                Content = new DocumentContent(File.ReadAllBytes(fileIndice)),
                Name = Path.GetFileName(fileIndice)
            };
            var documentIndiceXML = new Document
            {
                Archive = archive,
                AttributeValues = new BindingList<DocumentAttributeValue>
                    {
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdPreservation"},
                            Value = idPreservation
                        },
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdArchive"},
                            Value = idArchive
                        },
                    },
                Content = new DocumentContent(File.ReadAllBytes(fileIndiceXML)),
                Name = Path.GetFileName(fileIndiceXML)
            };
            var documentIndiceXSLT = new Document
            {
                Archive = archive,
                AttributeValues = new BindingList<DocumentAttributeValue>
                    {
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdPreservation"},
                            Value = idPreservation
                        },
                        new DocumentAttributeValue
                        {
                            Attribute = new DocumentAttribute{ Name = "IdArchive"},
                            Value = idArchive
                        },
                    },
                Content = new DocumentContent(File.ReadAllBytes(fileIndiceXSLT)),
                Name = Path.GetFileName(fileIndiceXSLT)
            };
            using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName))
            {
                var res = (clientChannel as IDocuments).AddDocumentToChain(documentChiusura, idChain, DocumentContentFormat.Binary);
                preservation.IdDocumentCloseFile = res.IdDocument;
                idChain = res.DocumentParent.IdDocument;
            }
            using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName))
            {
                preservation.IdDocumentIndexFile = (clientChannel as IDocuments).AddDocumentToChain(documentIndice, idChain, DocumentContentFormat.Binary).IdDocument;
            }
            using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName))
            {
                preservation.IdDocumentIndexFileXML = (clientChannel as IDocuments).AddDocumentToChain(documentIndiceXML, idChain, DocumentContentFormat.Binary).IdDocument;
            }
            using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName))
            {
                preservation.IdDocumentIndexFileXSLT = (clientChannel as IDocuments).AddDocumentToChain(documentIndiceXSLT, idChain, DocumentContentFormat.Binary).IdDocument;
            }
            preservation.IdArchiveBiblosStore = archive.IdArchive;
            DbProvider.UpdatePreservationModifyField(preservation);
            return documentChiusura.Content.Blob.Length + documentIndice.Content.Blob.Length + documentIndiceXML.Content.Blob.Length + documentIndiceXSLT.Content.Blob.Length;
        }

        private bool ValidatePreservationTask(PreservationTask preservationTask, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (preservationTask.TaskType == null)
            {
                errorMessage = $"Il task: {preservationTask.IdPreservationTask} non ha una tipologia definita.";
                return false;
            }
            if (preservationTask.TaskType.Type != PreservationTaskTypes.Preservation && preservationTask.TaskType.Type != PreservationTaskTypes.Verify
                && preservationTask.TaskType.Type != PreservationTaskTypes.CloseAnnualPreservation)
            {
                errorMessage = $"Il task: {preservationTask.IdPreservationTask} non è di una tipologia processabile, è possibile eseguire solamente task di tipo 'Preservation', 'Verify' o 'CloseAnnualPreservation'.";
                return false;
            }
            if (preservationTask.Archive == null)
            {
                errorMessage = $"Il task: {preservationTask.IdPreservationTask} non ha un archivio definito.";
                return false;
            }
            if (!preservationTask.StartDocumentDate.HasValue || !preservationTask.EndDocumentDate.HasValue)
            {
                errorMessage = $"Il task: {preservationTask.IdPreservationTask} non ha definite la data di inizio e fine conservazione.";
                return false;
            }
            return true;
        }

        private void UpdateCompanyTemplates(Company company, ArchiveCompany archiveCompany)
        {
            if (string.IsNullOrEmpty(company.TemplateCloseFile) || string.IsNullOrEmpty(archiveCompany.TemplateXSLTFile))
            {
                if (string.IsNullOrEmpty(company.TemplateCloseFile))
                {
                    company.TemplateCloseFile = Resources.CHIUSURA;
                    DbProvider.UpdateCompany(company);
                }

                if (string.IsNullOrEmpty(archiveCompany.TemplateXSLTFile))
                {
                    archiveCompany.TemplateXSLTFile = Resources.INDICE;
                    DbProvider.UpdateArchiveCompany(archiveCompany);
                }
            }
        }

        private Guid GetOrCreateIdPreservationByTask(PreservationTask preservationTask)
        {
            if (preservationTask.TaskType.Type == PreservationTaskTypes.Verify)
            {
                Guid preservationId = GetIdPreservationByVerifyTask(preservationTask);
                if (preservationId == Guid.Empty)
                {
                    return DbProvider.CreatePreservation(preservationTask);
                }
            }
            return GetIdPreservationByPreservationTask(preservationTask);
        }

        private Guid GetIdPreservationByVerifyTask(PreservationTask preservationTask)
        {
            if (preservationTask.TaskType.Type != PreservationTaskTypes.Verify)
            {
                throw new Generic_Exception($"Il task {preservationTask.IdPreservationTask} che si sta cercando di processare non è di tipo Verify");
            }

            Guid currentTaskPreservationId = DbProvider.GetPreservationIdByTask(preservationTask.IdPreservationTask).GetValueOrDefault();
            if (currentTaskPreservationId == Guid.Empty)
            {
                ICollection<PreservationTask> previousTasksToExecute = DbProvider.PreviousTaskToExecute(preservationTask, preservationTask.Archive.IdArchive);
                if (previousTasksToExecute.Count > 0)
                {
                    new PreservationError($"Ci sono task da eseguire antecedenti il task corrente. Eseguire prima i tasks: {string.Join(",", previousTasksToExecute.Select(x => string.Format("Task Dal {0:dd/MM/yyyy} al {1:dd/MM/yyyy}", x.StartDocumentDate, x.EndDocumentDate)))}", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }
            }
            return currentTaskPreservationId;
        }

        private Guid GetIdPreservationByPreservationTask(PreservationTask preservationTask)
        {
            if (preservationTask.TaskType.Type != PreservationTaskTypes.Preservation
                && preservationTask.TaskType.Type != PreservationTaskTypes.CloseAnnualPreservation)
            {
                throw new Generic_Exception($"Il task {preservationTask.IdPreservationTask} che si sta cercando di processare non è di tipo Preservation");
            }

            if (preservationTask.IdPreservation.HasValue && preservationTask.IdPreservation.Value != Guid.Empty)
            {
                return preservationTask.IdPreservation.Value;
            }

            if (!preservationTask.IdCorrelatedPreservationTask.HasValue)
            {
                new PreservationError("Nessun task di verifica associato al task di conservazione. Contattare il riferiemnto tecnico.", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
            }
            PreservationTask verifyPreservationTask = DbProvider.GetPreservationTask(preservationTask.IdCorrelatedPreservationTask.Value, false);
            if (verifyPreservationTask == null)
            {
                new PreservationError("Nessun task di verifica trovato. Eseguire il task di verifica per proseguire.", PreservationErrorCode.E_PRESERVATION_VERIFY_EX).ThrowsAsFaultException();
            }
            if (!verifyPreservationTask.Executed)
            {
                new PreservationError("Il Task di verifica non risulta essere stato eseguito. Eseguire il task di verifica per proseguire.", PreservationErrorCode.E_PRESERVATION_VERIFY_EX).ThrowsAsFaultException();
            }
            if (verifyPreservationTask.HasError)
            {
                new PreservationError("Il Task di verifica risulta essere stato eseguito con i seguenti errori: " + verifyPreservationTask.ErrorMessages, PreservationErrorCode.E_PRESERVATION_VERIFY_EX).ThrowsAsFaultException();
            }
            return verifyPreservationTask.IdPreservation.GetValueOrDefault();
        }

        private PreservationInfoResponse InitializePreservationDocuments(PreservationTask preservationTask, Guid idPreservation, ArchiveConfiguration archiveConfiguration)
        {
            PreservationInfoResponse availableDocumentResponse = DbProvider.GetAvailableDocumentDateForPreservation(preservationTask.Archive, preservationTask, idPreservation, archiveConfiguration.PreservationLimitTaskToDocumentDate);
            if (!availableDocumentResponse.HasPendingDocument)
            {
                new PreservationError("Nessun documento disponibile per la conservazione.", PreservationErrorCode.E_NO_DOCUMENT_EX).ThrowsAsFaultException();
            }

            if (availableDocumentResponse.StartDocumentDate.GetValueOrDefault() == DateTime.MinValue || availableDocumentResponse.EndDocumentDate.GetValueOrDefault() == DateTime.MinValue)
            {
                new PreservationError("Date per la conservazione non valide.", PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
            }

            if (availableDocumentResponse.StartDocumentDate.Value.Year < availableDocumentResponse.EndDocumentDate.Value.Year)
            {
                new PreservationError("Sono presenti documenti con anno inferiore all'anno della conservazione corrente. Eseguire una conservazione di chiusura anno.", PreservationErrorCode.E_EXIST_NO_CONSERVATED_DOCUMENT).ThrowsAsFaultException();
            }

            if (availableDocumentResponse.StartDocumentDate.Value.Year != availableDocumentResponse.EndDocumentDate.Value.Year)
            {
                new PreservationError($"Attenzione, ci sono dei documenti con data cross anno. Le conservazioni vanno effettuate su documenti dello stesso anno. Eseguire una conservazione sui documenti dell'anno {availableDocumentResponse.StartDocumentDate:yyyy}, e se il problema persiste contattare il riferimento tecnico.", PreservationErrorCode.E_EXIST_NO_CONSERVATED_DOCUMENT).ThrowsAsFaultException();
            }

            var preservationDate = DbProvider.GetLastPreservedDate(preservationTask.Archive.IdArchive, idPreservation);
            if (preservationTask.TaskType.Type == PreservationTaskTypes.CloseAnnualPreservation || 
                (preservationTask.CorrelatedTasks != null && preservationTask.CorrelatedTasks.Any(x => x.TaskType.Type == PreservationTaskTypes.CloseAnnualPreservation)))
            {
                preservationDate = preservationTask.StartDocumentDate;
            }
            
            if (preservationDate.HasValue && availableDocumentResponse.StartDocumentDate.Value < preservationDate.GetValueOrDefault())
            {
                new PreservationError("Sono presenti documenti con data inferiore all'ultima conservazione. Contattare il riferimento tecnico.", PreservationErrorCode.E_EXIST_NO_CONSERVATED_DOCUMENT).ThrowsAsFaultException();
            }
            return availableDocumentResponse;
        }

        public string GetAzurePreservationStorage(DocumentArchive archive, Company company, PreservationTask preservationTask, Objects.Preservation preservation)
        {
            throw new NotImplementedException("La conservazione su Azure Storage non è abilitata");
            //logger.Debug("CreatePreservation - Checking Preservation_SingleVHD...");
            //bool singleVHD = false;
            //bool.TryParse(AzureService.GetSettingValue("Preservation_SingleVHD"), out singleVHD);
            //logger.DebugFormat("CreatePreservation - Preservation_SingleVHD:{0}", singleVHD);
            //if (!singleVHD)
            //    preservationOnDiskSize = DbProvider.GetPreservationTotalSizeOnDisk(archive.IdArchive, currentPreservation.Path);

            //var documentSize = currentPreservation.Documents.Sum(x => x.Size);
            //long txtfileSize = 5000 * currentPreservation.Documents.Count();//500 is the extimed index file row size
            //logger.InfoFormat("documentSize:{0}, txtfileSize:{1}", documentSize, txtfileSize);
            //documentSize = documentSize.GetValueOrDefault() + txtfileSize;
            //logger.InfoFormat("TotalSize:{0}", documentSize);
            //Pulse(METHOD_NAME, string.Format("Verifica delle dimensioni. Totale dati conservati: {0}, totale della conservazione corrente: {1}", preservationOnDiskSize, documentSize), 20);
            //logger.InfoFormat("Preservation total size: {0}, this Documents size: {1}", preservationOnDiskSize, documentSize);
            //if (!PreservationFileManager.CheckSize(preservationOnDiskSize + documentSize.GetValueOrDefault(), AzureService.IsAvailable))
            //{
            //    int preservationEnum = 1;
            //    if (int.TryParse(archive.PathPreservation, out preservationEnum))
            //    {
            //        logger.WarnFormat("archive.PathPreservation non conforme alle specifiche. {0}", archive.PathPreservation);
            //    }
            //    archive.PathPreservation = preservationEnum.ToString();
            //    DbProvider.UpdateArchivePathPreservation(archive);
            //}
            //fileManager = new PreservationFileManager(archive, company);
            ////Chech sui requisiti del file manager per la conservazione
            //fileManager.Check((int)(preservationOnDiskSize + documentSize.GetValueOrDefault()));
            //return fileManager.CheckPreservationDirectory(currentPreservation, verifyOnly);
        }

        public void MoveDocumentToPreservationFolder(string folderPath, Document document, IDictionary<Guid, BindingList<DocumentAttributeValue>> fullDocumentAttributes)
        {
            logger.Debug($"MoveDocumentToPreservationFolder - Copia documento {document.IdDocument} in {folderPath}");
            try
            {
                BiblosStorageService.StorageService storageService = new BiblosStorageService.StorageService();
                BindingList<DocumentAttributeValue> docAttributes = fullDocumentAttributes[document.IdDocument];
                var uniqueKey = PurgeFileName(docAttributes,
                    string.IsNullOrEmpty(document.PrimaryKeyValue) ? document.PreservationIndex.GetValueOrDefault().ToString() : document.PrimaryKeyValue);

                var fileName = $"{uniqueKey}{Path.GetExtension(document.Name)}";
                var finalPathName = Path.Combine(folderPath, fileName);
                if (!File.Exists(finalPathName))
                {
                    storageService.CopyDocumentTo(document, finalPathName);
                }
                document.DocumentHash = UtilityService.GetHash(File.ReadAllBytes(finalPathName), true);
                DbProvider.UpdateDocumentPreservationReference(document.IdDocument, finalPathName, document.DocumentHash);
                logger.Debug("MoveDocumentToPreservationFolder - Copia terminata correttamente");
            }
            catch (Exception ex)
            {
                logger.Error($"E' avvenuto un errore durante lo spostamento del documento {document.IdDocument}", ex);
                throw ex;
            }
        }

        public static List<Objects.Preservation> PreservationToClose(Guid idCompany)
        {
            return DbProvider.PreservationToClose(idCompany);
        }

        public string PreservationDocumentFileName(Objects.Document document)
        {
            return PurgeFileName(document.AttributeValues, string.IsNullOrEmpty(document.PrimaryKeyValue) ? document.PreservationIndex.GetValueOrDefault().ToString() : document.PrimaryKeyValue);
        }

        public BindingList<Objects.Preservation> ArchivePreservationClosedInDate(Guid idArchive, DateTime dateFrom, DateTime dateTo)
        {
            return DbProvider.ArchivePreservationClosedInDate(idArchive, dateFrom, dateTo);
        }

        public BindingList<Objects.ArchiveCompany> GetArchiveCompanies(Guid idArchive)
        {
            return DbProvider.GetArchiveCompanies(idArchive);
        }

        public BindingList<Objects.Document> GetPreservationDocumentsToPurge(Guid idPreservation)
        {
            return DbProvider.GetPreservationDocumentsToPurge(idPreservation);
        }

        public int CountPreservationDocumentsToPurge(Guid idPreservation)
        {
            return DbProvider.CountPreservationDocumentsToPurge(idPreservation);
        }

        public ICollection<Guid> GetIdPreservationsInStorageDevice(Guid idStorageDevice)
        {
            return DbProvider.GetIdPreservationsInStorageDevice(idStorageDevice);
        }

        public void SavePreservationTaskStatus(PreservationTask task, Objects.Enums.PreservationTaskStatus taskStatus, bool hasError, string errorMessage)
        {
            DbProvider.SavePreservationTaskStatus(task, taskStatus, hasError, errorMessage);
        }

        public bool ExistPreservationsByArchive(DocumentArchive archive)
        {
            return DbProvider.ExistPreservationsByArchive(archive.IdArchive);
        }

        public string BuildPreservationIndexXSL(DocumentArchive archive)
        {
            ICollection<DocumentAttribute> preservationAttributes = AttributeService.GetAttributesFromArchive(archive.IdArchive)
                .Where(x => x.ConservationPosition > 0)
                .OrderBy(o => o.ConservationPosition)
                .ToList();
            string template = Resources.IndiceXSLTemplate;

            StringBuilder headerBuilder = new StringBuilder();
            StringBuilder bodyBuilder = new StringBuilder();
            string attributeName;
            foreach (DocumentAttribute preservationAttribute in preservationAttributes)
            {
                attributeName = string.IsNullOrEmpty(preservationAttribute.Description) ? preservationAttribute.Name : preservationAttribute.Description;
                headerBuilder.Append($"<td>{attributeName}</td>");
                bodyBuilder.Append($"<td>"+
                    "<xsl:choose>"+
                      $"<xsl:when test=\"count(Attributo[@Nome='{attributeName}']/child::node()) = 1\">"+
                        $"<xsl:value-of select=\"Attributo[@Nome='{attributeName}']\" />"+
                      "</xsl:when>"+
                      "<xsl:otherwise>"+
                        "<xsl:text disable-output-escaping=\"yes\">&amp;nbsp;</xsl:text>"+
                      "</xsl:otherwise>"+
                    "</xsl:choose>"+
                  "</td>");
            }
            return template.Replace("<%ATTRIBUTES_HEADER%>", headerBuilder.ToString()).Replace("<%ATTRIBUTES_BODY%>", bodyBuilder.ToString());
        }

        public string GetAwardBatchXSL()
        {
            return Resources.AwardBatch;
        }
    }
}
