using BiblosDS.Library.Common.DB;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Preservation.IpdaDoc;
using BiblosDS.Library.Common.Preservation.ObjectsXml;
using BiblosDS.Library.Common.Preservation.ServiceReferenceStorage;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using BiblosDS.WCF.DigitalSign;
using log4net;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using VecompSoftware.BiblosDS.Model.Parameters;
using VecompSoftware.BiblosDS.WCF.Common;

namespace BiblosDS.Library.Common.Preservation.Services
{
    public partial class PreservationService : ServiceBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PreservationService));
        private const string TESTO_HASH_INDICE = "Evidenza informatica file indice (formato Hex): ";
        private const string TESTO_HASH_INDICE_XML = "Evidenza informatica file indice XML (formato Hex): ";
        private const string TESTO_HASH_INDICE_XSL = "Evidenza informatica foglio di stile di visualizzazione (formato Hex): ";
        private const string TESTO_SEPARATORE = "-------------------------------------------------------------------------------------------------------------------------";

        public List<string> ErrorMessages = new List<string>();  // lista dei messaggi di errore 
        public string VerifyFile = "";
        public Ipda verifiedIpda = null;
        public bool PulseHighFrequencyEnabled = false;

        public event EventHandler<PulseEventArgs> OnPulse;

        public PreservationService() : base()
        {

        }

        public Guid GetIdPreservationArchiveFromName(string archiveName)
        {
            logger.InfoFormat("GetIdPreservationArchiveFromName - nome archivio {0}", archiveName);

            Guid ret = DbProvider.GetIdPreservationArchiveFromName(archiveName);

            logger.Info("GetIdPreservationArchiveFromName - ritorno al chiamante");

            return ret;
        }

        public Guid GetIdPreservationExceptionFromDescription(string exceptionName)
        {
            logger.InfoFormat("GetIdPreservationExceptionFromDescription - nome eccezione {0}", exceptionName);

            Guid ret = DbProvider.GetIdPreservationExceptionFromDescription(exceptionName);

            logger.Info("GetIdPreservationExceptionFromDescription - ritorno al chiamante");

            return ret;
        }

        public Guid GetIdPreservationRoleFromKeyCode(int keyCode)
        {
            logger.InfoFormat("GetIdPreservationRoleFromKeyCode - keycode {0}", keyCode);

            Guid ret = DbProvider.GetIdPreservationRoleFromKeyCode(keyCode);

            logger.Info("GetIdPreservationRoleFromKeyCode - ritorno al chiamante");

            return ret;
        }

        public bool MustUseSHA256Mark()
        {
            return ("True".Equals(DbProvider.GetFirstPreservationParameter("UsaFirmaSHA256"), StringComparison.InvariantCultureIgnoreCase));
        }

        public BindingList<PreservationArchiveInfoResponse> GetPreservationArchives(string domainUserName)
        {
            logger.InfoFormat("GetPreservationArchives - utente {0}", domainUserName);

            BindingList<PreservationArchiveInfoResponse> ret = ArchiveService.GetLegalArchives(domainUserName, null);

            logger.Info("GetPreservationArchives - ritorno al chiamante");

            return ret;
        }

        public BindingList<DocumentArchive> GetArchivesWithPreservations()
        {
            logger.InfoFormat("GetArchivesWithPreservations - entry point");

            BindingList<DocumentArchive> ret = DbProvider.GetArchivesWithPreservations();

            logger.Info("GetArchivesWithPreservations - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationUserRole> GetUserRoleInArchive(string domainUserName, Guid idArchive)
        {
            logger.InfoFormat("GetUserRoleInArchive - utente {0} id archivio {1}", domainUserName, idArchive);

            BindingList<PreservationUserRole> ret = DbProvider.GetUserRoleInArchive(domainUserName, idArchive);

            logger.Info("GetUserRoleInArchive - ritorno al chiamante");

            return ret;
        }

        public Dictionary<string, string> GetPreservationParameter(Guid idArchive)
        {
            logger.InfoFormat("GetPreservationParameter - id archivio {0}", idArchive);

            Dictionary<string, string> ret = DbProvider.GetPreservationParameter(idArchive);

            logger.Info("GetPreservationParameter - ritorno al chiamante");

            return ret;
        }

        public string GetFirstPreservationParameter(string paramKey)
        {
            logger.InfoFormat("GetFirstPreservationParameter - label parametro {0}", paramKey);

            var ret = DbProvider.GetFirstPreservationParameter(paramKey);

            logger.Info("GetFirstPreservationParameter - ritorno al chiamante");

            return ret;
        }

        //TODO: Serve nella pagina ArchiveCompany
        public BindingList<Company> GetCompanies()
        {
            logger.InfoFormat("GetCompanies");

            BindingList<Company> ret = DbProvider.GetCompanies();

            logger.Info("GetCompanies - ritorno al chiamante");

            return ret;
        }

        public List<Company> GetCustomerCompanies(string idCustomer)
        {
            try
            {
                return DbProvider.GetCustomerCompanies(idCustomer);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }

        public Company GetCompany(Guid IdCompany)
        {
            logger.InfoFormat("GetCompany");

            Company ret = DbProvider.GetCompany(IdCompany);

            logger.Info("GetCompany - ritorno al chiamante");

            return ret;
        }

        public bool IsUserInRole(Guid idArchive, string domainUserName, Guid idRole)
        {
            logger.InfoFormat("IsUserInRole - id archivio {0} utente {1} id ruolo {2}", idArchive, domainUserName,
                              idRole);

            bool ret = DbProvider.IsUserInRole(idArchive, domainUserName, idRole);

            logger.Info("IsUserInRole - ritorno al chiamante");

            return ret;
        }

        public bool IsUserInRole(string domainUserName, Guid idRole)
        {
            logger.InfoFormat("IsUserInRole - utente {0} id ruolo {1}", domainUserName, idRole);

            bool ret = DbProvider.IsUserInRole(domainUserName, idRole);

            logger.Info("IsUserInRole - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTaskGroup> GetPreservationTaskGroup(string archiveName)
        {
            logger.InfoFormat("GetPreservationTaskGroup - nome archivio {0}", archiveName);

            DocumentArchive archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new FaultException("Nessun archivio presente con il nome: " + archiveName);

            BindingList<PreservationTaskGroup> ret = DbProvider.GetTaskGroupList(archive.IdArchive);

            logger.Info("GetPreservationTaskGroup - ritorno al chiamante");

            return ret;
        }

        public void SetTaskGroupClosed(Guid idTaskGroup, Guid idArchive)
        {
            logger.InfoFormat("SetTaskGroupClosed - id gruppo task {0} id archivio {1}", idTaskGroup, idArchive);

            DbProvider.SetTaskGroupClosed(idTaskGroup, idArchive);

            logger.Info("SetTaskGroupClosed - ritorno al chiamante");
        }

        public Guid GetScheduleFromTaskGroup(Guid idTaskGroup)
        {
            logger.InfoFormat("GetScheduleFromTaskGroup - id gruppo task {0}", idTaskGroup);

            Guid ret = DbProvider.GetScheduleFromTaskGroup(idTaskGroup);

            logger.Info("GetScheduleFromTaskGroup - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationSchedule> GetSchedule(Guid? idSchedule = null)
        {
            logger.InfoFormat("GetSchedule - id scadenziario {0}",
                              (idSchedule.HasValue ? idSchedule.Value.ToString() : null));

            BindingList<PreservationSchedule> ret = DbProvider.GetSchedule(idSchedule);

            logger.Info("GetSchedule - ritorno al chiamante");

            return ret;
        }

        public Guid GetNextPreservationId(Guid idArchive, string domainUser, Guid idTaskGroup, DateTime startDate,
                                          DateTime endDate)
        {
            logger.InfoFormat(
                "GetNextPreservationId - id archivio {0} utente {1} id gruppo task {2} data inizio {3} data fine {4}",
                idArchive, domainUser, idTaskGroup, startDate, endDate);

            Guid ret = DbProvider.GetNextPreservationId(idArchive, domainUser, idTaskGroup, startDate, endDate);

            logger.Info("GetNextPreservationId - ritorno al chiamante");

            return ret;
        }

        public PreservationFileInfoResponse GetPreservationClosingFileInfo(Guid preservationId)
        {
            logger.InfoFormat("GetPreservationClosingFileInfo - id conservazione {0}", preservationId);
            Objects.Preservation pres = DbProvider.GetPreservation(preservationId, false);
            if (pres == null)
                throw new FaultException("Nessuna conservazione trovata con id " + preservationId + ". Contattare il riferimento tecnico.");
            if (string.IsNullOrEmpty(pres.Path))
                throw new FaultException("Nessuna directory temporanea definita. Contattare il riferimento tecnico.");
            string sFileNameTXT = pres.Path + "\\CHIUSURA_" + pres.Label + ".txt";
            var ret = new PreservationFileInfoResponse
            {
                File = File.ReadAllBytes(sFileNameTXT),
                FileName = "CHIUSURA_" + pres.Label + ".txt",
                InfoCamereFileExtension = InfoCamereFileExtension(pres.Archive.IdArchive),
                DigitalSignFileExtension = DigitalSignFileExtension(pres.Archive.IdArchive)
            };

            logger.Info("GetPreservationClosingFileInfo - ritorno al chiamante.");

            return ret;
        }

        public string InfoCamereFileExtension(Guid idArchive)
        {
            logger.InfoFormat("InfoCamereFileExtension - id archivio {0}", idArchive);

            string icExt = DbProvider.GetPreservationParameter(idArchive, "ExtMarcTempInfoCamere", true);

            if (icExt == null || icExt.Length == 0)
                return ".m7m";
            if (!icExt.StartsWith(".")) icExt = icExt.Insert(0, ".");

            logger.Info("InfoCamereFileExtension - ritorno al chiamante");

            return icExt;
        }

        public string DigitalSignFileExtension(Guid idArchive)
        {
            logger.InfoFormat("DigitalSignFileExtension - id archivio {0}", idArchive);

            string dsExt = DbProvider.GetPreservationParameter(idArchive, "ExtMarcTempDigitalSign", true);

            if (dsExt == null || dsExt.Length == 0)
                return ".x7m";
            if (!dsExt.StartsWith(".")) dsExt = dsExt.Insert(0, ".");

            logger.Info("DigitalSignFileExtension - ritorno al chiamante");

            return dsExt;
        }

        public PreservationInfoResponse CreatePreservation(Guid idArchive, string domainUser, Guid idGruppoTask, DateTime dataInizio, DateTime dataFine, bool verifyOnly)
        {
            logger.InfoFormat("CreatePreservation - idArchive:{0}, domainUser:{1}, idGruppoTask:{2}, dataInizio:{3}, dataFine:{4}, verifyOnly:{5}", idArchive, domainUser, idGruppoTask, dataInizio, dataFine, verifyOnly);
            bool persistVerifyPreservation = ConfigurationManager.AppSettings["PersistVerifyPreservation"] != null && ConfigurationManager.AppSettings["PersistVerifyPreservation"].ToString().ToLower() == "true";
            var exceptions = "";
            var preservationId = Guid.Empty;
            const string METHOD_NAME = "CreatePreservation";
            var result = new PreservationInfoResponse();
            var archive = ArchiveService.GetArchive(idArchive);
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(archive.PreservationConfiguration);
            var useSHA256 = MustUseSHA256Mark();
            var workingDir = archive.PathPreservation;
            if (string.IsNullOrEmpty(workingDir))
            {
                new PreservationError("Nessuna directory definita per la conservazione. Contattare il riferimento tecnico.", PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
            }
            else
            {
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }
            }
            Objects.Preservation pres = null;
            try
            {
                Pulse(METHOD_NAME, "Recupero ID conservazione.", 1);
                preservationId = GetNextPreservationId(idArchive, domainUser, idGruppoTask, dataInizio, dataFine);
                Pulse(METHOD_NAME, "Recupero ID conservazione.", 10);
                if (persistVerifyPreservation)
                {
                    if (DbProvider.CheckPreservationVerify(preservationId))
                        new PreservationError("Verifica della conservazione in corso di verifica. Eliminare la verifica per proseguire.", PreservationErrorCode.E_PRESERVATION_VERIFY_EX).ThrowsAsFaultException();
                }
                result.IdPreservation = preservationId;

                Pulse(METHOD_NAME, "Reset delle conservazioni precedentemente salvate.", 11);
                ResetPreparedPreservation(preservationId);
                Pulse(METHOD_NAME, "Reset delle conservazioni precedentemente salvate.", 20);

                Pulse(METHOD_NAME, "Recupero documenti da conservare.", 30);
                var documents = FindPreparedPreservationObjects(dataInizio, dataFine, idArchive);
                Pulse(METHOD_NAME, "Recupero documenti da conservare.", 40);

                if (documents.Count() < 1)
                {
                    result.Documents = new BindingList<Document>();
                    result.Error = null;
                    result.TotalRecords = 0;
                    logger.InfoFormat("{0} - ritorno al chiamante.", METHOD_NAME);

                    return result;
                }

                Pulse(METHOD_NAME, "", 50);
                var errors = DbProvider.UpdateDocumentsPreservation(preservationId, archive, documents, archiveConfiguration.VerifyPreservationDateEnabled);
                if (errors.Count() > 0)
                {
                    exceptions = string.Join(Environment.NewLine, errors);
                    new PreservationError(Environment.NewLine + exceptions, PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }
                Pulse(METHOD_NAME, "", 60);


                //foreach (var doc in documents)
                //{
                //    WritePreservationDocument(doc.IdDocument, preservationId);
                //}

                Pulse(METHOD_NAME, "Verifica eccezioni.", 61);
                Objects.Preservation tmpPres;
                if (!CheckPreservationExceptions(preservationId, out errors, documents, out tmpPres, archiveConfiguration))
                {
                    //Nel caso di eccezione non va cancellata la conservazione                    
                    exceptions = string.Join(Environment.NewLine, errors.Take(20));
                    logger.Debug(string.Join(Environment.NewLine, errors));
                    new PreservationError(exceptions, PreservationErrorCode.E_PRESERVATION_EX).ThrowsAsFaultException();
                }

                Pulse(METHOD_NAME, "Verifica eccezioni.", 70);

                Pulse(METHOD_NAME, "Recupero conservazione.", 80);
                pres = DbProvider.GetPreservation(preservationId, true);
                Pulse(METHOD_NAME, "Recupero conservazione.", 90);

                //JOURNALING
                Pulse(METHOD_NAME, "Scrittura journal", 1);

                var journal = new PreservationJournaling
                {
                    DateActivity = DateTime.Now,
                    DateCreated = DateTime.Now,
                    DomainUser = domainUser,
                    Preservation = pres,
                    PreservationJournalingActivity =
                                    DbProvider.GetPreservationJournalingActivities(null).Where(x =>
                                    x.KeyCode.Equals("CreazioneConservazione", StringComparison.InvariantCultureIgnoreCase)).Single(),
                };

                Pulse(METHOD_NAME, "Scrittura journal", 91);
                AddPreservationJournaling(journal);
                Pulse(METHOD_NAME, "Scrittura journal", 100);

                Pulse(METHOD_NAME, "Recupero nome conservazione.", 1);
                var preservationName = GetPreservationName(pres);

                pres.Name = preservationName;

                pres.Path = verifyOnly ? Path.Combine(workingDir, "Temp", preservationName) : Path.Combine(workingDir, preservationName);

                Pulse(METHOD_NAME, "Recupero nome conservazione.", 20);

                var sDirectory = pres.Path;

                try
                {
                    if (Directory.Exists(sDirectory))
                        Directory.Delete(sDirectory, true);

                    Directory.CreateDirectory(sDirectory);
                }
                catch (Exception ex)
                {
                    new PreservationError(ex, PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
                }

                var sFileNameTxt = sDirectory + "\\INDICE_" + preservationName + ".txt";

                Thread.BeginCriticalRegion();

                try
                {
                    var progress = 1;
                    Pulse(METHOD_NAME, "Copia documenti conservati.", progress);
                    //Comunica che sta per iniziare il processo di copia.
                    decimal percentage;
                    //int count = 0;
                    for (int i = 0; i < documents.Count; i++)
                    {
                        //TODO lasciare l'update solo quà
                        //count += 1;
                        //doc.PreservationIndex = count;
                        var doc = documents[i];
                        var sHlp = CopyPreservationObject(sDirectory, doc, preservationId, useSHA256);
                        percentage = ((decimal)progress / documents.Count) * 100.0m;
                        Pulse(METHOD_NAME, "Copia documenti conservati (" + i + " di " + documents.Count() + ").", (int)Math.Ceiling(percentage));

                        if (sHlp.Length > 0)
                            new PreservationError(doc.PrimaryKeyValue + " - " + sHlp, PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                        progress++;
                    }

                    Thread.EndCriticalRegion();
                }
                catch
                {
                    Thread.EndCriticalRegion();
                    throw;
                }

                pres.Documents = documents;

                Pulse(METHOD_NAME, "Copia documenti conservati.", 100);

                Pulse(METHOD_NAME, "Creazione file indice.", 1);
                CreatePreservationIndexFile(pres, sFileNameTxt);
                Pulse(METHOD_NAME, "Creazione file indice.", 100);

                //string sRet = CopyCompEDViewerIndexFile(pres, workingDir);
                Pulse(METHOD_NAME, "Copia VIEWER CompEd.", 1);
                var sRet = CopyCompEDViewerIndexFile(pres, sDirectory);
                Pulse(METHOD_NAME, "Copia VIEWER CompEd.", 100);

                if (sRet.Length > 0)
                {
                    new PreservationError("Errore Creazione Viewer " + sRet).ThrowsAsFaultException();
                }

                //sRet = CreatePreservationClosingFile(preservationId, workingDir, exceptions);
                Pulse(METHOD_NAME, "Creazione file di chiusura.", 1);
                sRet = CreatePreservationClosingFile(pres, sDirectory, exceptions);
                Pulse(METHOD_NAME, "Creazione file di chiusura.", 100);

                if (sRet.Length > 0)
                {
                    new PreservationError("Errore Creazione File Chiusura " + sRet).ThrowsAsFaultException();
                }


                /*
                 * Ora la chiusura di una conservazione viene fatta esplicitamente con la chiamata al metodo remoto "ClosePreservation".
                 */
                //this.Pulse(METHOD_NAME, "Chiusura conservazione.", 1);
                //ClosePreservation(pres);
                //this.Pulse(METHOD_NAME, "Chiusura conservazione.", 100);

                DbProvider.UpdatePreservation(pres, true);

                result.IdPreservation = preservationId;
                result.Documents = documents;
                result.TotalRecords = documents.Count();
                if (persistVerifyPreservation)
                {
                    DbProvider.SavePreservationVerify(preservationId, null);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Pulse(METHOD_NAME, "", 1);
                    if (preservationId != Guid.Empty)
                    {
                        Pulse(METHOD_NAME, "Annullamento conservazione.", 1);
                        try
                        {
                            if (persistVerifyPreservation)
                            {
                                if (ex is FaultException<ResponseError> && (ex as FaultException<ResponseError>).Detail.ErrorCode == (int)PreservationErrorCode.E_PRESERVATION_VERIFY_EX)
                                {
                                    logger.InfoFormat("Preservation in VERIFY: {0}", preservationId);
                                }
                                else
                                    DbProvider.SavePreservationVerify(preservationId, ex);
                            }
                            else
                                AbortPreservation(preservationId);
                        }
                        catch (Exception abortError)
                        {
                            logger.Fatal(abortError);
                        }
                        Pulse(METHOD_NAME, "Annullamento conservazione.", 100);
                    }
                }
                finally
                {
                    if (ex is FaultException<PreservationError>)
                    {
                        logger.Warn(ex);
                        result.Error = (ex as FaultException<PreservationError>).Detail;
                    }
                    else
                    {
                        logger.Error(ex);
                        result.Error = new PreservationError(ex.Message, PreservationErrorCode.E_SYSTEM_EXCEPTION);
                    }
                }
            }
            result.Documents = null;
            logger.InfoFormat("{0} - ritorno al chiamante.", METHOD_NAME);

            return result;
        }

        public void ResetPreparedPreservation(Guid idPreservation)
        {
            logger.InfoFormat("ResetPreparedPreservation - id conservazione {0}", idPreservation);

            DbProvider.ResetPreparedPreservation(idPreservation);

            logger.Info("ResetPreparedPreservation - ritorno al chiamante");
        }

        public BindingList<Document> FindPreparedPreservationObjects(DateTime dateFrom, DateTime dateTo, Guid idArchive)
        {
            logger.InfoFormat("FindPreparedPreservationObjects - data DA {0} data A {1} id archivio {2}", dateFrom,
                              dateTo, idArchive);

            BindingList<Document> ret = DbProvider.FindPreparedPreservationObjects(dateFrom, dateTo, idArchive);

            logger.Info("FindPreparedPreservationObjects - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationException(Guid idPreservationException, Guid idDocument)
        {
            logger.InfoFormat("UpdatePreservationException - id eccezione {0} id documento {1}", idPreservationException,
                              idDocument);

            DbProvider.UpdatePreservationException(idPreservationException, idDocument);

            logger.Info("UpdatePreservationException - ritorno al chiamante");
        }

        private void UpdatePreservationExceptionForAllDocuments(Guid idPreservationException, Guid idPreservation)
        {
            logger.InfoFormat("UpdatePreservationExceptionForAllDocuments - id eccezione {0} id conservazione {1}",
                              idPreservationException, idPreservation);

            DbProvider.UpdatePreservationExceptionForAllDocuments(idPreservationException, idPreservation);

            logger.Info("UpdatePreservationExceptionForAllDocuments - ritorno al chiamante");
        }

        public string CopyPreservationObject(string folderPath, Guid idDocument, Guid idPreservation, bool useSHA256)
        {
            logger.InfoFormat("CopyPreservationObject - folder {0} id documento {1} id conservazione {2}", folderPath,
                              idDocument, idPreservation);

            string ret = CopyPreservationObject(folderPath, DbProvider.GetDocument(idDocument), idPreservation, useSHA256);

            logger.Info("CopyPreservationObject - ritorno al chiamante");

            return ret;
        }

        [Obsolete("Utilizzare il metodo MoveDocumentToPreservationFolder")]
        public string CopyPreservationObject(string folderPath, Document doc, Guid idPreservation, bool useSHA256)
        {
            logger.InfoFormat("CopyPreservationObject - folder {0} id conservazione {1}", folderPath, idPreservation);

            try
            {
                Document document;
                using (var svc = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>("ServiceDocumentStorage"))
                {
                    document = (svc as IServiceDocumentStorage).GetDocument(doc);

                    if (document == null)
                        throw new Exception(string.Format("Errore in \"{0}\": il documento con ID \"{1}\" non e' stato trovato.", "CopyPreservationObject", doc.IdDocument));
                }

                var uniqueKey = PurgeFileName(doc.AttributeValues,
                    string.IsNullOrEmpty(doc.PrimaryKeyValue) ? doc.PreservationIndex.GetValueOrDefault().ToString() : doc.PrimaryKeyValue);
                //var destinationFolder = archive.PathPreservation;
                var destinationFolder = folderPath;

                var cu = uniqueKey; //PurgeFileName(doc.AttributeValues, uniqueKey);

                var sFileName = string.Format("{0}{1}", cu, Path.GetExtension(doc.Name));
                var finalPathName = Path.Combine(destinationFolder, sFileName);
                document.PreservationName = sFileName;
                //if (File.Exists(finalPathName))
                //    return string.Empty;

                var buf = document.Content.Blob;

                if (!File.Exists(finalPathName))
                {
                    using (var fs = new FileStream(finalPathName, FileMode.Create))
                    {
                        fs.Write(buf, 0, buf.Length);
                        fs.Close();
                    }
                }

                //Verificare che l'hash corrisponda
                //// hash
                //                var sha = new SHA1CryptoServiceProvider();
                //                var result = sha.ComputeHash(buf);
                //                //byte[] result = sha.ComputeHash(Convert.ToBase64String(buf));
                //                string hexValue = "";

                //                for (int i = 0; i < result.Length; i++)
                //                    hexValue += result[i].ToString("X2");

                //                //doc.IdPreservation = idPreservation;

                //                ////if (string.IsNullOrEmpty(document.DocumentHash))
                //                doc.DocumentHash = hexValue;
                doc.DocumentHash = UtilityService.GetHash(buf, useSHA256);

                //if (string.IsNullOrEmpty(doc.Name))
                //    doc.Name = sFileName;

                //DbProvider.UpdateDocument(doc);

                logger.Info("CopyPreservationObject - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                if (ex.Message.IndexOf("Verifica FALSE") != -1)
                {
                    throw new ApplicationException(string.Format("L' impronta hash del documento con chiave univoca '{0}' non corrisponde al documento stesso.", doc.PrimaryKeyValue));
                }

                throw ex;
            }
        }

        private string PurgeFileName(BindingList<DocumentAttributeValue> attributes, string uniqueKey)
        {
            try
            {
                if (!attributes.Any(x => x.Attribute.KeyOrder.GetValueOrDefault() > 0) && attributes.Any(x => x.Attribute.Name.ToLower() == "filename"))
                {
                    uniqueKey = string.Format("{0}_{1}", uniqueKey, attributes.First(x => x.Attribute.Name.ToLower() == "filename").Value);
                }
                var chiaveU = uniqueKey.ToCharArray();
                for (var i = 0; i < uniqueKey.Length; i++)
                    if (!(chiaveU[i] >= '0' && chiaveU[i] <= '9') &&
                        !(chiaveU[i] >= 'a' && chiaveU[i] <= 'z') &&
                        !(chiaveU[i] >= 'A' && chiaveU[i] <= 'Z'))
                        chiaveU[i] = '-';
                var cu = new string(chiaveU);
                return cu;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public string GetPreservationName(Guid preservationId)
        {
            logger.InfoFormat("GetPreservationName - id conservazione {0}", preservationId);

            Objects.Preservation pres = DbProvider.GetPreservation(preservationId, false);
            string ret = GetPreservationName(pres);

            logger.Info("GetPreservationName - ritorno al chiamante");

            return ret;
        }

        public string GetPreservationName(Objects.Preservation pres)
        {
            logger.InfoFormat("GetPreservationName - entry point con conservazione gia' recuperata");

            string retval = string.Empty;
            DateTime maxDate, minDate;

            if (pres != null)
            {
                DocumentArchive archive = DbProvider.GetArchive(pres.IdArchive);
                string tipoDoc = archive.FiscalDocumentType;

                //Dictionary<string, string> configParameter = DbProvider.GetPreservationParameter(pres.IdArchive);
                //string tipoDoc = configParameter.GetValue("TipoDocumento");

                if (string.IsNullOrEmpty(tipoDoc))
                    tipoDoc = pres.Archive != null ? pres.Archive.Name : null;

                if (tipoDoc == null)
                    tipoDoc = string.Empty;

                //Normalizzazione Nome
                tipoDoc = tipoDoc.Replace(" ", "");
                tipoDoc = tipoDoc.Replace(":", "");
                tipoDoc = tipoDoc.Replace(@"\", "");
                tipoDoc = tipoDoc.Replace("/", "");
                tipoDoc = tipoDoc.Replace("*", "");
                tipoDoc = tipoDoc.Replace("?", "");
                tipoDoc = tipoDoc.Replace("\"", "");
                tipoDoc = tipoDoc.Replace("<", "");
                tipoDoc = tipoDoc.Replace(">", "");
                tipoDoc = tipoDoc.Replace("|", "");

                minDate = (pres.StartDate.HasValue) ? pres.StartDate.Value : DateTime.MinValue;
                maxDate = (pres.EndDate.HasValue) ? pres.EndDate.Value : DateTime.MaxValue;

                retval = string.Format("{0}_Dal_{1}_al_{2}", tipoDoc, minDate.ToString("dd-MM-yyyy"),
                                       maxDate.ToString("dd-MM-yyyy"));
            }

            logger.InfoFormat("GetPreservationName - ritorno al chiamante con valore {0}", retval);

            return retval;
        }

        public void MarkPreservationAsSigned(Guid idPreservation, byte[] signedFile, byte[] timeStampFile)
        {
            logger.InfoFormat("MarkPreservationAsSigned - id conservazione {0}", idPreservation);

            try
            {
                Objects.Preservation preserv = DbProvider.GetPreservation(idPreservation, false);
                if (preserv != null)
                {
                    string preservationName = GetPreservationName(preserv);
                    string closedFileName = "CHIUSURA_" + preservationName + ".txt";
                    string fileName = Path.Combine(preserv.Path, preservationName);
                    File.WriteAllBytes(fileName + InfoCamereFileExtension(preserv.Archive.IdArchive), timeStampFile);
                    File.WriteAllBytes(fileName + DigitalSignFileExtension(preserv.Archive.IdArchive), signedFile);

                    preserv.LastVerifiedDate = DateTime.Now;
                    if (preserv.Task != null)
                        preserv.Task.ExecutedDate = DateTime.Now;
                    DbProvider.UpdatePreservation(preserv, false);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                if (ex is FaultException)
                    throw ex;
                else
                    throw new FaultException(ex.Message);
            }

            logger.Info("MarkPreservationAsSigned - ritorno al chiamante");
        }

        public Objects.Preservation ClosePreservation(Guid idPreservation)
        {
            logger.InfoFormat("ClosePreservation - id conservazione {0}", idPreservation);

            Objects.Preservation ret = ClosePreservation(GetPreservation(idPreservation, false));

            logger.Info("ClosePreservation - ritorno al chiamante");

            return ret;
        }

        public Objects.Preservation ClosePreservation(Objects.Preservation preservation)
        {
            logger.InfoFormat("ClosePreservation - entry point con conservazione gia' recuperata");

            try
            {
                if (preservation == null)
                    new PreservationError("Nessuna conservazione passata come parametro",
                                          PreservationErrorCode.E_INVALID_PARAMS)
                        .ThrowsAsFaultException();

                if (preservation.Archive == null)
                    new PreservationError(
                        "Nessun archivio associato alla conservazione con id " + preservation.IdPreservation,
                        PreservationErrorCode.E_INVALID_PARAMS)
                        .ThrowsAsFaultException();

                string workingDir = preservation.Path;
                string nomePres = GetPreservationName(preservation);

                logger.InfoFormat("ClosePreservation: working Dir {0} e nomePres {1}", workingDir, nomePres);
                string PreservationName = string.Empty;

                if (string.IsNullOrEmpty(workingDir) || !Directory.Exists(preservation.Path))
                {
                    logger.Info("ClosePreservation Directory non trovata");
                    workingDir = preservation.Archive.PathPreservation;

                    if (string.IsNullOrEmpty(workingDir))
                        new PreservationError(
                            "Percorso di conservazione non configurato per l'archivio " + preservation.Archive.Name ??
                            preservation.Archive.IdArchive.ToString(),
                            PreservationErrorCode.E_SYSTEM_EXCEPTION)
                            .ThrowsAsFaultException();

                    if (!string.IsNullOrEmpty(nomePres))
                    {
                        nomePres = Path.Combine(workingDir, nomePres);

                        if (!Directory.Exists(nomePres))
                        {
                            PreservationName = GetPreservationName(preservation);
                            nomePres = Path.Combine(workingDir, PreservationName);
                        }
                    }
                    else
                    {
                        PreservationName = GetPreservationName(preservation);
                        nomePres = Path.Combine(workingDir, PreservationName);
                    }

                    workingDir = nomePres;

                    if (!Directory.Exists(nomePres))
                    {
                        new PreservationError(
                            "Non esiste alcuna directory per la conservazione che corrisponde sotto il percorso " +
                            workingDir,
                            PreservationErrorCode.E_SYSTEM_EXCEPTION)
                            .ThrowsAsFaultException();
                    }
                }
                logger.Info("ClosePreservation: set CloseDate");
                preservation.CloseDate = DateTime.Now;
                //preservation.Path = Path.Combine(preservation.Archive.PathPreservation, GetPreservationName(preservation));
                preservation.Label = nomePres;
                preservation.Path = workingDir;

                //Journaling.
                logger.Info("Scrittura Journalings INIT");
                PreservationJournalingActivity activity = DbProvider.GetPreservationJournalingActivities(null)
                    .Where(x => x.KeyCode.Equals("ChiusuraConservazione", StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
                if (activity == null)
                    new PreservationError(
                        "Errore di configurazione. Nessun entry configurata in \"PreservationJournalingActivity\" per l'attività:  ChiusuraConservazione",
                        PreservationErrorCode.E_SYSTEM_EXCEPTION)
                        .ThrowsAsFaultException();

                //preservation.PreservationJournalings.Add(new PreservationJournaling
                //                                             {
                //                                               IdPreservationJournaling = Guid.NewGuid(),
                //                                               DateActivity = DateTime.Now,
                //                                               DateCreated = DateTime.Now,
                //                                               DomainUser = preservation.User.DomainUser,
                //                                               User =
                //                                                   new PreservationUser
                //                                                       {
                //                                                         IdPreservationUser =
                //                                                             preservation.User.IdPreservationUser
                //                                                       },
                //                                               IdPreservationJournalingActivity =
                //                                                   activity.IdPreservationJournalingActivity,
                //                                               Preservation = preservation,
                //                                             });
                DbProvider.AddPreservationJournaling(new PreservationJournaling { IdPreservation = preservation.IdPreservation, PreservationJournalingActivity = activity, DateActivity = DateTime.Now, DateCreated = DateTime.Now, DomainUser = preservation.User.DomainUser });
                logger.Info("Scrittura Journalings END");
                logger.Info("UpdatePreservation INIT");
                DbProvider.UpdatePreservation(preservation, false);
                logger.Info("UpdatePreservation END");

                logger.Info("ClosePreservation - ritorno al chiamante");

                return preservation;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                if (ex is FaultException)
                    throw ex;
                else
                    throw new FaultException(ex.Message);
            }
        }

        public PreservationInfoResponse GetPreservationInfo(Guid idArchive, Guid? idTaskGroup)
        {
            try
            {
                logger.InfoFormat("GetPreservationInfo - id archivio {0} id gruppo task {1}", idArchive,
                            (idTaskGroup.HasValue ? idTaskGroup.Value.ToString() : null));

                var response = new PreservationInfoResponse();

                /* Commentato poiché "GetPreservationDateByTask" lavora già in base ai documenti presenti. */
                //Pulse("GetPreservationInfo", "Verifica documenti in archivio", 1);
                //response.HasPendingDocument = DbProvider.GetIfExistDocumentNotPreservedFromArchive(idArchive);

                Pulse("GetPreservationInfo", "Recupero date conservazione associate al gruppo task.", 1);
                response = DbProvider.GetPreservationDateByTask(idArchive, idTaskGroup);

                response.DocumentType = DbProvider.GetPreservationParameter(idArchive, "TipoDocumento", true);

                logger.Info("GetPreservationInfo - ritorno al chiamante");

                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                if (ex is FaultException)
                    throw ex;
                else
                    throw new FaultException(ex.Message);
            }
        }

        public PreservationUser GetPreservationUserForArchive(Guid idArchive, string domainUser)
        {
            logger.InfoFormat("GetPreservationUserForArchive - id archivio {0} utente {1}", idArchive, domainUser);

            PreservationUser ret = DbProvider.GetPreservationUserForArchive(idArchive, domainUser);

            logger.Info("GetPreservationUserForArchive - ritorno al chiamante");

            return ret;
        }

        public int GetSelectedDocumentsNumber(Guid idArchive, DateTime startDate, DateTime endDate)
        {
            logger.InfoFormat("GetSelectedDocumentsNumber - id archivio {0} data inizio {1} data fine {2}", idArchive,
                              startDate, endDate);

            var documents = FindPreparedPreservationObjects(startDate, endDate, idArchive);

            logger.InfoFormat("GetSelectedDocumentsNumber - ritorno al chiamante con valore {0}", documents.Count());

            return documents.Count();
        }

        public BindingList<DocumentAttribute> GetAttributes(Guid idArchive)
        {
            logger.InfoFormat("GetAttributes - id archivio {0}", idArchive);

            //TODO: Perchè non usare "DbProvider.GetAttributesFromArchive" ?

            BindingList<DocumentAttribute> ret = DbProvider.GetAttributes(idArchive);

            logger.Info("GetAttributes - ritorno al chiamante");

            return ret;
        }

        public string CopyCompEDViewerIndexFile(Guid idPreservation, string workingDirectory)
        {
            logger.InfoFormat("CopyCompEDViewerIndexFile - id conservazione {0} work dir {1}", idPreservation,
                              workingDirectory);

            if (idPreservation == Guid.Empty)
                throw new Exception("Non è stato passato alcun ID del documento conservato.");

            string ret = CopyCompEDViewerIndexFile(GetPreservation(idPreservation, false), workingDirectory);

            logger.InfoFormat("CopyCompEDViewerIndexFile - ritorno al chiamante con valore {0}", ret);

            return ret;
        }

        private string CopyCompEDViewerIndexFile(Objects.Preservation preservation, string workingDirectory)
        {
            logger.InfoFormat("CopyCompEDViewerIndexFile - work dir {0}", workingDirectory);

            try
            {
                if (string.IsNullOrEmpty(workingDirectory) || !Directory.Exists(workingDirectory))
                    throw new Exception(string.Format("Il parametro {0} e' errato. Contattare l'amministratore di sistema.", "workingDirectory"));

                if (preservation == null)
                    throw new Exception(string.Format("Non esiste alcun documento corrispondente all'id \"{0}\"", preservation.IdPreservation));

                var configParameter = DbProvider.GetPreservationParameter(preservation.IdArchive);
                //TODO: MIGLIORARE IL CHECK DEGLI ERRORI!

                //var sDocType = configParameter.GetValue("TipoDocumento");

                var sDirectoryTemplate = configParameter.GetValue("DirectoryTemplate");
                //if (RoleEnvironment.IsAvailable)
                //{
                //    CloudDriveManager manager = new CloudDriveManager();
                //    sDirectoryTemplate = manager.GetMountedDrive().FirstOrDefault(x => x.Value.LocalPath.EndsWith(preservation.Archive.PathPreservation)).Key;                    
                //}

                var sDirectory = GetPreservationName(preservation);
                var sTemplateFile = Path.Combine(sDirectoryTemplate, "INDICE.xsl");
                //string sFileName = workingDirectory + "\\" + sDirectory + "\\INDICE_" + sDirectory + ".xsl";
                var sFileName = Path.Combine(workingDirectory, "INDICE_" + sDirectory + ".xsl");

                if (!File.Exists(sTemplateFile))
                {
                    return string.Format("Non esiste il template per il Viewer contattare l'amministratore di sistema:\n{0}", sTemplateFile);
                }

                var srTemplate = new StreamReader(sTemplateFile);
                var sStream = srTemplate.ReadToEnd();
                srTemplate.Close();

                var sr = File.CreateText(sFileName);
                sr.Write(sStream);
                sr.Flush();
                sr.Close();

                logger.Info("CopyCompEDViewerIndexFile - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message;
            }
        }

        public BindingList<PreservationHoliday> GetHolidays(Guid? idPreservationHolidays)
        {
            logger.InfoFormat("GetHolidays - id festivita {0}",
                              (idPreservationHolidays.HasValue ? idPreservationHolidays.Value.ToString() : null));

            BindingList<PreservationHoliday> ret = DbProvider.GetPreservationHolidays(idPreservationHolidays);

            logger.Info("GetHolidays - ritorno al chiamante");

            return ret;
        }

        public void CreatePreservationIndexFile(Guid idPreservation, string fileName)
        {
            logger.InfoFormat("CreatePreservationIndexFile - id conservazione {0} file {1}", idPreservation, fileName);

            if (idPreservation != Guid.Empty)
                CreatePreservationIndexFile(GetPreservation(idPreservation, true), fileName);

            logger.Info("CreatePreservationIndexFile - ritorno al chiamante");
        }

        private void CreatePreservationIndexFile(Objects.Preservation preservation, string fileName)
        {
            logger.InfoFormat("CreatePreservationIndexFile - file {0}", fileName);

            const long maxValueLenght = 100;
            const string sHlp = "Impronta file SHA1";

            if (string.IsNullOrEmpty(fileName))
                return;

            if (preservation == null)
                return;

            var fileInfo = new FileInfo(fileName);


            //string sCmd = "SELECT ISNULL(MAX(LEN([Hash])),0) AS [HASH], ISNULL(MAX(LEN(NomeFileInArchivio)),0) AS [FILE] FROM Oggetto_Conservazione WHERE IdConservazione = " + IdConservazione.ToString();

            var docDestinationFolder = (preservation.Path ??
                                        ((preservation.Archive != null)
                                             ? preservation.Archive.PathPreservation
                                             : string.Empty));
            //long lenFile = docDestinationFolder.Length + preservation.Documents.Select(x => (x.Name ?? string.Empty).Length).Max() + fileInfo.Extension.Length;
            long lenFile = docDestinationFolder.Length + preservation.Documents.Select(x => string.Format("{0}{1}", x.PrimaryKeyValue, Path.GetExtension(x.Name)).Length).Max();
            if (lenFile < 0)
                lenFile = 0;

            long lenHash = preservation.Documents.Select(x => (x.DocumentHash ?? string.Empty).Length).Max();
            if (lenHash < 0)
                lenHash = 0;

            //A che server????
            //string sFormat = "{" + nCount + "} ";

            ////var tmp = attValues
            ////        .Where(x => x.Value != null);
            //List<Document> documents = new List<Document>();
            //BindingList<Document> tmpDocuments;
            //int totalDocuments = 0;
            //int skip = 0;
            //while ((tmpDocuments = DbProvider.GetDocumentsInPreservation(preservation.IdPreservation, 10, skip * 10, out totalDocuments)).Count() > 0)
            //{
            //    documents.AddRange(tmpDocuments);
            //    skip += 1;
            //}

            var dtCd = CreaDataTableColumnsDimensions(lenFile);

            var attributes = DbProvider.GetAttributesFromArchive(preservation.IdArchive);

            foreach (var attr in attributes.OrderBy(x => x.ConservationPosition))
            {
                PopolaDataTableColumnsDimensions(preservation, maxValueLenght, attr, dtCd);
            }

            var myRow = dtCd.NewRow();
            myRow["Attributo"] = "ImprontaFileSHA1";
            myRow["Lunghezza"] = sHlp.Length > lenHash ? sHlp.Length : lenHash;

            dtCd.Rows.Add(myRow);


            //nCount++;
            //sFormat += "{" + nCount + "}";

            //var fileNameXML = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(fileInfo.Extension, string.Empty) + ".xml");
            //var fileNameXsl = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(fileInfo.Extension, string.Empty) + ".xsl");

            var fileNameXML = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(fileInfo.Extension, ".xml"));
            var fileNameXsl = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(fileInfo.Extension, ".xsl"));

            var nIndex = fileNameXsl.LastIndexOf("\\");
            if (nIndex > 0 && (nIndex < fileNameXsl.Length - 1))
                fileNameXsl = fileNameXsl.Substring(nIndex + 1);

            var sw = new StreamWriter(fileName);
            var swXML = new StreamWriter(fileNameXML);

            swXML.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
            swXML.Write("<?xml-stylesheet type=\"text/xsl\" href=\"" + fileNameXsl + "\"?>\r\n");
            swXML.Write("<Indice>\r\n");


            //Scrivo la riga d'intestazione intestazioni delle righe
            var sHlp2 = CalcolaIntestazione(dtCd);
            sw.WriteLine(sHlp2);

            ////Selezione dati
            //sCmd = "SELECT OC.IdOggetto,"
            //    + "			OC.[Hash] AS ImprontaFileSHA1,"
            //    + "			OC.Progressivo,"
            //    + "			OC.NomeFileInArchivio,"
            //    + "			VAO.NomeAttributoOggetto,"
            //    + "			VAO.Valore"
            //    + " FROM"
            //    + "			ValoreAttributoOggetto VAO"
            //    + "			INNER JOIN AttributoOggetto AO ON  AO.Nome = VAO.NomeAttributoOggetto"
            //    + "			INNER JOIN Oggetto_Conservazione OC ON VAO.IdOggetto = OC.IdOggetto"
            //    + " WHERE"
            //    + "			AO.PosizioneInFileChiusura > 0"
            //    + "			AND OC.IdConservazione = " + IdConservazione.ToString()
            //    + "			ORDER BY OC.Progressivo, AO.PosizioneInFileChiusura";                    

            //
            var dtCv = new DataTable("ColumnsValuess");
            dtCv.Columns.Add("Attributo", typeof(string));
            dtCv.Columns.Add("Valore", typeof(string));

            foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
            {
                myRow = dtCv.NewRow();
                myRow["Attributo"] = "ImprontaFileSHA1";
                myRow["Valore"] = doc.DocumentHash;
                dtCv.Rows.Add(myRow);

                myRow = dtCv.NewRow();
                myRow["Attributo"] = "NomeFileInArchivio";
                myRow["Valore"] = string.Format("{0}{1}", PurgeFileName(doc.AttributeValues, string.IsNullOrEmpty(doc.PrimaryKeyValue) ? doc.PreservationIndex.GetValueOrDefault().ToString() : doc.PrimaryKeyValue), Path.GetExtension(doc.Name));
                dtCv.Rows.Add(myRow);

                BindingList<DocumentAttributeValue> docAttributes = DbProvider.GetFullDocumentAttributeValues(doc.IdDocument);
                foreach (DocumentAttribute attr in attributes)
                {
                    PopolaDataTableColumnsValues(dtCv, attr, docAttributes);
                }

                var sRow = GetNewTxtRow(doc.PreservationIndex.GetValueOrDefault().ToString(), dtCd, dtCv);

                var sRowXML = "<File Progressivo=\"" + doc.PreservationIndex.GetValueOrDefault() + "\">\r\n";
                sRowXML += GetNewXMLRow(dtCd, dtCv);
                sRowXML += "</File>";

                dtCv.Rows.Clear();
                sw.WriteLine(sRow);
                swXML.WriteLine(sRowXML);
            }

            swXML.Write("</Indice>\r\n");

            sw.Flush();
            swXML.Flush();

            sw.Close();
            swXML.Close();

            var hexValue = CalculateHashCode(fileName, MustUseSHA256Mark());

            //sCmd = "UPDATE ConservazioneSostitutiva"
            //    + "   SET FileIndiceHashHSA1='" + hexValue + "'"
            //    + " WHERE IdConservazione=" + IdConservazione.ToString();

            preservation.IndexHash = hexValue;
            DbProvider.UpdatePreservation(preservation, false);

            logger.Info("CreatePreservationIndexFile - ritorno al chiamante");
        }

        private static string CalculateHashCode(string fileName, bool useSHA256)
        {
            return UtilityService.GetHash(fileName, useSHA256);
        }

        private void PopolaDataTableColumnsValues(DataTable dtCv, DocumentAttribute attr, ICollection<DocumentAttributeValue> documentAttributes)
        {
            var currAttributeValue = documentAttributes.Where(x => x.IdAttribute == attr.IdAttribute).FirstOrDefault();

            if (currAttributeValue == null)
                return;

            if (currAttributeValue.Attribute == null)
                currAttributeValue.Attribute = attr;

            var myRow = dtCv.NewRow();
            myRow["Attributo"] = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description;
            //L'attributo va formattato in funzione della proprieta' "Format" dell'Attributo, se valorizzata.
            string valoreAttr;
            if (currAttributeValue.Attribute != null && !string.IsNullOrEmpty(currAttributeValue.Attribute.Format))
            {
                try
                {
                    valoreAttr = string.Format(currAttributeValue.Attribute.Format, currAttributeValue.Value);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    valoreAttr = string.Empty;
                }
            }
            else
            {
                valoreAttr = (currAttributeValue.Value != null) ? currAttributeValue.Value.ToString() : string.Empty;
            }

            myRow["Valore"] = valoreAttr;
            dtCv.Rows.Add(myRow);
        }

        private static string CalcolaIntestazione(DataTable dtCd)
        {
            int nLen;
            string sAttributo;
            string sHlp2 = "Num.    ";

            foreach (DataRow row in dtCd.Rows)
            {
                sAttributo = row["Attributo"].ToString();
                nLen = int.Parse(row["Lunghezza"].ToString());

                string sBlank = "";

                for (int i = 0; i < nLen; i++)
                    sBlank += " ";

                if (sAttributo.Length >= nLen)
                    sHlp2 += sAttributo.Substring(0, nLen) + " ";
                else
                    sHlp2 += sAttributo + sBlank.Substring(0, nLen - sAttributo.Length) + " ";
            }
            return sHlp2;
        }

        private static DataTable CreaDataTableColumnsDimensions(long lenFile)
        {
            const string sHlp = "File";
            var dtCd = new DataTable("ColumnsDimensions");

            dtCd.Columns.Add("Attributo", typeof(string));
            dtCd.Columns.Add("Lunghezza", typeof(long));

            var myRow = dtCd.NewRow();

            myRow["Attributo"] = "NomeFileInArchivio";
            myRow["Lunghezza"] = (sHlp.Length > lenFile) ? sHlp.Length : lenFile;

            dtCd.Rows.Add(myRow);
            return dtCd;
        }

        private void PopolaDataTableColumnsDimensions(Objects.Preservation preservation, long maxValueLenght, DocumentAttribute attr, DataTable dtCd)
        {
            //int nCount = 0;
            //nCount++;
            //sFormat += "{" + nCount + "} ";

            var sHlp = string.IsNullOrEmpty(attr.Description) ? attr.Name : attr.Description;
            var myRow = dtCd.NewRow();

            myRow["Attributo"] = sHlp;
            myRow["Lunghezza"] = 1000;
            //try
            //{
            //    var docsAttr = preservation.Documents.Where(x =>
            //                                                x.AttributeValues.Count() > 0 &&
            //                                                x.AttributeValues.Any(a => a.IdAttribute == attr.IdAttribute))
            //                                                .Select(x => x.AttributeValues.Where(a => a.Value != null).Select(a => a.Value.ToString().Length)
            //                                                        .Max()).OrderByDescending(x => x);

            //    myRow["Lunghezza"] = (sHlp.Length > docsAttr.FirstOrDefault())
            //                             ? sHlp.Length
            //                             : docsAttr.FirstOrDefault();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    myRow["Lunghezza"] = (sHlp.Length > maxValueLenght) ? sHlp.Length : maxValueLenght;
            //}

            dtCd.Rows.Add(myRow);
            return;
        }

        public string CreatePreservationClosingFile(Guid idPreservation, string workingDir, string exceptions)
        {
            logger.InfoFormat("CreatePreservationClosingFile - id conservazione {0} work dir {1} eccezioni {2}",
                              idPreservation, workingDir, exceptions);

            string ret = CreatePreservationClosingFile(GetPreservation(idPreservation, true), workingDir, exceptions);

            logger.InfoFormat("CreatePreservationClosingFile - ritorno al chiamante con valore {0}", ret);

            return ret;
        }


        private string CreatePreservationClosingFile(Objects.Preservation preservation, string workingDir, string exceptions)
        {
            logger.InfoFormat("CreatePreservationClosingFile - work dir {0} eccezioni {1}", workingDir, exceptions);

            try
            {
                if (string.IsNullOrEmpty(workingDir))
                    throw new Exception("Working directory non configurata correttamente.");

                if (preservation.Documents == null)
                    throw new Exception(string.Format("Nessun documento associato alla conservazione con id {0}", preservation.IdPreservation));

                if (exceptions == null)
                    exceptions = string.Empty;

                var user = preservation.User;
                var configParameter = DbProvider.GetPreservationParameter(preservation.IdArchive);
                var sCognomeNomeCf = string.Format("{0} {1} C.F. {2}", user.Name, user.Surname, user.FiscalId);
                var nDoc = preservation.Documents.Count();

                var dtHlp = preservation.Documents.Select(x => x.DateMain).Min();
                var sDataMinima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                dtHlp = preservation.Documents.Select(x => x.DateMain).Max();
                var sDataMassima = dtHlp.HasValue ? dtHlp.Value.ToString("dd/MM/yyyy") : "N.D.";

                //Recupero Parametri
                var sAzienda =
                    configParameter.Where(
                        x => x.Key.Equals("RagioneSociale", StringComparison.InvariantCultureIgnoreCase)).Select(
                            x => x.Value).SingleOrDefault();

                var sPartitaIva =
                    configParameter.Where(x => x.Key.Equals("PartitaIVA", StringComparison.InvariantCultureIgnoreCase)).
                        Select(x => x.Value).SingleOrDefault();

                var sDocType =
                    configParameter.Where(
                        x => x.Key.Equals("TipoDocumento", StringComparison.InvariantCultureIgnoreCase)).Select(
                            x => x.Value).SingleOrDefault();

                var sEvidenzaInformatica =
                    configParameter.Where(
                        x => x.Key.Equals("EvidenzaInformatica", StringComparison.InvariantCultureIgnoreCase)).Select(
                            x => x.Value).SingleOrDefault();

                var sDirectory = GetPreservationName(preservation);
                var sFileName = Path.Combine(workingDir, "CHIUSURA_" + sDirectory + ".txt");
                var sFileIndice = Path.Combine(workingDir, "INDICE_" + sDirectory + ".txt");
                var sFileIndiceXML = Path.Combine(workingDir, "INDICE_" + sDirectory + ".xml");
                var sFileIndiceXsl = Path.Combine(workingDir, "INDICE_" + sDirectory + ".xsl");

                var useSHA256 = MustUseSHA256Mark();

                var sFileIndiceNome = "INDICE_" + sDirectory + ".txt";
                var sHash = UtilityService.GetHash(sFileIndice, useSHA256);
                var sHashXML = UtilityService.GetHash(sFileIndiceXML, useSHA256);
                var sHashXsl = UtilityService.GetHash(sFileIndiceXsl, useSHA256);

                var sElencoCampi = "  - NomeFile\r\n";

                //oCommand.CommandText = "SELECT Nome FROM AttributoOggetto WHERE PosizioneInFileChiusura>0 ORDER BY PosizioneInFileChiusura";
                var attrs = DbProvider.GetAttributeByPreservationPosition(preservation.IdArchive);

                foreach (var item in attrs)
                    sElencoCampi += string.Format("  - {0}\r\n", item.Description);

                sElencoCampi += "  - Impronta SHA1 (formato Hex)\r\n";


                var groupsOutput = new StringBuilder(1000);
                Dictionary<string, List<long>> sectional = EstraiGruppi(preservation, configParameter);
                AddProcessedDoc(groupsOutput, sectional);

                var sDirectoryTemplate = configParameter.Where(x => x.Key == "DirectoryTemplate").Select(x => x.Value).SingleOrDefault();
                //if (RoleEnvironment.IsAvailable)
                //{
                //    CloudDriveManager manager = new CloudDriveManager();
                //    sDirectoryTemplate = manager.GetMountedDrive().FirstOrDefault(x => x.Value.LocalPath.EndsWith(preservation.Archive.PathPreservation)).Key;
                //}

                var sTemplateFile = sDirectoryTemplate + "\\CHIUSURA.txt";

                var srTemplate = new StreamReader(sTemplateFile);
                var sStream = srTemplate.ReadToEnd();
                srTemplate.Close();

                sStream = sStream.Replace("<%PERCORSO%>", sDirectory);
                sStream = sStream.Replace("<%FILE_INDICE%>", sFileIndiceNome);
                sStream = sStream.Replace("<%DATA_ORA%>", DateTime.Now.Date.ToString("d") + " alle " + DateTime.Now.ToString("HH:mm"));
                sStream = sStream.Replace("<%EVIDENZA_INFORMATICA%>", sEvidenzaInformatica);
                sStream = sStream.Replace("<%EVIDENZA_INDICE_TXT%>", sHash);
                sStream = sStream.Replace("<%EVIDENZA_INDICE_XML%>", sHashXML);
                sStream = sStream.Replace("<%EVIDENZA_INDICE_XSL%>", sHashXsl);
                sStream = sStream.Replace("<%AZIENDA%>", sAzienda);
                sStream = sStream.Replace("<%PIVA_AZIENDA%>", sPartitaIva);
                sStream = sStream.Replace("<%RESPONSABILE%>", sCognomeNomeCf);
                sStream = sStream.Replace("<%ARCHIVIO%>", preservation.Archive.Name);
                sStream = sStream.Replace("<%TIPO_DOCUMENTI%>", sDocType);
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

                DbProvider.UpdatePreservation(preservation, false);

                var sr = File.CreateText(sFileName);
                sr.Write(sStream);
                sr.Close();

                logger.Info("CreatePreservationClosingFile - ritorno al chiamante");

                return string.Empty;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return e.Message + " ===> " + e.Source;
            }
        }


        private static Dictionary<string, List<long>> EstraiGruppi_v1(Objects.Preservation preservation)
        {
            Dictionary<string, List<long>> sectional = new Dictionary<string, List<long>>();
            foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
            {
                if (sectional.ContainsKey(doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString()))
                    sectional[doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString()].Add(doc.AttributeValues.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault(false)).GetOrderAttributeValue());
                else
                    sectional.Add(doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString(), new List<long> { doc.AttributeValues.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault(false)).GetOrderAttributeValue() });
            }

            return sectional;
        }

        private static Dictionary<string, List<long>> EstraiGruppi(Objects.Preservation preservation, Dictionary<string, string> configParameter)
        {
            Dictionary<string, List<long>> sectional = new Dictionary<string, List<long>>();

            var grpProgrKey =
                configParameter.Where(
                    x => x.Key.Equals("GruppoProgressivoChiave", StringComparison.InvariantCultureIgnoreCase)).
                    Select(x => x.Value).SingleOrDefault();

            if (!string.IsNullOrEmpty(grpProgrKey))
            {
                var offsetGruppo = grpProgrKey;
                var offsetGruppoArr = offsetGruppo.Split('|');

                if (offsetGruppoArr.Length > 0 && (IsNumeric(offsetGruppoArr[offsetGruppoArr.Length - 1])))
                {
                    var groupWithParameters = offsetGruppoArr.Length != 1;


                    foreach (var doc in preservation.Documents.OrderBy(x => x.PreservationIndex))
                    {
                        if (sectional.ContainsKey(doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString()))
                            sectional[doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString()].Add(doc.AttributeValues.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault(false)).GetOrderAttributeValue());
                        else
                            sectional.Add(doc.AttributeValues.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).ToList().GetOrderAttributeValueString(), new List<long> { doc.AttributeValues.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault(false)).GetOrderAttributeValue() });
                    }
                }
            }

            return sectional;
        }

        private static void AddProcessedDoc(StringBuilder groupsOutput, Dictionary<string, List<long>> sectional)
        {
            if (sectional.Count() > 0)
                groupsOutput.Append("I documenti sono così suddivisi:" + Environment.NewLine);

            foreach (var item in sectional)
            {
                groupsOutput.Append("  L'elenco");
                groupsOutput.Append("  - gruppo <" + item.Key + ">");

                groupsOutput.Append(" comprende ");
                groupsOutput.Append(item.Value.Count());
                groupsOutput.Append(" documenti dal numero ");
                groupsOutput.Append(item.Value.Min());
                groupsOutput.Append(" al numero ");
                groupsOutput.Append(item.Value.Max());
                groupsOutput.Append(Environment.NewLine);

            }
        }

        //private static string AddProcessedDoc(Dictionary<string, string> configParameter, StringBuilder groupsOutput, Dictionary<string, List<long>> sectional)
        //{
        //    var uniqueKey = doc.PrimaryKeyValue.Substring(0, doc.PrimaryKeyValue.IndexOf('|', doc.PrimaryKeyValue.IndexOf('|', 0) + 1) + 1);

        //    processedDocs.Add(doc);

        //    if (groupWithParameters)
        //    {
        //        var key = "DescrizioneSezionale|" +
        //                  doc.PrimaryKeyValue.Substring(
        //            /* D { */doc.PrimaryKeyValue.IndexOf('|') /* } */+ 1,
        //            /*C {*/
        //                               doc.PrimaryKeyValue.IndexOf('|', /* B { */
        //                                                           doc.PrimaryKeyValue.IndexOf('|', 0)
        //            /* } */+ 1) /* } */
        //                               - doc.PrimaryKeyValue.IndexOf('|', 0) - 1);
        //        parameter = configParameter.Where(x => x.Key == key).Select(x => x.Value).SingleOrDefault();
        //    }

        //    if (!groupWithParameters &&
        //        !uniqueKey.Equals(uninqueKeyLast, StringComparison.InvariantCultureIgnoreCase)
        //        ||
        //        (   parameter != null && 
        //            groupWithParameters &&
        //            !parameter.Equals(parameterLast, StringComparison.InvariantCultureIgnoreCase)))
        //    {
        //        if (string.IsNullOrEmpty(uniqueKey))
        //            groupsOutput.Append("  L'elenco");
        //        else
        //            groupsOutput.Append("  - gruppo <" + uniqueKey.Replace("|", " ") + ">");

        //        if (!string.IsNullOrEmpty(parameter))
        //            groupsOutput.Append(" \"" + parameter + "\"");

        //        groupsOutput.Append(" comprende ");
        //        groupsOutput.Append(docsCounter);
        //        groupsOutput.Append(" documenti dal numero ");
        //        var cu = processedDocs.Select(x => x.PrimaryKeyValue).Min().Split('|');
        //        groupsOutput.Append(cu[cu.Length - 1]);
        //        groupsOutput.Append(" al numero ");
        //        cu = processedDocs.Select(x => x.PrimaryKeyValue).Max().Split('|');
        //        groupsOutput.Append(cu[cu.Length - 1] + ".");
        //        groupsOutput.Append(Environment.NewLine);

        //        uninqueKeyLast = uniqueKey;

        //        if (groupWithParameters)
        //            parameterLast = parameter;

        //        docsCounter = 0;
        //        processedDocs.Clear();
        //    }

        //    docsCounter++;
        //    processedDocs.Add(doc);
        //    return parameter;
        //}

        public BindingList<PreservationTaskGroup> GetTaskGroup(Guid? idPreservationTaskGroup = null)
        {
            logger.InfoFormat("GetTaskGroup - id gruppo task {0}",
                              (idPreservationTaskGroup.HasValue ? idPreservationTaskGroup.Value.ToString() : null));

            BindingList<PreservationTaskGroup> ret = DbProvider.GetTaskGroup(idPreservationTaskGroup);

            logger.Info("GetTaskGroup - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationAlert> GetPreservationAlert(Guid? idAlert, Guid? idTaskType, Guid? idAlertType,
                                                                   Guid? idTask, bool orderByOffset = false)
        {
            logger.InfoFormat(
                "GetPreservationAlert - id avviso {0} id tipo task {1} id tipo avviso {2} id task {3} ordinato {4}",
                (idAlert.HasValue ? idAlert.Value.ToString() : null),
                (idTaskType.HasValue ? idTaskType.Value.ToString() : null),
                (idAlertType.HasValue ? idAlertType.Value.ToString() : null),
                (idTask.HasValue ? idTask.Value.ToString() : null), orderByOffset);

            BindingList<PreservationAlert> ret = DbProvider.GetPreservationAlert(idAlert, idTaskType, idAlertType,
                                                                                 idTask, orderByOffset);

            logger.Info("GetPreservationAlert - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTaskGroupType> GetTaskGroupTypes(Guid? idTaskGroupType)
        {
            logger.InfoFormat("GetTaskGroupTypes - id gruppo task {0}",
                              (idTaskGroupType.HasValue ? idTaskGroupType.Value.ToString() : null));

            BindingList<PreservationTaskGroupType> ret = DbProvider.GetTaskGroupTypes(idTaskGroupType);

            logger.Info("GetTaskGroupTypes - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationUser> GetPreservationUser(Guid? idUser, Guid idArchive)
        {
            logger.InfoFormat("GetPreservationUser - id utente {0} id archivio {1}",
                              (idUser.HasValue ? idUser.Value.ToString() : null), idArchive);

            BindingList<PreservationUser> ret = DbProvider.GetPreservationUser(idUser, idArchive);

            logger.Info("GetPreservationUser - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTaskType> GetPreservationTaskTypes(Guid? idPreservationTaskType)
        {
            logger.InfoFormat("GetPreservationTaskTypes - id tipo task {0}", idPreservationTaskType);

            BindingList<PreservationTaskType> ret = DbProvider.GetPreservationTaskTypes(idPreservationTaskType);

            logger.Info("GetPreservationTaskTypes - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTask> GetTasksFromTaskGroup(Guid idTaskGroup)
        {
            logger.InfoFormat("GetTasksFromTaskGroup - id gruppo task {0}", idTaskGroup);

            BindingList<PreservationTask> ret = DbProvider.GetTasksFromTaskGroup(idTaskGroup);

            logger.Info("GetTasksFromTaskGroup - ritorno al chiamante");

            return ret;
        }

        public PreservationTaskGroup AddPreservationTaskGroup(PreservationTaskGroup toAdd, Guid idArchive)
        {
            try
            {
                logger.InfoFormat("AddPreservationTaskGroup - entry point - toAdd.EstimatedExpiry: {0}, toAdd.Expiry: {1}", toAdd.EstimatedExpiry, toAdd.Expiry);

                /*
                   IdPreservationTaskGroup = Guid.NewGuid(),
                IdPreservationTaskGroupType = toAdd.GroupType.IdPreservationTaskGroupType,
                Name = toAdd.Name,
                IdPreservationUser = toAdd.User.IdPreservationUser,
                IdPreservationSchedule = toAdd.Schedule.IdPreservationSchedule,
                Expiry = toAdd.Expiry,
                EstimatedExpiry = toAdd.EstimatedExpiry,
                IdArchive = idArchive,
                 * */

                //if (toAdd.Expiry <= new DateTime(1899, 12, 31))
                //{                
                PreservationTaskGroup tGroup = null;
                if (toAdd.IdPreservationTaskGroup != Guid.Empty)
                {
                    tGroup = GetTaskGroup(toAdd.IdPreservationTaskGroup)
                        .SingleOrDefault();
                }

                IOrderedEnumerable<PreservationTaskGroup> taskNotClosed = null;
                if (tGroup != null)
                {
                    taskNotClosed = DbProvider.GetTaskGroupNotClosed(idArchive, (toAdd.User != null) ? toAdd.User.IdPreservationUser : Guid.Empty)
                        .Where(x => x.EstimatedExpiry.HasValue && x.EstimatedExpiry.Value > tGroup.Expiry)
                        .OrderBy(x => x.EstimatedExpiry.Value); //Prende la data minima (il gruppo task piu' vecchio da chiudere)
                }
                else if (toAdd.EstimatedExpiry.HasValue)
                {
                    taskNotClosed = DbProvider.GetTaskGroupNotClosed(idArchive, (toAdd.User != null) ? toAdd.User.IdPreservationUser : Guid.Empty)
                        .Where(x => x.EstimatedExpiry.HasValue && x.EstimatedExpiry.Value > toAdd.EstimatedExpiry)
                        .OrderBy(x => x.EstimatedExpiry.Value); //Prende la data minima (il gruppo task piu' vecchio da chiudere)
                }

                if (taskNotClosed != null && taskNotClosed.Count() > 0)
                {
                    //Esiste un task non chiuso, inutile generarne uno nuovo
                    return taskNotClosed.First();
                }

                if (tGroup != null)
                {
                    //Verifico se esiste uno schedule di default.
                    var schedule = DbProvider.GetPreservationSchedule()
                        .Where(x => x.Default)
                        .SingleOrDefault();

                    if (schedule == null)
                    {
                        throw new Exception("Nessun PreservationSchedule di default configurato.");
                    }

                    toAdd.Expiry = GetProssimaScadenzaTeorica(schedule.FrequencyType, schedule.Period, tGroup.EstimatedExpiry.GetValueOrDefault());
                    logger.InfoFormat("AddPreservationTaskGroup {0} {1}", tGroup.EstimatedExpiry.GetValueOrDefault(), toAdd.Expiry);
                }
                //}

                PreservationTaskGroup ret = DbProvider.AddPreservationTaskGroup(toAdd, idArchive);

                logger.Info("AddPreservationTaskGroup - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw new FaultException(exx.Message);
            }
        }

        private static readonly char[] CARATTERI_SEPARATORI = new[] { '|' };

        private int[] CastPeriodicita(string periodicita)
        {
            var ret = new List<int>();
            string[] periodi = (periodicita ?? string.Empty).Trim().Split(CARATTERI_SEPARATORI, StringSplitOptions.RemoveEmptyEntries);

            if (periodi.Length < 1)
                throw new Exception("E' necessario indicare una periodicità per la tipologia di frequenza settimanale.");

            foreach (string periodo in periodi)
            {
                int res;
                if (!int.TryParse(periodo, out res))
                    throw new Exception("I valori indicati per la periodicità devono essere dei numeri interi.");

                ret.Add(res);
            }

            ret.Sort();

            return ret.ToArray();
        }

        public DateTime GetProssimaScadenzaTeorica(short frequenza, string periodicita, DateTime ultimaScadenzaTeorica)
        {
            logger.InfoFormat("GetProssimaScadenzaTeorica(frequenza:{0}, periodicita:{1}, ultimaScadenzaTeorica:{2}", frequenza, periodicita, ultimaScadenzaTeorica);
            DateTime proxScadenzaTeorica = ultimaScadenzaTeorica;

            switch (frequenza)
            {
                case 0: //cadenzata
                    int[] cadenza = this.CastPeriodicita(periodicita);
                    proxScadenzaTeorica = ultimaScadenzaTeorica.AddDays(cadenza[0]); // la periodicità cadenzata prevede un solo valore
                    break;
                case 1: //giornaliera
                    proxScadenzaTeorica = ultimaScadenzaTeorica.AddDays(1);
                    break;
                case 2: //settimanale
                    int[] weekDays = this.CastPeriodicita(periodicita);

                    int ultimaScadWeekDay = 0;

                    ultimaScadWeekDay = Convert.ToInt32(ultimaScadenzaTeorica.DayOfWeek); // giorno della settimana dell' ultima scadenza
                    foreach (int weekDay in weekDays)
                    {
                        if (weekDay >= 1 && weekDay <= 7)
                        {
                            if (ultimaScadWeekDay < weekDay)
                            {
                                proxScadenzaTeorica = ultimaScadenzaTeorica.AddDays(weekDay - ultimaScadWeekDay);
                                break;
                            }
                        }
                        else
                        {
                            throw new Exception("I valori di periodicità memorizzati nel db per la tipologia di frequenza settimanale non sono validi.");
                        }
                    }

                    if (weekDays[0] <= ultimaScadWeekDay)
                    {
                        // se uguale si considera comunque il successivo
                        proxScadenzaTeorica = ultimaScadenzaTeorica.AddDays(7 - ultimaScadWeekDay + weekDays[0]);
                    }
                    break;
                case 3: //mensile
                    int[] monthDays = this.CastPeriodicita(periodicita);
                    if (ultimaScadenzaTeorica.Month == 12) // è l'ultimo mese dell'anno?
                    {
                        proxScadenzaTeorica = new DateTime(ultimaScadenzaTeorica.Year + 1, 1, DateTime.DaysInMonth(ultimaScadenzaTeorica.Year + 1, 1));
                    }
                    else
                    {
                        proxScadenzaTeorica = new DateTime(ultimaScadenzaTeorica.Year, ultimaScadenzaTeorica.Month + 1, DateTime.DaysInMonth(ultimaScadenzaTeorica.Year, ultimaScadenzaTeorica.Month + 1));
                    }
                    break;
                case 4: // annuale
                    string[] yearDateTokens = periodicita.Split('|');

                    if (yearDateTokens.Length == 0) throw new Exception("Errore nella procedura di generazione della prossima scadenza. Nessuna periodicità definita per la tipologia annuale di questo scadenziario.");

                    foreach (string yearDateToken in yearDateTokens)
                    {
                        Exception periodExc = new Exception("Errore nella procedura di generazione della prossima scadenza. Periodicità definita per la tipologia annuale di questo scadenziario non valida.");
                        if (yearDateToken.Length != 4) throw periodExc;

                        DateTime yearDate = DateTime.MinValue;
                        try
                        {
                            yearDate = new DateTime(
                                ultimaScadenzaTeorica.Year,
                                int.Parse(yearDateToken.Substring(0, 2)),
                                int.Parse(yearDateToken.Substring(2, 2))
                                );
                        }
                        catch (Exception)
                        {
                            throw periodExc;
                        }

                        if (ultimaScadenzaTeorica < yearDate)
                        {
                            proxScadenzaTeorica = yearDate;
                            break;
                        }
                    }

                    if (proxScadenzaTeorica <= ultimaScadenzaTeorica)
                    {
                        // se uguale si considera comunque il successivo
                        proxScadenzaTeorica = new DateTime(
                            ultimaScadenzaTeorica.AddYears(1).Year,
                            int.Parse(yearDateTokens[0].Substring(0, 2)),
                            int.Parse(yearDateTokens[0].Substring(2, 2))
                            );
                    }
                    break;
            }

            if (proxScadenzaTeorica < ultimaScadenzaTeorica) throw new Exception("Errore nella procedura di generazione della prossima scadenza.");
            return proxScadenzaTeorica;
        }

        public PreservationTask AddPreservationTask(PreservationTask toAdd, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationTask - id archivio {0}", idArchive);

            PreservationTask ret = DbProvider.AddPreservationTask(toAdd, idArchive);

            logger.Info("AddPreservationTask - ritorno al chiamante");

            return ret;
        }

        public PreservationAlert AddPreservationAlert(PreservationAlert toAdd)
        {
            logger.Info("AddPreservationAlert - entry point");

            PreservationAlert ret = DbProvider.AddPreservationAlert(toAdd);

            logger.Info("AddPreservationAlert - ritorno al chiamante");

            return ret;
        }

        public Objects.Preservation GetPreservation(Guid idPreservation, bool fillAll = true)
        {
            logger.InfoFormat("GetPreservation - id conservazione {0}", idPreservation);

            Objects.Preservation ret = DbProvider.GetPreservation(idPreservation, fillAll);

            logger.Info("GetPreservation - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationUser> GetPreservationUsersInArchiveByRole(Guid idRole, string archiveName)
        {
            logger.InfoFormat("GetPreservationUsersInArchiveByRole - id ruolo {0} nome archivio {1}", idRole,
                              archiveName);

            BindingList<PreservationUser> ret = DbProvider.GetPreservationUsersInArchiveByRole(idRole,
                                                                                               GetIdPreservationArchiveFromName
                                                                                                   (archiveName));

            logger.Info("GetPreservationUsersInArchiveByRole - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationRole> GetRoles(Guid? idRole)
        {
            logger.InfoFormat("GetRoles - id ruolo {0}", (idRole.HasValue ? idRole.Value.ToString() : null));

            BindingList<PreservationRole> ret = DbProvider.GetRoles(idRole);

            logger.Info("GetRoles - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationUser(PreservationUser user, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationUser - id archivio {0}", idArchive);

            EntityProvider provider = DbProvider;

            using (DbTransaction tran = provider.BeginNoSave())
            {
                try
                {
                    provider.UpdatePreservationUser(user, idArchive);
                    provider.SaveChanges();

                    tran.Commit();
                }
                catch
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            logger.Info("UpdatePreservationUser - ritorno al chiamante");
        }

        public void DeletePreservationUserRolesByPreservationUser(Guid idPreservationUser)
        {
            logger.InfoFormat("DeletePreservationUserRolesByPreservationUser - id utente {0}", idPreservationUser);

            DbProvider.DeletePreservationUserRolesByPreservationUser(idPreservationUser);

            logger.Info("DeletePreservationUserRolesByPreservationUser - ritorno al chiamante");
        }

        public PreservationUser AddPreservationUser(PreservationUser user, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationUser - id archivio {0}", idArchive);

            var provider = new EntityProvider();
            PreservationUser ret = null;

            using (DbTransaction tran = provider.BeginNoSave())
            {
                try
                {
                    ret = provider.AddPreservationUser(user, idArchive);
                    provider.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            logger.Info("AddPreservationUser - ritorno al chiamante");

            return ret;
        }

        public void DeletePreservationUser(Guid idPreservationUser)
        {
            logger.InfoFormat("DeletePreservationUser - id utente {0}", idPreservationUser);

            EntityProvider provider = DbProvider;

            try
            {
                using (DbTransaction tran = provider.BeginNoSave())
                {
                    //provider.DeletePreservationUserRolesByPreservationUser(idPreservationUser);
                    //provider.DeletePreservationUser(idPreservationUser);

                    try
                    {
                        provider.DeletePreservationUser(idPreservationUser);

                        provider.SaveChanges();

                        tran.Commit();
                    }
                    catch
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw new FaultException(exx.Message);
            }

            logger.Info("DeletePreservationUser - ritorno al chiamante");
        }

        public PreservationUserRole AddPreservationUserRole(PreservationUserRole userRole, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationUserRole - id archivio {0}", idArchive);

            try
            {
                PreservationUserRole ret = DbProvider.AddPreservationUserRole(userRole, idArchive);

                logger.Info("AddPreservationUserRole - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);

                throw new FaultException(exx.Message);
            }
        }

        public void UpdatePreservationSchedule(PreservationSchedule sched, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationSchedule - id archivio {0}", idArchive);

            var provider = new EntityProvider();

            provider.BeginNoSaveNoTransaction();
            provider.UpdatePreservationSchedule(sched, idArchive);
            provider.SaveChanges();

            logger.Info("UpdatePreservationSchedule - ritorno al chiamante");
        }

        public void DeletePreservationSchedule(Guid idPreservationSchedule)
        {
            logger.InfoFormat("DeletePreservationSchedule - id scadenziario {0}", idPreservationSchedule);

            EntityProvider provider = DbProvider;

            using (DbTransaction tran = provider.BeginNoSave())
            {
                try
                {
                    provider.DeletePreservationSchedule(idPreservationSchedule);

                    provider.SaveChanges();

                    tran.Commit();
                }
                catch
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            logger.Info("DeletePreservationSchedule - ritorno al chiamante");
        }

        public PreservationSchedule AddPreservationSchedule(PreservationSchedule sched, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationSchedule - id archivio {0}", idArchive);

            EntityProvider provider = DbProvider;
            PreservationSchedule ret = null;

            using (DbTransaction tran = provider.BeginNoSave())
            {
                try
                {
                    ret = provider.AddPreservationSchedule(sched, idArchive);

                    provider.SaveChanges();

                    tran.Commit();
                }
                catch
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            logger.Info("AddPreservationSchedule - ritorno al chiamante");

            return ret;
        }

        public void DeletePreservationSchedule_TaskTypeBySchedule(Guid idPreservationSchedule)
        {
            logger.InfoFormat("DeletePreservationSchedule_TaskTypeBySchedule - id scadenziario {0}",
                              idPreservationSchedule);

            DbProvider.DeletePreservationSchedule_TaskTypeBySchedule(idPreservationSchedule);

            logger.Info("DeletePreservationSchedule_TaskTypeBySchedule - ritorno al chiamante");
        }

        public BindingList<PreservationTaskType> GetPreservationTaskTypesAndPreservationScheduleTaskTypes(
            Guid? idPreservationSchedule, Guid idArchive)
        {
            logger.InfoFormat(
                "GetPreservationTaskTypesAndPreservationScheduleTaskTypes - id scadenziario {0} id archivio {1}",
                (idPreservationSchedule.HasValue ? idPreservationSchedule.Value.ToString() : null), idArchive);

            BindingList<PreservationTaskType> items =
                DbProvider.GetPreservationTaskTypesAndPreservationScheduleTaskTypes(idPreservationSchedule, idArchive);

            foreach (PreservationTaskType item in items)
            {
                if (idPreservationSchedule.HasValue)
                    item.ScheduleTaskTypes =
                        new BindingList<PreservationScheduleTaskType>(
                            item.ScheduleTaskTypes.Where(x => x.IdPreservationSchedule == idPreservationSchedule.Value).
                                ToList());
            }

            logger.Info("GetPreservationTaskTypesAndPreservationScheduleTaskTypes - ritorno al chiamante");

            return items;
        }

        public BindingList<PreservationTaskType> GetTaskTypesByUserRole(Guid? idPreservationUserRole, Guid idArchive)
        {
            logger.InfoFormat("GetTaskTypesByUserRole - id ruolo utente {0} id archivio {1}",
                              (idPreservationUserRole.HasValue ? idPreservationUserRole.Value.ToString() : null),
                              idArchive);

            BindingList<PreservationTaskType> ret = DbProvider.GetTaskTypesByUserRole(idPreservationUserRole, idArchive);

            logger.Info("GetTaskTypesByUserRole - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationTaskType(PreservationTaskType taskType, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationTaskType - id archivio {0}", idArchive);

            DbProvider.UpdatePreservationTaskType(taskType, idArchive);

            logger.Info("UpdatePreservationTaskType - ritorno al chiamante");
        }

        public PreservationInfoResponse AbortPreservation(Guid idPreservation)
        {
            logger.InfoFormat("AbortPreservation - id conservazione {0}", idPreservation);

            var response = new PreservationInfoResponse();
            try
            {
                var preservation = GetPreservation(idPreservation, false);
                if (preservation == null)
                    new PreservationError("Impossibile trovare una conservazione con i parametri passati.", PreservationErrorCode.E_UNEXPECTED_RESULT).ThrowsAsFaultException();

                DbProvider.AbortPreservation(idPreservation);

                //string preservationPathName = Path.Combine(preservation.Archive.PathPreservation,GetPreservationName(preservation));
                string preservationPathName = preservation.Path;

                try
                {
                    if (!string.IsNullOrEmpty(preservationPathName) && Directory.Exists(preservationPathName))
                    {
                        Directory.Delete(preservationPathName, true);
                    }
                }
                catch
                {
                }
                response.StartDocumentDate = preservation.StartDate;
                response.EndDocumentDate = preservation.EndDate;
                response.IdPreservationTaskGroup = preservation.IdPreservationTaskGroup;
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                PreservationError toThrow;

                if (ex is FaultException)
                    toThrow = new PreservationError(ex.Message, PreservationErrorCode.E_SYSTEM_EXCEPTION);
                else
                    toThrow = new PreservationError(ex, PreservationErrorCode.E_SYSTEM_EXCEPTION);

                toThrow.ThrowsAsFaultException();
            }

            logger.Info("AbortPreservation - ritorno al chiamante");

            return response;
        }

        public bool CheckPreservationExceptions(Guid idPreservation, out List<string> exceptions, IEnumerable<Document> documents, out Objects.Preservation preservation)
        {
            DocumentArchive currentArchive = ArchiveService.GetArchiveFromPreservation(idPreservation);
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(currentArchive.PreservationConfiguration);
            return CheckPreservationExceptions(idPreservation, out exceptions, documents, out preservation, archiveConfiguration);
        }

        public bool CheckPreservationExceptions(Guid idPreservation, out List<string> exceptions, IEnumerable<Document> documents, out Objects.Preservation preservation, ArchiveConfiguration archiveConfiguration)
        {
            logger.InfoFormat("CheckPreservationExceptions - IdPreservation = {0}", idPreservation);

            const string METHOD_NAME = "CheckPreservationExceptions";

            exceptions = new List<string>();

            preservation = null;
            if (idPreservation == Guid.Empty)
            {
                exceptions.Add("Id conservazione non valido.");
                return false;
            }

            logger.Info("Recupero conservazione con id = " + idPreservation);

            Objects.Preservation pres = DbProvider.GetPreservation(idPreservation, false);

            if (pres == null)
            {
                exceptions.Add("Nessuna conservazione trovata con id " + idPreservation);
                return false;
            }

            DocumentArchive archive = pres.Archive;
            var verifyIncrementalActive = archiveConfiguration.VerifyPreservationIncrementalEnabled ? "attiva" : "non attiva";
            logger.InfoFormat("verifica dell'incrementale {0}", verifyIncrementalActive);

            long incremental = 0;
            long totalPres = 0;
            Objects.Preservation lastExecutedPreservation = null;
            Dictionary<string, long> preservationSectionalValue = new Dictionary<string, long>(); ;
            var preservationHistory = GetPreservationsFromArchive(pres.Archive.IdArchive, 2, 0, out totalPres);
            if (totalPres >= 2)
            {
                lastExecutedPreservation = preservationHistory.Where(x => x.CloseDate.HasValue).FirstOrDefault();
                if (lastExecutedPreservation != null)
                {
                    preservationSectionalValue = (Dictionary<string, long>)JsonConvert.DeserializeObject<Dictionary<string, long>>(lastExecutedPreservation.LastSectionalValue);
                }
            }

            var lastPres = preservationHistory.Where(x => x.IdPreservation != idPreservation).FirstOrDefault();
            if (preservationHistory != null && lastPres != null)
            {
                Document docLastPres = GetLastDocumentPreservation(lastPres.IdPreservation);
                if (docLastPres != null)
                {
                    logger.InfoFormat($"Numero documenti da conservare {documents.Count()}");
                    int year = documents.OrderBy(x => x.DateMain).FirstOrDefault().DateMain.Value.Year;
                    logger.InfoFormat($"Anno documenti da conservare {year}");
                    if (archiveConfiguration.ForceAutoInc && !archiveConfiguration.VerifyPreservationIncrementalEnabled && lastPres.EndDate.HasValue
                        && lastPres.EndDate.Value.Year == year)
                    {
                        var att = docLastPres.AttributeValues.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault()).FirstOrDefault();
                        logger.InfoFormat("ultimo incrementale {0}", att.Value);
                        incremental = (Int64)att.Value;
                    }
                }
            }

            documents = documents.OrderBy(x => x.PreservationIndex);
            pres.Documents = new BindingList<Document>(documents.ToList());
            logger.Info("Recupero anagrafiche eccezioni");

            var mancaValoreChiaveUnivocaId = GetIdPreservationExceptionFromDescription("MancaValoreChiaveUnivoca");
            var validazioneFallitaId = GetIdPreservationExceptionFromDescription("ValidazioneFallita");

            //idArchive = pres.Archive.IdArchive;
            //BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(idArchive);

            var i = 0;
            var numDocuments = documents.Count();
            Dictionary<string, long> lastIncrementalValue = new Dictionary<string, long>();
            foreach (var doc in documents)
            {
                logger.Info(string.Format("Controllo eccezioni - documento ({0}) {1} di {2}", doc.IdDocument, i, numDocuments));
                try
                {
                    BindingList<DocumentAttributeValue> docAttributes = DbProvider.GetFullDocumentAttributeValues(doc.IdDocument);
                    var progressiveError = false;
                    var tmpSectional = docAttributes.Where(x => x.Attribute.IsSectional.GetValueOrDefault(false)).GetOrderAttributeValueString();
                    string sectional = tmpSectional != null ? tmpSectional.ToString().ToUpper() : "Default";
                    Int64 incrementalValue = -1;

                    // se l'autoincrementale è popolato e da verificare 
                    if (archiveConfiguration.VerifyPreservationIncrementalEnabled)
                    {
                        var attributes = docAttributes.Where(x => x.Attribute.IsAutoInc.GetValueOrDefault());

                        var attrCount = attributes.Count();

                        if (attrCount < 1)
                        {
                            logger.InfoFormat("Add Exceptions - Progressivo numerico non presente per il documento con ID " + doc.IdBiblos + " - " + doc.PrimaryKeyValue);

                            exceptions.Add("Progressivo numerico non presente per il documento con ID " + doc.IdBiblos + " - " + doc.PrimaryKeyValue);
                            continue;
                        }
                        else if (attrCount > 1)
                        {
                            logger.InfoFormat("Add Exceptions - E' presente piu' di un progressivo numerico per il documento con ID " + doc.IdBiblos + " - " + doc.PrimaryKeyValue);

                            exceptions.Add("E' presente piu' di un progressivo numerico per il documento con ID " + doc.IdBiblos + " - " + doc.PrimaryKeyValue);
                            continue;
                        }
                        else
                        {
                            var attrValue = attributes.Single();

                            if (attrValue.Value is Int64)
                                incrementalValue = (Int64)attrValue.Value;
                            else if (attrValue.Value is double)
                                incrementalValue = (Int64)attrValue.Value;
                            else if (!string.IsNullOrEmpty(attrValue.Value.ToStringExt()))
                            {
                                try
                                {
                                    if (!long.TryParse(attrValue.Value.ToString(), out incrementalValue))
                                    {
                                        string log = string.Format("Il progressivo numerico per il documento con ID {0} non e' configurato correttamente ( valore attuale = {1} )", doc.IdBiblos, attrValue.Value);
                                        logger.InfoFormat("Add Exceptions - " + log);

                                        exceptions.Add(string.Format("Il progressivo numerico per il documento con ID {0} non e' configurato correttamente ( valore attuale = {1} )", doc.IdBiblos, attrValue.Value));
                                        continue;
                                    }
                                }
                                catch
                                {
                                    string log = string.Format("Il progressivo numerico per il documento con ID {0} non e' configurato correttamente ( valore attuale = {1} )", doc.IdBiblos, attrValue.Value);
                                    logger.InfoFormat("Add Exceptions - " + log);

                                    exceptions.Add(string.Format("Il progressivo numerico per il documento con ID {0} non e' configurato correttamente ( valore attuale = {1} )", doc.IdBiblos, attrValue.Value));
                                    continue;
                                }
                            }
                            else
                            {
                                logger.InfoFormat("Add Exceptions - Progressivo numerico non configurato correttamente per il documento con ID " + doc.IdBiblos);

                                exceptions.Add("Progressivo numerico non configurato correttamente per il documento con ID " + doc.IdBiblos);
                                continue;
                            }
                        }
                        long last = 1;
                        if (lastIncrementalValue.ContainsKey(sectional))
                        {
                            last = lastIncrementalValue[sectional];
                            if (i < documents.Count() &&
                            last + 1 != incrementalValue)
                            {
                                string log = string.Format("Numerazione progressiva errata {3}. Numerazione mancante {4} da {1} a {2}. (document Id {0})", doc.IdBiblos, last + 1, incrementalValue - 1, docAttributes.Where(x => x.Attribute.Name.ToLower() == "signature").Select(x => x.Value).FirstOrDefault(), docAttributes.Where(x => x.Attribute.IsSectional.GetValueOrDefault()).GetOrderAttributeValueString());
                                logger.InfoFormat("Add Exceptions - " + log);

                                exceptions.Add(string.Format("Numerazione progressiva errata {3}. Numerazione mancante {4} da {1} a {2}. (document Id {0})", doc.IdBiblos, last + 1, incrementalValue - 1, docAttributes.Where(x => x.Attribute.Name.ToLower() == "signature").Select(x => x.Value).FirstOrDefault(), docAttributes.Where(x => x.Attribute.IsSectional.GetValueOrDefault()).GetOrderAttributeValueString()));
                                progressiveError = true;
                            }
                        }
                    }
                    else
                    {
                        if (archiveConfiguration.ForceAutoInc)
                        {
                            // se archive.VerifyPreservationIncrementalEnabled = false ed è configurato 
                            // un metadato AutoInc , intero (uno ed uno solo) e non ha valore, allora genera
                            // da solo il valore autoincrementale e lo mette nel metadato. 

                            //cerco sull'archivio perché non è detto che tutti i documenti abbiano l'attributo, in quanto era facoltativo                         
                            var autoIncAttribute = new PreservationService().GetAttributes(archive.IdArchive).Where(x => x.IsAutoInc.HasValue && x.IsAutoInc.Value);
                            if (autoIncAttribute.Count() < 1)
                            {
                                logger.InfoFormat("Add Exceptions - Attributo IsAutoInc non presente per l'archivio {0}", archive.Name);

                                exceptions.Add(string.Concat("Attributo IsAutoInc non presente per l'archivio ", archive.Name));
                                continue;
                            }
                            else if (autoIncAttribute.Count() > 1)
                            {
                                logger.InfoFormat("Add Exceptions - Presenti più attributi IsAutoInc per l'archivio {0}", archive.Name);

                                exceptions.Add(string.Concat("Presenti più attributi IsAutoInc per l'archivio {0}", archive.Name));
                                continue;
                            }
                            else
                            {
                                var autoIncAttr = autoIncAttribute.SingleOrDefault();
                                if (!autoIncAttr.AttributeType.ToLower().Equals("system.int64"))
                                {
                                    logger.InfoFormat("Add Exceptions - Attributo IsAutoInc non configurato correttamente per l'archivio {0}", archive.Name);

                                    exceptions.Add(string.Concat("Attributo IsAutoInc non configurato correttamente per l'archivio {0}", archive.Name));
                                    continue;
                                }
                                else
                                {
                                    incremental += 1;

                                    //se il documento ha attibuto settato
                                    var docAutoIncAttribute = doc.AttributeValues.Where(x => x.IdAttribute == autoIncAttr.IdAttribute).SingleOrDefault();
                                    if (docAutoIncAttribute != null)
                                        docAutoIncAttribute.Value = incremental;
                                    else
                                        //se non ha attributo settato
                                        doc.AttributeValues.Add(new DocumentAttributeValue { Attribute = autoIncAttr, Value = incremental });

                                    //aggiorno i metadati del documento
                                    IDictionary<Guid, BindingList<DocumentAttributeValue>> documentAttributes = new Dictionary<Guid, BindingList<DocumentAttributeValue>>();
                                    documentAttributes.Add(doc.IdDocument, doc.AttributeValues);
                                    UpdateDocumentMetadata(archive.IdArchive, documentAttributes);
                                    //setto la chiave primaria a null, così la aggiorna
                                    doc.PrimaryKeyValue = null;
                                }
                            }
                        }
                    }

                    if (lastIncrementalValue.ContainsKey(sectional))
                        lastIncrementalValue[sectional] = incrementalValue;
                    else
                    {
                        if (preservationSectionalValue.ContainsKey(sectional))
                        {
                            if (preservationSectionalValue[sectional] + 1 != incrementalValue && archiveConfiguration.VerifyPreservationIncrementalEnabled)
                            {
                                string log = string.Format("Numerazione progressiva errata rispetto alla conservazione precedente, Document Id {0}, Ultimo sezionale salvato: {1}", doc.IdBiblos, preservationSectionalValue[sectional]);
                                logger.InfoFormat("Add Exceptions - " + log);

                                exceptions.Add(string.Format("Numerazione progressiva errata rispetto alla conservazione precedente, Document Id {0}, Ultimo sezionale salvato: {1}", doc.IdBiblos, preservationSectionalValue[sectional]));
                            }
                        }
                        lastIncrementalValue.Add(sectional, incrementalValue);
                    }
                    i += 1;
                    if (progressiveError)
                        continue;
                    //Remarks: Chiave univoca duplicata non possibile, garantita da indice IX_PrimaryKeyValue

                    //if (doc.SignHeader != null && AttributeService.GetAttributesHash(docAttributes) != doc.SignHeader)
                    //{
                    //    exceptions.Add(string.Format("Alcuni metadati non sono validi, Document id {0}", doc.IdBiblos));
                    //    UpdatePreservationException(validazioneFallitaId, doc.IdDocument);
                    //    continue;
                    //}

                    DateTime? mainDate;

                    var calculatedPrimaryKey = AttributeService.ParseAttributeValues(archive, docAttributes, out mainDate);
                    if (string.IsNullOrEmpty(doc.PrimaryKeyValue))
                    {
                        if (string.IsNullOrEmpty(calculatedPrimaryKey))
                        {
                            if (docAttributes.Any(x => x.Attribute.KeyOrder > 0))
                            {
                                UpdatePreservationException(mancaValoreChiaveUnivocaId, doc.IdDocument);

                                string log = string.Format("Chiavi Univoche mancanti per la conservazione corrente, Document Id {0}", doc.IdBiblos);
                                logger.InfoFormat("Add Exceptions - " + log);

                                exceptions.Add(string.Format("Chiavi Univoche mancanti per la conservazione corrente, Document Id {0}", doc.IdBiblos));
                                continue;
                            }
                        }

                        doc.PrimaryKeyValue = calculatedPrimaryKey;
                        DbProvider.UpdatePrimaryKey(doc.IdDocument, calculatedPrimaryKey, mainDate);

                    }
                    if (doc.PrimaryKeyValue != calculatedPrimaryKey)
                    {
                        UpdatePreservationException(validazioneFallitaId, doc.IdDocument);
                        exceptions.Add(string.Format("Metadati non corrispondenti con la chiave primaria, Document id {0}", doc.IdBiblos));
                    }
                }
                catch (Exception exx)
                {
                    logger.Error(string.Format("{0} - iterazione {1} su {2} : {3}", METHOD_NAME, i, numDocuments, exx.Message), exx);
                    new PreservationError(exx, PreservationErrorCode.E_SYSTEM_EXCEPTION).ThrowsAsFaultException();
                }
            }

            foreach (var item in preservationSectionalValue.Keys)
            {
                if (!lastIncrementalValue.ContainsKey(item))
                    lastIncrementalValue.Add(item, preservationSectionalValue[item]);
            }
            DbProvider.SavePreservationLastSectionalValue(idPreservation, JsonConvert.SerializeObject(lastIncrementalValue));

            //Reset del progresso.
            logger.Info("Verifica eccezioni terminata.");

            logger.Info("CheckPreservationExceptions - ritorno al chiamante. Numero eccezioni = " + exceptions.Count().ToString());
            preservation = pres;
            return exceptions.Count() <= 0;
        }

        public PreservationTaskType AddPreservationTaskType(PreservationTaskType taskType, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationTaskType - id archivio {0}", idArchive);

            PreservationTaskType ret = DbProvider.AddPreservationTaskType(taskType, idArchive);

            logger.Info("AddPreservationTaskType - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTaskGroup> GetDetailedPreservationTaskGroup(Guid? idTaskGroup, Guid? idArchive,
                                                                                   int maxReturnedValues)
        {
            logger.InfoFormat(
                "GetDetailedPreservationTaskGroup - id gruppo task {0} id archivio {1} valori massimi {2}",
                (idTaskGroup.HasValue ? idTaskGroup.Value.ToString() : null),
                (idArchive.HasValue ? idArchive.Value.ToString() : null), maxReturnedValues);

            BindingList<PreservationTaskGroup> ret = DbProvider.GetDetailedPreservationTaskGroup(idTaskGroup, idArchive,
                                                                                                 maxReturnedValues);

            logger.Info("GetDetailedPreservationTaskGroup - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationTaskGroupExpiryByTask(Guid idPreservationTask, DateTime newEstimatedExpiry,
                                                            Guid idArchive)
        {
            logger.InfoFormat(
                "UpdatePreservationTaskGroupExpiryByTask - id task {0} data scadenza prevista {1} id archivio {2}",
                idPreservationTask, newEstimatedExpiry, idArchive);

            DbProvider.UpdatePreservationTaskGroupExpiryByTask(idPreservationTask, newEstimatedExpiry, idArchive);

            logger.Info("UpdatePreservationTaskGroupExpiryByTask - ritorno al chiamante");
        }

        public BindingList<Objects.Preservation> GetPreservationsByUserAndTask(Guid idPreservationUser,
                                                                               Guid? idPreservationTask, Guid idArchive)
        {
            logger.InfoFormat("GetPreservationsByUserAndTask - id utente {0} id task {1} id archivio {2}",
                              idPreservationUser,
                              (idPreservationTask.HasValue ? idPreservationTask.Value.ToString() : null), idArchive);

            BindingList<Objects.Preservation> ret = DbProvider.GetPreservationsByUserAndTask(idPreservationUser,
                                                                                             idPreservationTask,
                                                                                             idArchive);

            logger.Info("GetPreservationsByUserAndTask - ritorno al chiamante");

            return ret;
        }

        public string GetPreservationPathByIdTask(Guid idtask)
        {
            try
            {
                logger.InfoFormat("GetPreservationPathByIdTask: {0}", idtask);
                var preservationPath = DbProvider.GetPreservationPathByTask(idtask);
                return preservationPath;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        public Objects.Preservation GetPreservationsByIdTask(Guid idtask)
        {
            try
            {
                logger.InfoFormat("GetPreservationsByIdTask: {0}", idtask);
                var idPreservation = DbProvider.GetPreservationIdByTask(idtask);
                if (!idPreservation.HasValue)
                    return null;

                return DbProvider.GetPreservation(idPreservation.Value, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public void RemovePendigPreservation(Guid idArchive)
        {
            try
            {
                logger.InfoFormat("RemovePendigPreservation: {0}", idArchive);
                int total = 0;
                var preservation = DbProvider.GetPreservations(idArchive, 5, 0, out total);
                foreach (var item in preservation)
                {
                    logger.InfoFormat("RemovePendigPreservation CloseDate: {0} - Name: {1}", item.CloseDate, item.Name);
                    //if (!item.CloseDate.HasValue)
                    DbProvider.RemovePreservation(idArchive, item.IdPreservation);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public PreservationHoliday AddPreservationHoliday(PreservationHoliday holiday, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationHoliday - id archivio {0}", idArchive);

            PreservationHoliday ret = DbProvider.AddPreservationHoliday(holiday, idArchive);

            logger.Info("AddPreservationHoliday - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationHoliday(PreservationHoliday holiday, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationHoliday - id archivio {0}", idArchive);

            DbProvider.UpdatePreservationHoliday(holiday, idArchive);

            logger.Info("UpdatePreservationHoliday - ritorno al chiamante");
        }

        public void DeletePreservationHoliday(Guid idPreservationHoliday, Guid idArchive)
        {
            logger.InfoFormat("DeletePreservationHoliday - id festivita {0} id archivio {1}", idPreservationHoliday,
                              idArchive);

            DbProvider.DeletePreservationHoliday(idPreservationHoliday, idArchive);

            logger.Info("DeletePreservationHoliday - ritorno al chiamante");
        }

        public BindingList<PreservationAlertType> GetPreservationAlertTypes(Guid? idPreservationAlertType,
                                                                            Guid idArchive)
        {
            logger.InfoFormat("GetPreservationAlertTypes - id tipo avviso {0} id archivio {1}",
                              (idPreservationAlertType.HasValue ? idPreservationAlertType.Value.ToString() : null),
                              idArchive);

            BindingList<PreservationAlertType> ret = DbProvider.GetPreservationAlertTypes(idPreservationAlertType,
                                                                                          idArchive);

            logger.Info("GetPreservationAlertTypes - ritorno al chiamante");

            return ret;
        }

        public void DeletePreservationAlertType(Guid idPreservationAlertType, Guid idArchive)
        {
            logger.InfoFormat("DeletePreservationAlertType - id tipo avviso {0} id archivio {1}",
                              idPreservationAlertType, idArchive);

            DbProvider.DeletePreservationAlertType(idPreservationAlertType, idArchive);

            logger.Info("DeletePreservationAlertType - ritorno al chiamante");
        }

        public void UpdatePreservationAlertType(PreservationAlertType alertType, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationAlertType - id archivio {0}", idArchive);

            DbProvider.UpdatePreservationAlertType(alertType, idArchive);

            logger.Info("UpdatePreservationAlertType - ritorno al chiamante");
        }

        public PreservationAlertType AddPreservationAlertType(PreservationAlertType alertType, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationAlertType - id archivio {0}", idArchive);

            PreservationAlertType ret = DbProvider.AddPreservationAlertType(alertType, idArchive);

            logger.Info("AddPreservationAlertType - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationAlertType> GetPreservationAlertTypesByTaskType(Guid? idPreservationAlertType,
                                                                                      Guid idPreservationTaskType,
                                                                                      Guid idArchive)
        {
            logger.InfoFormat(
                "GetPreservationAlertTypesByTaskType - id tipo avviso {0} id tipo task {1} id archivio {2}",
                (idPreservationAlertType.HasValue ? idPreservationAlertType.Value.ToString() : null),
                idPreservationTaskType, idArchive);

            BindingList<PreservationAlertType> ret =
                DbProvider.GetPreservationAlertTypesByTaskType(idPreservationAlertType, idPreservationTaskType,
                                                               idArchive);

            logger.Info("GetPreservationAlertTypesByTaskType - ritorno al chiamante");

            return ret;
        }

        public PreservationRole AddPreservationRole(PreservationRole role, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationRole - id archivio {0}", idArchive);

            PreservationRole ret = DbProvider.AddPreservationRole(role, idArchive);

            logger.Info("AddPreservationRole - ritorno al chiamante");

            return ret;
        }

        public void UpdatePreservationRole(PreservationRole role, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationRole - id archivio {0}", idArchive);

            EntityProvider provider = DbProvider;

            using (DbTransaction tran = provider.BeginNoSave())
            {
                try
                {
                    provider.UpdatePreservationRole(role, idArchive);
                    provider.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch
                    {
                    }

                    throw;
                }
            }

            logger.Info("UpdatePreservationRole - ritorno al chiamante");
        }

        public void AddPreservationParameter(string label, string value, Guid idArchive)
        {
            logger.InfoFormat("AddPreservationParameter - label {0} valore {1} id archivio {2}", label, value, idArchive);

            DbProvider.AddPreservationParameter(label, value, idArchive);

            logger.Info("AddPreservationParameter - ritorno al chiamante");
        }

        public void UpdatePreservationParameter(string label, string value, string filterName, Guid? idArchive)
        {
            logger.InfoFormat("UpdatePreservationParameter - label {0} valore {1} filtro {2} id archivio {3}", label,
                              value, filterName, (idArchive.HasValue ? idArchive.Value.ToString() : null));

            DbProvider.UpdatePreservationParameter(label, value, filterName, idArchive);

            logger.Info("UpdatePreservationParameter - ritorno al chiamante");
        }

        public void DeletePreservationParameter(string label, Guid? idArchive)
        {
            logger.InfoFormat("DeletePreservationParameter - label {0} id archivio {1}", label, idArchive);

            DbProvider.DeletePreservationParameter(label, idArchive);

            logger.Info("DeletePreservationParameter - ritorno al chiamante");
        }

        public BindingList<Objects.Preservation> GetPreservationsFromArchive(Guid idArchive, int take, int skip, out long totaiItems)
        {
            logger.InfoFormat("GetPreservationsFromArchive - id archivio {0}", idArchive);

            BindingList<Objects.Preservation> ret = DbProvider.GetPreservationsFromArchive(idArchive, take, skip, out totaiItems);

            logger.InfoFormat("GetPreservationsFromArchive totaiItems:{0}- ritorno al chiamante", totaiItems);

            return ret;
        }

        public Objects.Document GetLastDocumentPreservation(Guid idPreservation)
        {
            logger.InfoFormat("GetLastDocumentPreservation - id conservazione {0}", idPreservation);

            Objects.Document ret = DbProvider.GetLastDocumentPreservation(idPreservation);

            logger.Info("GetLastDocumentPreservation - ritorno al chiamante");

            return ret;
        }

        public PreservationExpireResponse GetPreservationExpire(Guid idSchedule, Guid idTaskGroupType, Guid idArchive)
        {
            logger.InfoFormat(
                "GetPreservationTaskGroupsByScheduleAndGroupType - id scadenziario {0} id tipo gruppo task {1} id archivio {2}",
                idSchedule, idTaskGroupType, idArchive);

            var ret =
                DbProvider.GetPreservationExpire(idSchedule, idTaskGroupType, idArchive);

            logger.Info("GetPreservationTaskGroupsByScheduleAndGroupType - ritorno al chiamante");

            return ret;
        }

        public BindingList<PreservationTask> GetPreservationExpireTask()
        {
            try
            {
                logger.Info("GetPreservationExpireTask");

                return DbProvider.GetPreservationExpireTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public void UpdatePreservationAsSigned(Guid idPreservation, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationAsSigned - id conservazione {0} id archivio {1}", idPreservation,
                              idArchive);

            DbProvider.UpdatePreservationAsSigned(idPreservation, idArchive);

            logger.Info("UpdatePreservationAsSigned - ritorno al chiamante");
        }

        public void UpdatePreservationPath(Guid idPreservation, string path, Guid idArchive)
        {
            logger.InfoFormat("UpdatePreservationPath - id conservazione {0} path {1} id archivio {2}", idPreservation,
                              path, idArchive);

            DbProvider.UpdatePreservationPath(idPreservation, path, idArchive);

            logger.Info("UpdatePreservationPath - ritorno al chiamante");
        }

        public void DeletePreservationTaskGroup(Guid idTaskGroup, Guid idArchive)
        {
            logger.InfoFormat("DeletePreservationTaskGroup - id gruppo task {0} id archivio {1}", idTaskGroup, idArchive);

            DbProvider.DeletePreservationTaskGroup(idTaskGroup, idArchive);

            logger.Info("DeletePreservationTaskGroup - ritorno al chiamante");
        }

        public void UpdatePreservationTaskGroupTypeDescription(Guid idTaskGroupType, string description)
        {
            logger.InfoFormat("UpdatePreservationTaskGroupTypeDescription - id tipo gruppo task {0} descrizione {1}",
                              idTaskGroupType, description);

            DbProvider.UpdatePreservationTaskGroupTypeDescription(idTaskGroupType, description);

            logger.Info("UpdatePreservationTaskGroupTypeDescription - ritorno al chiamante");
        }

        public PreservationTaskGroupType AddPreservationTaskGroupType(PreservationTaskGroupType groupType,
                                                                      Guid idArchive)
        {
            logger.InfoFormat("AddPreservationTaskGroupType - id archivio {0}", idArchive);

            PreservationTaskGroupType ret = DbProvider.AddPreservationTaskGroupType(groupType, idArchive);

            logger.Info("AddPreservationTaskGroupType - ritorno al chiamante");

            return ret;
        }

        public PreservationInfoResponse ResetPreservation(Guid idPreservation, string domainUser)
        {
            logger.InfoFormat("ResetPreservation - id conservazione {0} utente {1}", idPreservation, domainUser);

            PreservationInfoResponse ret = DbProvider.ResetPreservation(idPreservation, domainUser);

            logger.Info("ResetPreservation - ritorno al chiamante");

            return ret;
        }

        public BindingList<Objects.Preservation> GetPreservationsToSign(Guid idArchive, int take, int skip, out int totalPreservations)
        {
            logger.Info("GetPreservationsToSign - entry point");

            try
            {
                BindingList<Objects.Preservation> ret = DbProvider.GetPreservationsToSign(idArchive, take, skip, out totalPreservations);

                logger.Info("GetPreservationsToSign - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<Objects.Preservation> GetPreservations(Guid idArchive, int take, int skip, bool includeReleatedEntities, bool includeCloseContent, out int totalPreservations)
        {
            try
            {
                return DbProvider.GetPreservations(idArchive, take, skip, includeReleatedEntities, includeCloseContent, out totalPreservations);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public BindingList<Objects.Preservation> GetPreservations(Guid idArchive, int take, int skip, out int totalPreservations)
        {
            logger.Info("GetPreservations - entry point");

            try
            {
                BindingList<Objects.Preservation> ret = DbProvider.GetPreservations(idArchive, take, skip, out totalPreservations);

                logger.Info("GetPreservations - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<PreservationJournalingActivity> GetPreservationJournalingActivities(
            Guid? idJournalingActivity, bool includeJournal = true)
        {
            logger.InfoFormat("GetPreservationJournalingActivities - id attivita' {0}",
                              (idJournalingActivity.HasValue ? idJournalingActivity.Value.ToString() : null));

            try
            {
                BindingList<PreservationJournalingActivity> ret =
                    DbProvider.GetPreservationJournalingActivities(idJournalingActivity, includeJournal);

                logger.Info("GetPreservationJournalingActivities - ritorno al chiamante");

                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void UpdatePreservationJournaling(PreservationJournaling toUpdate)
        {
            logger.Info("UpdatePreservationJournaling - entry point");

            try
            {
                DbProvider.UpdatePreservationJournaling(toUpdate);

                logger.Info("UpdatePreservationJournaling - ritorno al chiamante");
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public PreservationJournaling AddPreservationJournaling(PreservationJournaling toAdd)
        {
            logger.Info("AddPreservationJournaling - entry point");

            try
            {
                PreservationJournaling ret = DbProvider.AddPreservationJournaling(toAdd);

                logger.Info("AddPreservationJournaling - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDevices(Guid? idPreservationStorageDevice)
        {
            logger.InfoFormat("GetPreservationStorageDevices - id supporto {0} id archivio {1}",
                              (idPreservationStorageDevice.HasValue
                                   ? idPreservationStorageDevice.Value.ToString()
                                   : null));
            try
            {
                BindingList<PreservationStorageDevice> ret =
                    DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice);

                logger.Info("GetPreservationStorageDevices - ritorno al chiamante");

                foreach (var item in ret)
                {
                    foreach (var preservation in item.PreservationsInDevice)
                    {
                        if (preservation.Device != null)
                        {
                            preservation.Device.PreservationsInDevice = null;
                        }
                        if (preservation.Preservation != null)
                        {
                            preservation.Preservation.Documents = null;
                            if (preservation.Device != null)
                                preservation.Device.PreservationsInDevice = null;
                        }
                    }
                }
                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<PreservationInStorageDevice> GetPreservationsInStorageDevices(Guid? idPreservation, Guid? idPreservationStorageDevice, int skip, int take, out int totalItems)
        {
            logger.InfoFormat("GetPreservationsInStorageDevices - id conservazione {0} id supporto {1}",
                              (idPreservation.HasValue ? idPreservation.Value.ToString() : null),
                              (idPreservationStorageDevice.HasValue
                                   ? idPreservationStorageDevice.Value.ToString()
                                   : null));
            try
            {
                var provider = DbProvider;
                provider.BeginNoSaveNoTransaction();

                BindingList<PreservationInStorageDevice> ret = provider.GetPreservationsInStorageDevices(idPreservation, idPreservationStorageDevice, skip, take, out totalItems);

                foreach (var item in ret)
                {
                    if (item.Preservation == null)
                        continue;
                    item.Preservation.Archive = provider.GetArchive(item.Preservation.IdArchive);
                }
                provider.ForceDispose();

                logger.Info("GetPreservationsInStorageDevices - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public DateTime? GetPreservationJournalingLastPrintManualActivityDate()
        {
            logger.Info("GetPreservationJournalingLastPrintManualActivityDate - entry point");

            try
            {
                DateTime? ret = DbProvider.GetPreservationJournalingLastPrintManualActivityDate();

                logger.Info("GetPreservationJournalingLastPrintManualActivityDate - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<PreservationStorageDeviceStatus> GetPreservationStorageDeviceStatus(Guid? idStatus)
        {
            logger.InfoFormat("GetPreservationStorageDeviceStatus - id stato {0}",
                              (idStatus.HasValue ? idStatus.Value.ToString() : null));

            try
            {
                BindingList<PreservationStorageDeviceStatus> ret =
                    DbProvider.GetPreservationStorageDeviceStatus(idStatus);

                logger.Info("GetPreservationStorageDeviceStatus - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public PreservationInStorageDevice AddPreservationInStorageDevice(PreservationInStorageDevice toAdd)
        {
            logger.Info("AddPreservationInStorageDevice - entry point");

            try
            {
                PreservationInStorageDevice ret = DbProvider.AddPreservationInStorageDevice(toAdd);

                logger.Info("AddPreservationInStorageDevice - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public void DeletePreservationInStorageDevice(PreservationInStorageDevice preservationInStorageDevice)
        {
            logger.Info("DeletePreservationInStorageDevice - entry point");

            try
            {
                DbProvider.DeletePreservationInStorageDevice(preservationInStorageDevice);

                logger.Info("DeletePreservationInStorageDevice - ritorno al chiamante");
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public void DeletePreservationStorageDevice(PreservationStorageDevice preservationStorageDevice)
        {
            logger.Info("DeletePreservationStorageDevice - entry point");

            try
            {
                DbProvider.DeletePreservationStorageDevice(preservationStorageDevice);

                logger.Info("DeletePreservationStorageDevice - ritorno al chiamante");
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public PreservationStorageDevice AddPreservationStorageDevice(PreservationStorageDevice toAdd)
        {
            logger.Info("AddPreservationStorageDevice - entry point");

            try
            {
                PreservationStorageDevice ret = DbProvider.AddPreservationStorageDevice(toAdd);

                logger.Info("AddPreservationStorageDevice - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public void DeletePreservationJournaling(Guid idJournaling, Guid idArchive)
        {
            logger.InfoFormat("DeletePreservationJournaling - id journal {0} id archivio {1}", idJournaling, idArchive);

            DbProvider.DeletePreservationJournaling(idJournaling, idArchive);

            logger.Info("DeletePreservationJournaling - ritorno al chiamante");
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDeviceFromLabel(string label,
                                                                                            Guid? idArchive)
        {
            logger.InfoFormat("GetPreservationStorageDeviceFromLabel - etichetta {0} id archivio {1}", label,
                              (idArchive.HasValue ? idArchive.Value.ToString() : null));

            try
            {
                BindingList<PreservationStorageDevice> ret = DbProvider.GetPreservationStorageDeviceFromLabel(label);

                logger.Info("GetPreservationStorageDeviceFromLabel - ritorno al chiamante");

                return ret;
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public BindingList<PreservationStorageDevice> GetPreservationStorageDevicesFromDates(Guid? idPreservationStorageDevice, DateTime? minDate, DateTime? maxDate, string username, int skip, int take, out long totalItems)
        {
            try
            {
                var retval = new BindingList<PreservationStorageDevice>();

                var provider = DbProvider;
                provider.BeginNoSaveNoTransaction();
                retval = provider.GetPreservationStorageDevicesFromDates(idPreservationStorageDevice, minDate, maxDate, username, skip, take, out totalItems);
                provider.ForceDispose();

                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public void UpdatePreservationStorageDeviceLastVerifyDate(Guid idStorageDevice, DateTime? verifyDate)
        {
            logger.InfoFormat("UpdatePreservationStorageDeviceLastVerifyDate - id supporto {0} data verifica {1}",
                              idStorageDevice, (verifyDate.HasValue ? verifyDate.Value.ToString() : null));

            try
            {
                DbProvider.UpdatePreservationStorageDeviceLastVerifyDate(idStorageDevice, verifyDate);

                logger.Info("UpdatePreservationStorageDeviceLastVerifyDate - ritorno al chiamante");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public PreservationStorageDeviceStatus PreservationStorageDeviceChangeStatus(Guid idPreservation, PreservationStatus preservationStatus)
        {
            logger.InfoFormat("PreservationStorageDeviceChangeStatus - id Preservation {0} stato = {1}", idPreservation,
                              preservationStatus);

            try
            {
                return DbProvider.PreservationStorageDeviceChangeStatus(idPreservation, preservationStatus);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.Info("PreservationStorageDeviceChangeStatus - ritorno al chiamante");
            }
        }

        public BindingList<PreservationJournaling> GetPreservationJournalings(Guid? idArchive, Guid? idPreservation,
                                                                              DateTime? startDate, DateTime? endDate, Guid? idCompany,
                                                                              int skip, int take,
                                                                              out int journalingsInArchive, bool includePreservation = true, bool sortingDescending = true)
        {
            return GetPreservationJournalings(idArchive, idPreservation, startDate, endDate, null, idCompany, skip, take, out journalingsInArchive, includePreservation, sortingDescending);
        }

        public BindingList<PreservationJournaling> GetPreservationJournalings(Guid? idArchive, Guid? idPreservation,
                                                                              DateTime? startDate, DateTime? endDate, Guid? idActivityType, Guid? idCompany,
                                                                              int skip, int take,
                                                                              out int journalingsInArchive, bool includePreservation = true, bool sortingDescending = true)
        {
            return DbProvider.GetPreservationJournalings(idArchive, idPreservation, startDate, endDate, idActivityType, idCompany, skip, take,
                                                         out journalingsInArchive, includePreservation, sortingDescending);
        }

        public BindingList<Objects.Preservation> GetPreservationsFromJournaling(Guid idJournaling, int skip, int take,
                                                                                out int preservationsCount)
        {
            return DbProvider.GetPreservationsFromJournaling(idJournaling, skip, take, out preservationsCount);
        }

        private string formatSupportoLabelName(PreservationStorageDevice device)
        {
            //return string.Format("{0}_{1:yyyyMMdd}_{2:yyyyMMdd}", device.Label, device.MinDate.GetValueOrDefault(), device.MaxDate.GetValueOrDefault());
            return device.Label;
        }

        public bool CreateArchivePreservationMark(Guid idStorageDevice)
        {
            logger.InfoFormat("CreateArchivePreservationMark - id supporto {0}", idStorageDevice);

            var retval = false;

            try
            {
                PreservationStorageDevice device = GetPreservationStorageDevices(idStorageDevice)
                    .SingleOrDefault();

                if (device == null)
                    new PreservationError("Non ci sono archivi informatici aventi ID " + idStorageDevice,
                                          PreservationErrorCode.E_UNEXPECTED_RESULT)
                        .ThrowsAsFaultException();

                var archiveCompany = DbProvider.GetArchiveCompany(null, device.Company).First();

                var templateFile = archiveCompany.XmlFileTemplatePath;
                if (string.IsNullOrWhiteSpace(templateFile))
                {
                    var diagMsg = "Nessun percorso configurato per il template XML.";
                    logger.Error(diagMsg);
                    throw new FaultException(diagMsg);
                }

                var folderFileDeiFiles = archiveCompany.WorkingDir;
                if (string.IsNullOrWhiteSpace(folderFileDeiFiles) || !new DirectoryInfo(folderFileDeiFiles).Exists)
                {
                    var diagMsg = "Nessun percorso configurato per il file passabile di marca temporale.";
                    logger.Error(diagMsg);
                    throw new FaultException(diagMsg);
                }
                else if (!Directory.Exists(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device))))
                {
                    Directory.CreateDirectory(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)));
                }

                var conteggioConservazioniAssenti = device.PreservationsInDevice.Where(x => x.Preservation == null).Count();

                if (conteggioConservazioniAssenti != 0)
                {
                    logger.InfoFormat("Numero di \"PreservationsInDevice\" senza conservazione associata: ", conteggioConservazioniAssenti);

                    new PreservationError("Alcune conservazioni gia' nell'archivio informatico sono assenti in banca dati.")
                        .ThrowsAsFaultException();
                }

                var conservazioni = device.PreservationsInDevice.Select(x => x.Preservation).OrderBy(x => x.Archive.Name).ThenBy(x => x.StartDate);

                var buffer = File.ReadAllText(templateFile);
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
                var fileDeiFiles = new FileInfo(Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "txt")));

                logger.InfoFormat("File contenente TUTTI i file di chiusura creato: {0}", fileDeiFiles.FullName);

                if (fileDeiFiles.Exists)
                {
                    fileDeiFiles.Delete();
                }

                foreach (var cons in conservazioni)
                {
                    pathConservazione = new DirectoryInfo(cons.Path);

                    if (!pathConservazione.Exists)
                        new PreservationError("Percorso conservazione inesistente per conservazione avente ID " + cons.IdPreservation)
                            .ThrowsAsFaultException();

                    fileChiusura = pathConservazione.GetFiles("CHIUSURA*.txt").SingleOrDefault();

                    if (fileChiusura == null || !fileChiusura.Exists)
                        new PreservationError("File chiusura conservazione inesistente per conservazione avente ID " + cons.IdPreservation)
                                .ThrowsAsFaultException();

                    cons.Archive = ArchiveService.GetArchive(cons.IdArchive);

                    if (!listaArchivi.Any(x => x.IdArchive == cons.IdArchive))
                        listaArchivi.Add(cons.Archive);

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
                    File.AppendAllLines(fileDeiFiles.FullName, lines);
                }

                var isSha2 = ConfigurationManager.AppSettings["FormatoSuperImprontaSHA2"] ?? "True";

                buffer = buffer.Replace("%Impronta%", UtilityService.GetHash(fileDeiFiles.FullName, isSha2.Equals("True", StringComparison.InvariantCultureIgnoreCase)));

                Documento docXml;
                StringBuilder sbListaDocumenti = new StringBuilder();
                DateTime dataVal;
                TipoDocumento enumTipoDoc;
                foreach (var archive in listaArchivi)
                {
                    docXml = new Documento();

                    docXml.Numero = numDocumenti[archive.IdArchive];

                    dataVal = dataInizio[archive.IdArchive].Min();
                    docXml.DataInizioVal = new DataImpegno { Anno = dataVal.Year, Mese = dataVal.Month, Giorno = dataVal.Day };

                    dataVal = dataFine[archive.IdArchive].Max();
                    docXml.DataFineVal = new DataImpegno { Anno = dataVal.Year, Mese = dataVal.Month, Giorno = dataVal.Day };

                    docXml.TipoDocumento = Enum.TryParse<TipoDocumento>(archive.FiscalDocumentType ?? string.Empty, out enumTipoDoc) ? enumTipoDoc : TipoDocumento.AltriDocumenti;

                    sbListaDocumenti.Append(docXml.GetSerializedForm());
                }

                buffer = buffer.Replace("%ListaDocumenti%", sbListaDocumenti.ToString());

                var path = Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "xml"));

                File.WriteAllBytes(Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), "Template.xml"), File.ReadAllBytes(archiveCompany.XmlFileTemplatePath));

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

        public byte[] GetArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            logger.InfoFormat("GetArchivePreservationMarkFile - id supporto {0} skip {1} take {2}", idPreservationStorageDevice, skip, take);

            if (take < 0)
                take = 100;

            fileSize = 0L;

            var retval = new byte[take];

            try
            {
                var device = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

                if (device == null)
                    throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

                var archiveCompany = DbProvider.GetArchiveCompany(null, device.Company).First();

                var folderFileDeiFiles = archiveCompany.WorkingDir;

                if (string.IsNullOrWhiteSpace(folderFileDeiFiles) || !new DirectoryInfo(folderFileDeiFiles).Exists)
                    throw new FaultException("Nessun percorso configurato per il file passabile di marca temporale.");

                var path = Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "xml"));
                //var path = Path.Combine(folderFileDeiFiles, string.Format("{0}_{1}.xml", idPreservationStorageDevice, device.Label));

                using (var reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Write, take, FileOptions.RandomAccess))
                {
                    try
                    {
                        fileSize = reader.Length;

                        reader.Seek(skip, SeekOrigin.Begin);

                        var readedBytes = reader.Read(retval, 0, take);

                        if (readedBytes != retval.Length)
                        {
                            Array.Resize<byte>(ref retval, readedBytes);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException(ex.Message);
            }

            logger.Info("GetArchivePreservationMarkFile - ritorno al chiamante.");

            return retval;
        }

        public byte[] GetClosingFilesTimeStampFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            logger.InfoFormat("GetClosingFilesTimeStampFile - id supporto {0} skip {1} take {2}", idPreservationStorageDevice, skip, take);

            if (skip < 1)
                skip = 0;

            if (take < 1)
                take = 100;

            var retval = new byte[take];

            var device = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

            if (device == null)
                throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

            var archiveCompany = DbProvider.GetArchiveCompany(null, device.Company).First();

            var folderFileDeiFiles = archiveCompany.WorkingDir;

            if (string.IsNullOrWhiteSpace(folderFileDeiFiles))
                new PreservationError("La working directory non e' configurata correttamente.").ThrowsAsFaultException();

            var timeStampFile = new FileInfo(Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "txt")));

            if (!timeStampFile.Exists)
                new PreservationError("Il file di chiusura non esiste sul server.").ThrowsAsFaultException();

            using (var reader = new FileStream(timeStampFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Write, take, FileOptions.RandomAccess))
            {
                try
                {
                    fileSize = reader.Length;

                    reader.Seek(skip, SeekOrigin.Begin);

                    var readedBytes = reader.Read(retval, 0, take);

                    if (readedBytes != retval.Length)
                    {
                        Array.Resize<byte>(ref retval, readedBytes);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            logger.Info("GetClosingFilesTimeStampFile - ritorno al chiamante");

            return retval;
        }

        public bool TimeStampArchivePreservationMarkFile(Guid idPreservationStorageDevice, byte[] timeStampedFile, bool isInfoCamere)
        {
            var device = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

            if (device == null)
                throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

            var archiveCompany = DbProvider.GetArchiveCompany(null, device.Company).First();

            var folderFileDeiFiles = archiveCompany.WorkingDir;

            if (string.IsNullOrWhiteSpace(folderFileDeiFiles))
                new PreservationError("La working directory non e' configurata correttamente.").ThrowsAsFaultException();

            var fileXML = Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "xml"));

            if (!File.Exists(fileXML))
                new PreservationError("Il file XML per l'impronta archivio non e' stato generato.").ThrowsAsFaultException();

            var retval = false;

            using (var sr = new StringReader(Encoding.Default.GetString(timeStampedFile)))
            {
                var sb = new StringBuilder();
                string linea;
                if (isInfoCamere)
                {
                    while ((linea = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(linea) && linea.Contains("Content-Description: time-stamp response"))
                        {
                            while ((linea = sr.ReadLine()) != null)
                            {
                                if (!string.IsNullOrWhiteSpace(linea))
                                {
                                    if (linea.Trim().Contains("--Dike--"))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        sb.AppendLine(linea);
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (sb.Length > 0)
                    {
                        var marcaTemporale = sb.ToString();
                        if (!string.IsNullOrWhiteSpace(marcaTemporale))
                        {
                            var nuovoXml = File.ReadAllText(fileXML);
                            nuovoXml = nuovoXml.Replace("<MarcaTemporale>", string.Format("<MarcaTemporale>{0}", marcaTemporale));
                            File.WriteAllText(fileXML, nuovoXml);
                            retval = true;
                        }
                    }
                }
                else
                {
                    new PreservationError("Formato TSD non ancora implementato.")
                        .ThrowsAsFaultException();
                }
            }

            File.WriteAllBytes(Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "txt.p7m")), timeStampedFile);

            return retval;
        }

        public byte[] GetTimeStampedArchivePreservationMarkFile(Guid idPreservationStorageDevice, long skip, int take, out long fileSize)
        {
            logger.InfoFormat("GetTimeStampedArchivePreservationMarkFile - id supporto {0} skip {1} take {2}", idPreservationStorageDevice, skip, take);

            if (skip < 1)
                skip = 0;

            if (take < 1)
                take = 100;

            var retval = new byte[take];

            var device = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

            if (device == null)
                throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

            var archiveCompany = DbProvider.GetArchiveCompany(null, device.Company).First();

            var folderFileDeiFiles = archiveCompany.WorkingDir;

            if (string.IsNullOrWhiteSpace(folderFileDeiFiles))
                new PreservationError("La working directory non e' configurata correttamente.").ThrowsAsFaultException();

            var fileXML = Path.Combine(Path.Combine(folderFileDeiFiles, formatSupportoLabelName(device)), normalizzaNomeFile(archiveCompany, device, "xml"));

            if (!File.Exists(fileXML))
                new PreservationError("Il file XML per l'impronta archivio non e' stato generato.").ThrowsAsFaultException();

            using (var reader = new FileStream(fileXML, FileMode.Open, FileAccess.Read, FileShare.Write, take, FileOptions.RandomAccess))
            {
                try
                {
                    fileSize = reader.Length;

                    reader.Seek(skip, SeekOrigin.Begin);

                    var readedBytes = reader.Read(retval, 0, take);

                    if (readedBytes != retval.Length)
                    {
                        Array.Resize<byte>(ref retval, readedBytes);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            logger.Info("GetTimeStampedArchivePreservationMarkFile - ritorno al chiamante");

            return retval;
        }

        public int GetPreservationStorageDevicesCount()
        {
            return DbProvider.GetPreservationStorageDevicesCount();
        }

        public BindingList<Objects.Preservation> GetPreservationsFromArchiveAndDates(Guid idArchive, DateTime minDate, DateTime maxDate, int take, int skip, out long totalItems)
        {
            return DbProvider.GetPreservationsFromArchiveAndDates(idArchive, minDate, maxDate, take, skip, out totalItems);
        }

        public CryptoType GetUsedCryptography(byte[] encryptedBuffer)
        {
            var ret = CryptoType.TYPE_UNKNOWN;

            if (encryptedBuffer != null)
            {
                switch (encryptedBuffer.Length)
                {
                    case 20:
                        ret = CryptoType.TYPE_SHA1;
                        break;
                    case 32:
                        ret = CryptoType.TYPE_SHA256;
                        break;
                }
            }

            return ret;
        }

        public string GetHashFromFile(string filePathName, bool useSHA256)
        {
            string ret = string.Empty;

            logger.InfoFormat("GetHashFromFile - file {0}, sha256 = {1}", filePathName, useSHA256);

            try
            {
                if (!string.IsNullOrWhiteSpace(filePathName) && new FileInfo(filePathName).Exists)
                {
                    ret = this.GetHashFromBuffer(File.ReadAllBytes(filePathName), useSHA256);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.InfoFormat("GetHashFromFile - ritorno al chiamante", filePathName);

            return ret;
        }

        public string GetHashFromBuffer(byte[] buffer, bool useSHA256)
        {
            string ret = string.Empty;

            logger.InfoFormat("GetHashFromBuffer - sha256 = {0}", useSHA256);

            try
            {
                if (buffer != null)
                {
                    ret = UtilityService.GetHash(buffer, useSHA256);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.InfoFormat("GetHashFromBuffer - ritorno al chiamante");

            return ret;
        }

        public PreservationSchedule GetDefaultPreservationSchedule()
        {
            logger.Info("GetDefaultPreservationSchedule - entry point");

            try
            {
                return DbProvider.GetDefaultPreservationSchedule();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("GetDefaultPreservationSchedule - ritorno al chiamante");
            }
        }

        private string normalizzaNomeFile(ArchiveCompany company, PreservationStorageDevice device, string extension)
        {
            extension = (extension ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(extension))
                extension = ".ndf";

            if (!extension.StartsWith("."))
                extension = "." + extension;

            return device.Label + extension;
        }

        public string UploadEntratelFile(string clientFileName, Guid idPreservationStorageDevice, byte[] fileContent)
        {
            logger.InfoFormat("UploadEntratelFile - nome file sul client = {0} id supporto = {1} lunghezza contenuto file = {2}", clientFileName, idPreservationStorageDevice, (fileContent != null) ? fileContent.Length.ToString() : "IL CONTENUTO E' NULLO!");

            string retval = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(clientFileName))
                    throw new FaultException("Il nome del file sul client non e' valido.");

                if (fileContent == null || fileContent.Length < 1)
                    throw new FaultException("Il file e' vuoto.");

                var dev = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

                if (dev == null)
                    throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

                var archiveCompany = DbProvider.GetArchiveCompany(null, dev.Company).First();

                var folderUpload = archiveCompany.WorkingDir;

                if (string.IsNullOrWhiteSpace(folderUpload))
                    throw new FaultException("La working directory non e' configurata correttamente.");

                folderUpload = Path.Combine(folderUpload, formatSupportoLabelName(dev));

                try
                {
                    if (!Directory.Exists(folderUpload))
                        throw new ApplicationException();
                }
                catch
                {
                    throw new FaultException(string.Format("La directory di upload {0} non esiste o non e' utilizzabile.", folderUpload));
                }

                retval = Path.Combine(folderUpload, normalizzaNomeFile(archiveCompany, dev, Path.GetExtension(clientFileName)));

                try
                {
                    DbProvider.UpdateEntratelFileName(idPreservationStorageDevice, retval);
                    File.WriteAllBytes(retval, fileContent);
                }
                catch
                {
                    throw new FaultException("Errore durante la scrittura del file " + retval);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.InfoFormat("UploadEntratelFile - ritorno al chiamante con file {0}", retval);

            return retval;
        }

        public byte[] DownloadEntratelFile(Guid idPreservationStorageDevice, long skip, int take, out long fileLenght)
        {
            logger.InfoFormat("DownloadEntratelFile - id supporto = {0}", idPreservationStorageDevice);

            fileLenght = 0L;

            if (skip < 1)
                skip = 0;

            if (take < 1)
                take = 10;

            var retval = new byte[take];

            try
            {
                var dev = DbProvider.GetPreservationStorageDevices(idPreservationStorageDevice)
                    .SingleOrDefault();

                if (dev == null)
                    throw new FaultException("Nessun supporto con ID " + idPreservationStorageDevice);

                var archiveCompany = DbProvider.GetArchiveCompany(null, dev.Company).First();

                var folderUpload = archiveCompany.WorkingDir;

                if (string.IsNullOrWhiteSpace(folderUpload))
                    throw new FaultException("La working directory non e' configurata correttamente.");

                folderUpload = Path.Combine(folderUpload, formatSupportoLabelName(dev));

                try
                {
                    if (!Directory.Exists(folderUpload))
                        throw new ApplicationException();
                }
                catch
                {
                    throw new FaultException(string.Format("La directory di upload {0} non esiste o non e' utilizzabile.", folderUpload));
                }

                try
                {
                    using (var reader = new FileStream(dev.EntratelCompleteFileName, FileMode.Open, FileAccess.Read, FileShare.Write, take, FileOptions.RandomAccess))
                    {
                        try
                        {
                            fileLenght = reader.Length;

                            reader.Seek(skip, SeekOrigin.Begin);

                            var readedBytes = reader.Read(retval, 0, take);

                            if (readedBytes != retval.Length)
                            {
                                Array.Resize<byte>(ref retval, readedBytes);
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
                catch
                {
                    throw new FaultException("Errore in lettura del file " + dev.EntratelCompleteFileName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.InfoFormat("DownloadEntratelFile - ritorno al chiamante con file di lunghezza {0} bytes", fileLenght);
            return retval;
        }

        public string GetCurrentArchivePreservationMarkXmlTemplate(string companyName)
        {
            logger.InfoFormat("GetCurrentArchivePreservationMarkXmlTemplate - azienda {0}", companyName);

            string templateXml = string.Empty;

            try
            {
                var item = GetArchiveCompany(null, companyName).FirstOrDefault();

                if (item == null)
                    throw new FaultException("Nessuna azienda trovata in banca dati con la seguente ragione sociale: " + companyName);

                templateXml = item.XmlFileTemplatePath;

                if (string.IsNullOrWhiteSpace(templateXml))
                    throw new FaultException("La chiave \"ArchivePreservationMarkXmlTemplate\" non e' configurata correttamente.");

                if (!File.Exists(templateXml))
                    throw new FaultException("Il file XML di template per l'impronta archivio non e' presente sul server sotto al percorso " + templateXml);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.Info("GetCurrentArchivePreservationMarkXmlTemplate - ritorno al chiamante");

            return File.ReadAllText(templateXml);
        }

        public void ChangeCurrentArchivePreservationMarkXmlTemplate(string xmlContent, string companyName)
        {
            logger.InfoFormat("ChangeCurrentArchivePreservationMarkXmlTemplate - azienda {0}", companyName);

            try
            {
                var item = GetArchiveCompany(null, companyName).FirstOrDefault();

                if (item == null)
                    throw new FaultException("Nessuna azienda trovata in banca dati con la seguente ragione sociale: " + companyName);

                var templateXml = item.XmlFileTemplatePath;

                if (string.IsNullOrWhiteSpace(templateXml))
                    throw new FaultException("La chiave \"ArchivePreservationMarkXmlTemplate\" non e' configurata correttamente.");

                string xsd = null;

                try
                {
                    xsd = Path.Combine(Path.GetDirectoryName(templateXml), "schema_impronte.xsd");

                    if (!File.Exists(xsd))
                        new ApplicationException("Nessun file XSD presente sotto il percorso " + xsd);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Errore in fase di recuper XSD. Dettagli errore: " + ex.Message ?? "n.d.");
                }

                var originalContent = xmlContent ?? string.Empty;

                //Fase uno: le "variabili" fondamentali devono essere presenti.
                bool canProceed = xmlContent.Contains("%PeriodoImposta%");

                canProceed = canProceed && xmlContent.Contains("%Impronta%");
                canProceed = canProceed && xmlContent.Contains("%MarcaTemporale%");
                canProceed = canProceed && xmlContent.Contains("%NumDocumenti%");
                canProceed = canProceed && xmlContent.Contains("%ListaDocumenti%");

                if (!canProceed)
                    throw new FaultException("Alcune variabili fondamentali sono assenti nel template. Impossibile salvare.");

                //Fase due: sostituzione delle "variabili" con informazioni farlocche.
                xmlContent = xmlContent.Replace("%PeriodoImposta%", "2011");
                xmlContent = xmlContent.Replace("%Impronta%", @"E6FAB90D662E4EB2A9C2F208905C59C6B37BBEA9977560C0135822F8ED0421BC");
                xmlContent = xmlContent.Replace("%MarcaTemporale%", "Nessuna");
                xmlContent = xmlContent.Replace("%NumDocumenti%", "1");
                xmlContent = xmlContent.Replace("%ListaDocumenti%", "<Documento TipoDocumento=\"AltriDocumenti\"><Numero>1</Numero><DataInizioVal><Giorno>1</Giorno><Mese>1</Mese><Anno>2011</Anno></DataInizioVal><DataFineVal><Giorno>28</Giorno><Mese>7</Mese><Anno>2011</Anno></DataFineVal></Documento>");

                //Fase tre: diamo tutto in pasto al validatore XML interno all'XmlReader.
                // Create the XmlSchemaSet class.
                var sc = new XmlSchemaSet();
                //Create the validation error messages' list.
                var errori = new List<string>();
                //Use the URI within the SchemaSet.
                sc.Add(null, "file://" + xsd);
                // Set the validation settings.
                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationEventHandler += (s, e) =>
                {
                    errori.Add(e.Message);
                };
                // Create the XmlReader object.
                using (var memStream = new MemoryStream(Encoding.Default.GetBytes(xmlContent)))
                {
                    using (var reader = XmlReader.Create(memStream, settings))
                    {
                        try
                        {
                            // Parse the file. 
                            while (reader.Read()) ;
                        }
                        finally
                        {
                            //Close the reader.
                            reader.Close();
                        }
                    }
                    memStream.Close();
                }

                if (errori.Count > 0)
                    throw new FaultException(string.Join(Environment.NewLine, errori.ToArray()));

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(originalContent);
                xmlDoc.Save(templateXml);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException("Si sono verificati i seguenti errori in fase di validazione: " + ex.Message);
            }

            logger.Info("ChangeCurrentArchivePreservationMarkXmlTemplate - ritorno al chiamante");
        }

        public BindingList<ArchiveCompany> GetArchiveCompany(Guid? idArchive, string companyName)
        {
            logger.InfoFormat("GetArchiveCompany - id archivio {0} azienda {1}", idArchive.HasValue ? idArchive.ToString() : string.Empty, companyName);

            try
            {
                var ret = DbProvider.GetArchiveCompany(idArchive, companyName);

                logger.Info("GetArchiveCompany - ritorno al chiamante");

                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public BindingList<ArchiveCompany> GetArchiveCompanyByUser(Guid? idArchive, string companyName, string username)
        {
            logger.InfoFormat("GetArchiveCompanyByUser - id archivio {0} azienda {1} utente {2}", idArchive.HasValue ? idArchive.ToString() : null, companyName, username);

            try
            {
                var ret = DbProvider.GetArchiveCompanyByUser(idArchive, companyName, username);

                logger.Info("GetArchiveCompanyByUser - ritorno al chiamante");

                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        DateTime? lastNotify;

        private void Pulse(string methos, string message, int progress)
        {
            logger.InfoFormat("Log in Pulse - Method " + methos + " message " + message + " progress " + progress.ToString());

            if (OnPulse != null)
            {
                if (PulseHighFrequencyEnabled)
                {
                    OnPulse(this, PulseEventArgs.Create(progress, message, methos));
                }
                else
                {
                    if (!lastNotify.HasValue || (DateTime.Now - lastNotify.Value).TotalSeconds > 1)
                    {
                        lastNotify = DateTime.Now;
                        OnPulse(this, PulseEventArgs.Create(progress, message, methos));
                    }
                }
            }
        }

        #region UTILITIES

        /// <summary>
        /// Crea (formatta) una nuova riga in formato XML.
        /// </summary>
        /// <param name="dtCD"></param>
        /// <param name="dtCV"></param>
        /// <returns></returns>
        private static string GetNewXMLRow(DataTable dtCD, DataTable dtCV)
        {
            bool bFound;
            string sRow = "";

            //L'elenco dei campi è ordinato sulla tabella dtCD

            for (int i = 0; i < dtCD.Rows.Count; i++)
            {
                DataRow Row = dtCD.Rows[i];
                string sValue = "";
                string sAttributo = Row["Attributo"].ToString();
                //cerco l'attributo il campo corrente nel recordset
                bFound = false;
                for (int j = 0; (j < dtCV.Rows.Count) && !bFound; j++)
                {
                    DataRow RowVal = dtCV.Rows[j];
                    if (Row["Attributo"].ToString() == RowVal["Attributo"].ToString())
                    {
                        bFound = true;
                        sValue = RowVal["Valore"].ToString();
                    }
                }

                if (bFound)
                {
                    sRow += "<Attributo Nome=\"" + sAttributo + "\">";
                    sRow += "<![CDATA[" + sValue + "]]>";
                    sRow += "</Attributo>\r\n";
                }
            }

            return sRow;
        }

        /// <summary>
        /// Restituisce a riga testuale dtCD datatable cone le dimensioni dei singoli campi, dtCV tabella con i valori correnti
        /// </summary>
        /// <param name="Enum"></param>
        /// <param name="dtCD"></param>
        /// <param name="dtCV"></param>
        /// <returns></returns>
        private static string GetNewTxtRow(string Enum, DataTable dtCD, DataTable dtCV)
        {
            bool bFound;
            int nLen = 0;
            string sRow = "       ".Substring(Enum.Length) + Enum + " ";

            //L'elenco dei campi è ordinato sulla tabella dtCD
            for (int i = 0; i < dtCD.Rows.Count; i++)
            {
                DataRow Row = dtCD.Rows[i];
                nLen = int.Parse(Row["Lunghezza"].ToString());
                string sValue = "";

                //creo una stringa di spazi della lunghezza definita per il campo
                string sBlank = "";

                for (int n = 0; n < nLen; n++)
                    sBlank += " ";

                //cerco l'attributo il campo corrente nel recordset
                bFound = false;
                for (int j = 0; (j < dtCV.Rows.Count) && !bFound; j++)
                {
                    DataRow RowVal = dtCV.Rows[j];
                    if (Row["Attributo"].ToString() == RowVal["Attributo"].ToString())
                    {
                        bFound = true;
                        sValue = RowVal["Valore"].ToString();
                    }
                }

                if (bFound)
                {
                    if (sValue.Length >= nLen)
                        sRow += sValue.Substring(0, nLen) + " ";
                    else
                        sRow += sValue + sBlank.Substring(0, nLen - sValue.Length) + " ";
                }
                else
                    sRow += sBlank + " ";
            }

            return sRow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringToTest"></param>
        /// <returns></returns>
        private static bool IsNumeric(string stringToTest)
        {
            double newVal;
            return double.TryParse(stringToTest, NumberStyles.Any,
                                   NumberFormatInfo.InvariantInfo, out newVal);
        }

        #endregion

        #region Nested type: PulseEventArgs

        public class PulseEventArgs : EventArgs
        {
            public PulseEventArgs()
              : this(0, string.Empty, string.Empty)
            {
            }

            public PulseEventArgs(int progress, string message, string method)
            {
                Progress = progress;
                Message = message;
                Method = method;
            }

            public int Progress { get; set; }
            public string Message { get; set; }
            public string Method { get; set; }

            public static PulseEventArgs Create()
            {
                return Create(0, string.Empty, string.Empty);
            }

            public static PulseEventArgs Create(int progress)
            {
                return Create(progress, string.Empty, string.Empty);
            }

            public static PulseEventArgs Create(int progress, string message)
            {
                return Create(progress, message, string.Empty);
            }

            public static PulseEventArgs Create(int progress, string message, string method)
            {
                return new PulseEventArgs(progress, message, method);
            }
        }

        #endregion

        public void AddVerifyPreservationToJournaling(Guid idPreservation, string domainUser)
        {
            logger.Info("AddPreservationJournaling - entry point");

            try
            {
                var activity = DbProvider.GetPreservationJournalingActivities(null).Where(x => x.KeyCode == "VerificaConservazione").SingleOrDefault();
                if (activity == null)
                    throw new Exception("Impossibile recuperare l'attività con codice \"VerificaConservazione\" dalla tabella \"PreservationJournalingActivities\".");

                DbProvider.AddPreservationJournaling(new PreservationJournaling { IdPreservation = idPreservation, PreservationJournalingActivity = activity, DateActivity = DateTime.Now, DateCreated = DateTime.Now, DomainUser = domainUser });

                logger.Info("AddPreservationJournaling - ritorno al chiamante");
            }
            catch (Exception exx)
            {
                logger.Error(exx);
                throw exx;
            }
        }

        public Objects.Document GetPreservedDocument(Guid idPreservation, string name)
        {
            return DbProvider.GetPreservedDocuments(idPreservation, name);
        }

        public void IsAlive()
        {
            //string DCACHE_NAME = "LocalDriveCache";
            //string DCACHE_LOCATION = "cache";
            //string STORAGE_ACCOUNT_SETTING = "TableStorageConnectionString";
            //string DRIVE_SETTINGS = "BiblosDS.Cdm.DriveSettings";
            //try
            //{
            //    logger.Info("storageAccount");
            //    // connect to the storage account
            //    CloudStorageAccount storageAccount = CloudStorageAccount.FromConfigurationSetting(STORAGE_ACCOUNT_SETTING);
            //    logger.Info("blobClient");
            //    // client for talking to our blob files
            //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //    logger.Info("drives");
            //    // the container that our dive is going to live in
            //    CloudBlobContainer drives = blobClient.GetContainerReference("drives");
            //    logger.Info("CreateIfNotExist");
            //    // create blob container (it has to exist before creating the cloud drive)
            //    try { drives.CreateIfNotExist(); }
            //    catch (Exception ex) { logger.Error(ex); }

            //    var driveSettings = RoleEnvironment.GetConfigurationSettingValue(DRIVE_SETTINGS);
            //    logger.InfoFormat("driveSettings:{0}", driveSettings);
            //    string[] settings = driveSettings.Split(':');

            //    // get the url to the vhd page blob we'll be using
            //    var vhdUrl = blobClient.GetContainerReference("drives").GetPageBlobReference(settings[0]).Uri.ToString();

            //    string dCacheName = RoleEnvironment.GetConfigurationSettingValue(DCACHE_NAME);
            //    logger.InfoFormat("dCacheName: {0}", dCacheName);

            //    LocalResource localCache = RoleEnvironment.GetLocalResource(dCacheName);
            //    // create the cloud drive
            //    logger.InfoFormat("CreateCloudDrive: {0}", vhdUrl);
            //   var _mongoDrive = storageAccount.CreateCloudDrive(vhdUrl);
            //    try
            //    {
            //        logger.InfoFormat("localCache.MaximumSizeInMegabytes: {0}", localCache.MaximumSizeInMegabytes);

            //        _mongoDrive.CreateIfNotExist(localCache.MaximumSizeInMegabytes);
            //    }
            //    catch (CloudDriveException ex)
            //    {
            //        logger.Error(ex);
            //        // exception is thrown if all is well but the drive already exists
            //    }

            //    // mount the drive and get the root path of the drive it's mounted as
            //    var dataPath = _mongoDrive.Mount(localCache.MaximumSizeInMegabytes, DriveMountOptions.Force) + @"";
            //    logger.InfoFormat("dataPath: {0}", dataPath);
            //    string path = Path.Combine(dataPath, "Test");
            //    if (!Directory.Exists(path))
            //        Directory.CreateDirectory(path);
            //    File.WriteAllText(Path.Combine(path, "prova0.txt"), "Test");
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //}
            try
            {
                logger.Info("IsAlive");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public ArchiveCompany AddArchiveCompany(ArchiveCompany archComp)
        {
            logger.Info("AddArchiveCompany - INIT");
            try
            {
                var ret = DbProvider.AddArchiveCompany(archComp);
                logger.Info("AddArchiveCompany - END");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public IDictionary<Guid, Exceptions.BiblosDsException> UpdateDocumentMetadata(Guid idArchive, IDictionary<Guid, BindingList<DocumentAttributeValue>> documentAttributes)
        {
            logger.Info("UpdateDocumentMetadata - INIT");
            IDictionary<Guid, Exceptions.BiblosDsException> result = new Dictionary<Guid, Exceptions.BiblosDsException>();
            try
            {
                var archive = ArchiveService.GetArchive(idArchive);
                if (archive == null)
                    throw new Exceptions.Archive_Exception("Archive not found.");
                var metadata = AttributeService.GetAttributesFromArchive(idArchive);
                foreach (var item in documentAttributes)
                {
                    try
                    {
                        var document = DocumentService.GetDocument(item.Key);
                        if (document == null)
                            throw new Exceptions.DocumentNotFound_Exception("Documento non trovato: " + item.Key.ToString());
                        document.AttributeValues = new BindingList<DocumentAttributeValue>();
                        foreach (var itemAttr in item.Value)
                        {
                            var biblosAttribute = metadata.Where(x => x.Name == itemAttr.Attribute.Name).FirstOrDefault();
                            if (biblosAttribute == null)
                            {
                                result.Add(item.Key, new Exceptions.BiblosDsException("Attribute not found: " + itemAttr.Attribute.Name, Exceptions.FaultCode.Attribute_Exception));
                                continue;
                            }
                            document.AttributeValues.Add(new DocumentAttributeValue { Attribute = biblosAttribute, IdAttribute = biblosAttribute.IdAttribute, Value = itemAttr.Value });
                        }
                        DocumentService.ResetDocumentAttributeValue(document);
                        result.Add(item.Key, null);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        result.Add(item.Key, new Exceptions.BiblosDsException(ex));
                    }
                }
                logger.Info("UpdateDocumentMetadata - END");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return result;
        }

        public bool LockTask(PreservationTask task)
        {
            try
            {
                logger.InfoFormat("LockTask {0}", task.IdPreservationTask);
                return DbProvider.LockTask(task);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public void UnlockTask(PreservationTask task)
        {
            try
            {
                logger.InfoFormat("UnlockTask {0}", task.IdPreservationTask);
                DbProvider.UnlockTask(task);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void ProcessExpiredTask()
        {
            logger.DebugFormat("ProcessTask");
            var service = new PreservationService();
            var tasks = service.GetPreservationExpireTask();
            PreservationTask itemLock = null;
            foreach (var task in tasks)
            {
                try
                {
                    itemLock = null;
                    if (!service.LockTask(task))
                    {
                        logger.InfoFormat("Task is LOCK {0}", task.IdPreservationTask);
                        continue;
                    }
                    itemLock = task;

                    switch (task.TaskType.Type)
                    {
                        case PreservationTaskTypes.VerifyPreservation:
                            {
                                logger.DebugFormat("VerifyPreservation Task: {0} {1}", task.EstimatedDate, task.IdPreservationTask);
                                if (task.IdPreservation != null)
                                    service.VerifyExistingPreservation((Guid)task.IdPreservation);
                            }
                            break;
                        case PreservationTaskTypes.Verify:
                            {
                                logger.DebugFormat("Verify Task: {0} {1}", task.EstimatedDate, task.IdPreservationTask);

                                var tasksToExecute = service.PreviousTaskToExecute(task, task.Archive.IdArchive);
                                if (tasksToExecute.Count > 0)
                                {
                                    logger.InfoFormat("Impossibile eseguire il task, eseguire prima i task precedenti {0}", tasksToExecute.Select(x => x.IdPreservationTask + ";" + x.StartDocumentDate + ";" + x.EndDocumentDate));
                                    continue;
                                }
                            }
                            break;
                        case PreservationTaskTypes.Preservation:
                            {
                                logger.DebugFormat("Preservation Task: {0} {1}", task.EstimatedDate, task.IdPreservationTask);

                                PreservationTask verifyPreservationTask = service.GetPreservationTask(task.IdCorrelatedPreservationTask.GetValueOrDefault());
                                if (verifyPreservationTask == null)
                                {
                                    logger.InfoFormat("Nessun task di verifica trovato. Eseguire il task di verifica per proseguire. {0}", task.IdPreservationTask);
                                    continue;
                                }
                                else if (!verifyPreservationTask.Executed)
                                {
                                    logger.InfoFormat("Il Task di verifica non risulta essere stato eseguito. Eseguire il task di verifica per proseguire. {0}", task.IdPreservationTask);
                                    continue;
                                }
                                else if (verifyPreservationTask.HasError)
                                {
                                    logger.InfoFormat("Il Task di verifica risulta essere stato eseguito con i seguenti errori {1}. {0}", task.IdPreservationTask, task.ErrorMessages);
                                    continue;
                                }

                                logger.InfoFormat("ExecutePreservationTask: {0}", task.IdPreservationTask);
                                var taskToExecute = new PreservationService().GetPreservationTask(task.IdPreservationTask);
                                logger.InfoFormat("ExecutePreservationTask INIT: {0}", task == null ? "Nullo" : task.IdPreservationTask.ToString());
                                var result = new PreservationService().CreatePreservation(taskToExecute);
                                logger.InfoFormat("ExecutePreservationTask END:  {0} - {1}", result.Error == null ? "No error" : result.Error.Message, result.Error == null ? "" : result.Error.StackTrace);

                                if (Convert.ToBoolean(ConfigurationManager.AppSettings["AutoGenerateNextTask"]) == true)
                                    new PreservationService().AutoGenerateNextTask(taskToExecute);
                            }
                            break;
                        case PreservationTaskTypes.Unknown:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    if (itemLock != null)
                        service.UnlockTask(itemLock);
                }

            }
        }

        public void AutoGenerateNextTask(PreservationTask task)
        {
            var schedule = DbProvider.GetPreservationScheduleWithinArchive(task.Archive.IdArchive);
            if (schedule == null)
            {
                logger.InfoFormat("Nessuna schedulazione creata per l'archivio: {0}", task.Archive.Name);
            }
            else
            {
                CreatePreservationTask(task.Archive.IdArchive, schedule, task.EndDocumentDate.Value.AddDays(1), true);
            }
        }

        public void CreatePreservationTask(Guid idArchive, PreservationSchedule schedule, DateTime date, bool isEnabled)
        {
            /*
            * Tipologie frequenze:
            * 0 - cadenzata
            * 1 - giornaliera
            * 2 - settimanale
            * 3 - mensile
            * 4 - annuale
            */

            if (schedule != null)
            {
                var taskToCreate = new List<PreservationTask>();
                taskToCreate.AddRange(getTasksFromSchedulerPeriod(schedule.Period, date.Month, date.Year, date.Day, true));

                foreach (var task in taskToCreate)
                {
                    task.TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = Library.Common.Objects.Enums.PreservationTaskTypes.Verify };
                    task.Archive = new DocumentArchive(idArchive);
                    task.Enabled = isEnabled;

                    task.CorrelatedTasks = new BindingList<PreservationTask>();
                    task.CorrelatedTasks.Add(new PreservationTask
                    {
                        TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = Library.Common.Objects.Enums.PreservationTaskTypes.Preservation },
                        Archive = new DocumentArchive(idArchive),
                        StartDocumentDate = task.StartDocumentDate,
                        EndDocumentDate = task.EndDocumentDate,
                        EstimatedDate = task.EstimatedDate,
                        Enabled = task.Enabled
                    });
                }

                //service.RemovePendigPreservation(idArchive);
                var result = CreatePreservationTask(new BindingList<PreservationTask>(taskToCreate));
                logger.InfoFormat("Creati i task: {0}", string.Join(";", result.Select(x => x.TaskType.Type + " " + x.StartDate + " - " + x.EndDate)));
            }
        }

        public void DeletePreservationTask(Guid idPreservationTask)
        {
            logger.InfoFormat("DeletePreservationTask - id preservationTask {0}", idPreservationTask);

            DbProvider.DeletePreservationTask(idPreservationTask);

            logger.Info("DeletePreservationJournaling - ritorno al chiamante");
        }

        public IEnumerable<PreservationTask> getTasksFromSchedulerPeriod(string encodedPeriod, int baseMonth, int baseYear, int baseDay, bool onlyOne)
        {
            var retval = new List<PreservationTask>();

            if (!string.IsNullOrWhiteSpace(encodedPeriod))
            {
                DateTime startDate = new DateTime(baseYear, baseMonth, baseDay), endDate;
                PreservationTask toAdd = new PreservationTask();
                var splittedPeriods = encodedPeriod.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string[] splittedMonthAndDay;
                int month = 0, day = 0, lastMonthDay;

                foreach (var period in splittedPeriods)
                {
                    splittedMonthAndDay = period.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splittedMonthAndDay.Count() > 1)
                    {
                        try
                        {
                            try
                            {
                                month = int.Parse(splittedMonthAndDay.First());
                                day = int.Parse(splittedMonthAndDay.Last());
                            }
                            catch { throw new Exception("Configurazione non corretta per il periodo dello scadenziario."); }

                            endDate = new DateTime(baseYear, month, day);
                            endDate = endDate.ToLocalTime().Date.AddDays(1).AddMilliseconds(-3);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            /*
                             * Se si verifica un'eccezione di tipo "ArgumentOutOfRangeException" è perchè
                             * l'ultimo giorno del mese configurato in "encodedPeriod" è superiore all'ultimo
                             * giorno del mese reale.
                             */
                            lastMonthDay = new DateTime(baseYear, baseMonth, 1).AddMonths(1).AddHours(-1).Day;
                            endDate = new DateTime(baseYear, baseMonth, lastMonthDay);
                            endDate = endDate.ToLocalTime().Date.AddDays(1).AddMilliseconds(-3);
                        }
                    }
                    else
                    {
                        try
                        {
                            try
                            {
                                day = int.Parse(period);
                                if (day <= baseDay)
                                    continue;
                            }
                            catch { throw new Exception("Configurazione non corretta per il periodo dello scadenziario."); }

                            endDate = new DateTime(baseYear, baseMonth, day == 31 ? DateTime.DaysInMonth(baseYear, baseMonth) : day);
                            endDate = endDate.ToLocalTime().Date.AddDays(1).AddMilliseconds(-3);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            /*
                             * Se si verifica un'eccezione di tipo "ArgumentOutOfRangeException" è perchè
                             * l'ultimo giorno del mese configurato in "encodedPeriod" è superiore all'ultimo
                             * giorno del mese reale.
                             */
                            lastMonthDay = new DateTime(baseYear, baseMonth, 1).AddMonths(1).AddHours(-1).Day;
                            endDate = new DateTime(baseYear, baseMonth, lastMonthDay);
                            endDate = endDate.ToLocalTime().Date.AddDays(1).AddMilliseconds(-3);
                        }
                    }

                    //considera periodo successivo dei splittedPeriods
                    if (endDate <= startDate)
                        continue;

                    toAdd = new PreservationTask { StartDocumentDate = startDate, EndDocumentDate = endDate, EstimatedDate = findNextValidWeekDay(endDate.Date) };
                    startDate = endDate.AddDays(1);

                    retval.Add(toAdd);
                    if (onlyOne)
                        break;
                }
            }

            return retval;
        }

        private DateTime findNextValidWeekDay(DateTime baseDate, bool includeBaseDateDay = false, params int[] validWeekDays)
        {
            DateTime retval = DateTime.MinValue;

            if (baseDate != null && baseDate != DateTime.MinValue && baseDate != DateTime.MaxValue)
            {
                try
                {
                    IEnumerable<DayOfWeek> validDays;

                    if (validWeekDays != null && validWeekDays.Any())
                    {
                        validDays = validWeekDays.Cast<DayOfWeek>();
                    }
                    else
                    {
                        validDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().Where(x => x != DayOfWeek.Saturday && x != DayOfWeek.Sunday);
                    }

                    if (includeBaseDateDay && validDays.Contains(baseDate.DayOfWeek))
                    {
                        retval = baseDate;
                    }
                    else
                    {
                        DateTime newDate;
                        for (newDate = baseDate.AddDays(1); !validDays.Contains(newDate.DayOfWeek); newDate = newDate.AddDays(1))
                        {
                            //NESSUNA ULTERIORE OPERAZIONE DA FARE.
                        }
                        retval = newDate;
                    }
                }
                catch { }
            }

            return retval;
        }


        /// <summary>
        /// Esegue la verifica di un supporto di conservazione 
        /// </summary>
        /// <param name="IdPreservation">GUID della conservazione</param>
        /// <returns></returns>
        public bool VerifyExistingPreservation(Guid IdPreservation)
        {
            logger.Info("VerifyExistingPreservation - INIT");

            bool bEsito = true;
            this.ErrorMessages = new List<string>();  // lista dei messaggi di errore 

            string CloseFile = ""; // nome del file di chiusura
            string IndexXmlFile = ""; //nome del file indice 
            string VerifyCloseContent = ""; // contenuto del file di chisura
            string PathToVerify = ""; // directory di conservazione da verificare 

            Objects.Preservation thisPreservation = GetPreservation(IdPreservation, false);
            if (thisPreservation == null)
                throw new Exception("Nessuna conservazione presente non l'id passato");

            PathToVerify = thisPreservation.Path;

            // verifica esistenza percorso della conservazione
            if (Directory.Exists(PathToVerify) == false)
                throw new Exception("Directory di conservazione : " + PathToVerify + " non trovata o non accessibile");

            this.ErrorMessages.Add(TESTO_SEPARATORE);
            this.ErrorMessages.Add("INIZIO [" + IdPreservation.ToString() + "] Verifica conservazione " + DateTime.Now.ToString("s") + " nel percorso " + PathToVerify);

            //test
            //CheckIpda(IpdaUtil.GetIpdaXmlFile(thisPreservation.Path), out this.verifiedIpda);
            //fine test

            //se esiste il file idpa..tsd esegue la verifica di questo altrimenti esegue la verifica standard del file di chiusura e gli altri
            ArchiveConfiguration archiveConfiguration = JsonConvert.DeserializeObject<ArchiveConfiguration>(thisPreservation.Archive.PreservationConfiguration);
            string ipdaTsdFile = IpdaUtil.GetIpdaTsdFile(PathToVerify);
            if (ipdaTsdFile != "")
            {
                bEsito = CheckIpdaTsd(ipdaTsdFile, out this.verifiedIpda, archiveConfiguration);
            }
            else
            {
                // Controlla il file di chiusura 
                if (CheckClosePreservationFileIntegrity(PathToVerify, out VerifyCloseContent, out CloseFile) == false)
                    bEsito = false;
                else
                {
                    if (CheckIndexPreservationFileIntegrity(CloseFile, VerifyCloseContent, out IndexXmlFile) == false)
                        bEsito = false;
                    else
                    {
                        if (CheckFilesPreservationIntegrity(PathToVerify, IndexXmlFile) == false)
                            bEsito = false;
                        else
                        {
                            //se esiste controlla file xml
                            string ipdaFile = IpdaUtil.GetIpdaXmlFile(PathToVerify);
                            if (ipdaFile != "")
                                bEsito = CheckIpda(ipdaFile, out this.verifiedIpda);
                        }
                    }
                }
            }

            // salva il report di verifica 
            string timeend = DateTime.Now.ToString("s");

            this.VerifyFile = timeend + " Verifica conservazione con esito ";
            this.VerifyFile = this.VerifyFile.Replace(":", "_");

            if (bEsito == true)
            {
                this.ErrorMessages.Add("Verifica conclusa con esito positivo");
                this.VerifyFile += "positivo.txt";
            }
            else
            {
                this.ErrorMessages.Add("Verifica conclusa con esito negativo");
                this.VerifyFile += "negativo.txt";
            }

            this.VerifyFile = Path.Combine(PathToVerify, this.VerifyFile);

            this.ErrorMessages.Add("FINE [" + IdPreservation.ToString() + "] Verifica conservazione " + timeend);
            ErrorMessages.Add(TESTO_SEPARATORE);

            File.WriteAllLines(this.VerifyFile, this.ErrorMessages.ToArray());

            // se l'esito positivo deve aggiornare la lastverifydate 
            if (bEsito == true)
            {
                Objects.Preservation preserv = DbProvider.GetPreservation(IdPreservation, false);
                preserv.LastVerifiedDate = DateTime.Now;
                DbProvider.UpdatePreservation(preserv, false);
            }

            // aggiorna il 
            PreservationJournaling journal = new PreservationJournaling
            {
                DateActivity = DateTime.Now,
                DateCreated = DateTime.Now,
                DomainUser = "BiblosDS",
                Preservation = thisPreservation,
                PreservationJournalingActivity =
              DbProvider.GetPreservationJournalingActivities(null).Where(x =>
              x.KeyCode.Equals("VerificaConservazione", StringComparison.InvariantCultureIgnoreCase)).Single(),
            };
            AddPreservationJournaling(journal);

            logger.Info("VerifyExistingPreservation - END");
            return bEsito;
        }



        /// <summary>
        /// verifica l'integrità dei file conservati
        /// </summary>
        /// <param name="FileIndexXML"></param>
        /// <returns></returns>
        private bool CheckFilesPreservationIntegrity(string PreservationPath, string FileIndexXML)
        {
            int presenti = 0;
            int nonpresenti = 0;
            int alterati = 0;

            this.ErrorMessages.Add("Verifica del file indice e dei singoli file conservati");

            XmlDocument indiceXml = new XmlDocument();
            try
            {
                indiceXml.Load(FileIndexXML);
            }
            catch (XmlException)
            {
                this.ErrorMessages.Add("[ANOMALIA] Il file di indice xml non è un documento xml valido : " + FileIndexXML);
                return false;
            }

            XmlNodeList nList = indiceXml.SelectNodes("//File");
            foreach (XmlNode node in nList)
            {
                string fileName = Path.Combine(PreservationPath, node.SelectSingleNode("Attributo[@Nome = \"NomeFileInArchivio\"]").InnerText);
                string shortFileName = Path.GetFileName(fileName);
                string fileHash = "";
                if (node.SelectSingleNode("Attributo[@Nome = \"ImprontaFileSHA1\"]") != null)
                    fileHash = node.SelectSingleNode("Attributo[@Nome = \"ImprontaFileSHA1\"]").InnerText;
                else
                    fileHash = node.SelectSingleNode("Attributo[@Nome = \"ImprontaFileSHA256\"]").InnerText;
                if (File.Exists(fileName))
                {
                    presenti++;

                    string filesha256 = CalculateHashCode(Path.Combine(PreservationPath, fileName), true);
                    string filesha1 = CalculateHashCode(Path.Combine(PreservationPath, fileName), false);

                    if ((fileHash != filesha256) && (fileHash != filesha1))
                    {
                        this.ErrorMessages.Add("[ANOMALIA] Il file '" + shortFileName + "' NON è integro sul supporto e l'impronta NON corrisponde.");
                        alterati++;
                    }
                }
                else
                {
                    this.ErrorMessages.Add("[ANOMALIA] Il file '" + shortFileName + "' NON è presente nel supporto della conservazione.");
                    nonpresenti++;
                }
            }

            bool bEsito = true;
            this.ErrorMessages.Add("Sono stati esaminati " + nList.Count.ToString() + " files, di cui " + presenti.ToString() + " presenti ed integri nel supporto");
            if (nonpresenti > 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] " + nonpresenti.ToString() + " files non sono presenti nel supporto");
                bEsito = false;
            }
            if (alterati > 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] " + alterati.ToString() + " files sono alterati con impronta non corrispondente");
                bEsito = false;
            }

            return bEsito;
        }



        /// <summary>
        /// verifica l'integrità del file indice
        /// </summary>
        /// <param name="VerifyCloseContent"></param>
        /// <returns></returns>
        private bool CheckIndexPreservationFileIntegrity(string CloseFile, string VerifyCloseContent, out string IndexXmlFile)
        {
            this.ErrorMessages.Add("Verifica integrità dei files indice");
            IndexXmlFile = "";

            // estrazione hash indice
            Match match = Regex.Match(VerifyCloseContent, TESTO_HASH_INDICE.Replace("(", "\\(").Replace(")", "\\)") + @"([0-9A-Fa-f]+)");
            string hashIndice = "";
            if (match.Groups.Count == 2)
                hashIndice = match.Groups[1].ToString();
            if (hashIndice.Length == 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] Nel file di chiusura manca l'evidenza informatica del file di indice.");
                return false;
            }

            // estrazione hash indice xml
            match = Regex.Match(VerifyCloseContent, TESTO_HASH_INDICE_XML.Replace("(", "\\(").Replace(")", "\\)") + @"([0-9A-Fa-f]+)");
            string hashIndiceXML = "";
            if (match.Groups.Count == 2)
                hashIndiceXML = match.Groups[1].ToString();

            if (hashIndiceXML.Length == 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] Nel file di chiusura manca l'evidenza informatica del file di indice XML.");
                return false;
            }

            string IndexFile = CloseFile.Replace("CHIUSURA", "INDICE");
            string sha256Index = CalculateHashCode(IndexFile, true);
            string sha1Index = CalculateHashCode(IndexFile, false);

            if ((hashIndice != sha256Index) && (hashIndice != sha1Index))
            {
                this.ErrorMessages.Add("[ANOMALIA] L'impronta del file indice non corrisponde: " + IndexFile);
                return false;
            }
            else
            {
                this.ErrorMessages.Add("L'impronta del file indice corrisponde: " + IndexFile);
            }

            IndexXmlFile = IndexFile.Replace(".txt", ".xml");
            string sha256IndexXML = CalculateHashCode(IndexXmlFile, true);
            string sha1IndexXML = CalculateHashCode(IndexXmlFile, false);

            if ((hashIndiceXML != sha256IndexXML) && (hashIndiceXML != sha1IndexXML))
            {
                this.ErrorMessages.Add("[ANOMALIA] L'impronta del file indice XML non corrisponde: " + IndexXmlFile);
                return false;
            }
            else
            {
                this.ErrorMessages.Add("L'impronta del file indice XML corrisponde: " + IndexXmlFile);
            }

            return true;
        }

        /// <summary>
        /// verifica ed estrazione del contenuto del file di chiusura
        /// </summary>
        /// <param name="PathToVerify"></param>
        /// <param name="VerifyCloseContent"></param>
        /// <returns></returns>
        private bool CheckClosePreservationFileIntegrity(string PathToVerify, out string VerifyCloseContent, out string CloseFile)
        {
            this.ErrorMessages.Add("Verifica integrità del file di chiusura firmato e marcato digitalmente");

            VerifyCloseContent = "";

            string ChiusuraTimestampedFileName = "";
            string ChiusuraSignedFileName = "";
            CloseFile = "";

            //controllo se esiste il file in formato ipda
            FileInfo[] filesTSD = new DirectoryInfo(PathToVerify).GetFiles("chiusura*.tsd");
            FileInfo[] filesM7M = new DirectoryInfo(PathToVerify).GetFiles("chiusura*.m7m");
            FileInfo[] filesP7M = new DirectoryInfo(PathToVerify).GetFiles("chiusura*.p7m");

            // determina il tipo di firma sul file di chisusura 
            // sono validi:
            // 2010 : Chiusura.txt + Chiusura.txt.p7m + Chiusura.txt.m7m
            // 2011 : Chiusura.txt + Chiusura.txt.p7m + Chiusura.txt.m7m oppure Chiusura.txt + Chiusura.txt.tsd oppure Chiusura.txt + Chiusura.txt.p7m + Chiusura.txt.p7m.tsd
            // 2012 : Chiusura.txt + Chiusura.txt.p7m + Chiusura.txt.p7m.tsd oppure Chiusura.txt + Chiusura.txt.tsd
            // 2013 : Chiusura.txt + Chiusura.txt.p7m + Chiusura.txt.p7m.tsd

            bool isP7MCloseFile = false;

            if (filesTSD.Length == 1)
            {
                if (filesP7M.Length == 1)
                {
                    // tsd + p7m 
                    ChiusuraTimestampedFileName = filesTSD[0].FullName;
                    ChiusuraSignedFileName = filesP7M[0].FullName;
                    isP7MCloseFile = true;
                    CloseFile = filesP7M[0].FullName.Replace(".p7m", "");
                }
                else
                {
                    // solo tsd
                    ChiusuraTimestampedFileName = filesTSD[0].FullName;
                    ChiusuraSignedFileName = filesTSD[0].FullName;
                    isP7MCloseFile = false;
                    CloseFile = filesTSD[0].FullName.Replace(".tsd", "");
                }
                if (filesM7M.Length == 1)
                {
                    this.ErrorMessages.Add("[NOTA] Rilevata la presenza di marca temporale TSD e formato InfoCamere M7M : viene considerato il file TSD");
                }
            }
            else if (filesP7M.Length == 1)
            {
                if (filesM7M.Length == 1)
                {
                    // p7m + m7m 
                    ChiusuraSignedFileName = filesP7M[0].FullName;
                    isP7MCloseFile = true;
                    ChiusuraTimestampedFileName = filesM7M[0].FullName;
                    CloseFile = filesP7M[0].FullName.Replace(".p7m", "");
                }
                else
                {
                    this.ErrorMessages.Add("[ANOMALIA] Non è stato trovato il file chiusura marcato M7M");
                    return false;
                }
            }
            else
            {
                // manca il file di chiusura firmato
                this.ErrorMessages.Add("[ANOMALIA] Non è stato trovato il file chiusura firmato");
                return false;
            }

            this.ErrorMessages.Add("File di chiusura con firma: " + ChiusuraSignedFileName);
            this.ErrorMessages.Add("File di chiusura con marca temporale: " + ChiusuraTimestampedFileName);

            #region sbusta il contenuto del file di chiusura firmato
            byte[] plainContent = null;
            byte[] p7mContent = null;
            string Metadata;

            p7mContent = File.ReadAllBytes(ChiusuraSignedFileName);

            CompEdLib comped = new CompEdLib();
            plainContent = comped.GetContent(isP7MCloseFile, p7mContent, out Metadata);
            if (isP7MCloseFile == false)
                plainContent = comped.GetContent(true, plainContent, out Metadata);
            comped.Dispose();
            #endregion

            #region verifica file di chiusura
            StreamReader srChiusura = new StreamReader(new MemoryStream(plainContent), Encoding.UTF8);
            VerifyCloseContent = srChiusura.ReadToEnd();

            if (VerifyCloseContent.Length == 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] nella verifica del file di chiusura della conservazione che appare vuoto.");
                return false;
            }

            string txtVerifyCloseContent = File.ReadAllText(CloseFile);

            if (VerifyCloseContent.ToLower() != txtVerifyCloseContent.ToLower())
            {
                this.ErrorMessages.Add("[NOTA] Il contenuto del file di chiusura e della sua versione firmata e marcata digitalmente non corrispondono.");
            }

            srChiusura.Close();
            #endregion

            this.ErrorMessages.Add("I file di chiusura sono firmati e marcati digitalmente e sono integri");
            return true;
        }


        /// <summary>
        /// Controlla la validità 
        /// </summary>
        /// <param name="ipdaTsdFile">File tsd da controllare</param>
        private bool CheckIpdaTsd(string ipdaTsdFile, out Ipda ipda, ArchiveConfiguration archiveConfiguration)
        {
            logger.Info("CheckIpdaTsd INIT");
            ipda = null;
            string filesPath = Path.GetDirectoryName(ipdaTsdFile);

            //1. confronto della versione firmata con quella non firmata xml
            this.ErrorMessages.Add("Verifica integrità del file di chiusura IPDA firmato e marcato digitalmente");

            string ChiusuraTimestampedFileName = ipdaTsdFile;
            string CloseFile = ipdaTsdFile.Replace(".tsd", "").Replace(".p7m", "");

            this.ErrorMessages.Add("File di chiusura con marca temporale: " + ChiusuraTimestampedFileName);

            #region sbusta il contenuto del file di chiusura firmato
            byte[] plainContent = null;
            byte[] p7mContent = null;
            string Metadata;

            p7mContent = File.ReadAllBytes(ChiusuraTimestampedFileName);
            if (archiveConfiguration.CheckTsd)
            {
                using (CompEdLib comped = new CompEdLib())
                {
                    plainContent = comped.GetContent(false, p7mContent, out Metadata);
                    plainContent = comped.GetContent(true, (plainContent != null ? plainContent : p7mContent), out Metadata);

                    if (plainContent == null)
                    {
                        this.ErrorMessages.Add("[ANOMALIA] nella verifica del file di chiusura della conservazione che non appare valido.");
                        return false;
                    }

                    comped.Close();
                }
                #endregion

                #region verifica file di chiusura
                logger.Info("verifica file di chiusura INIT");
                using (StreamReader srChiusura = new StreamReader(new MemoryStream(plainContent), Encoding.UTF8))
                {
                    string VerifyCloseContent = srChiusura.ReadToEnd();

                    if (VerifyCloseContent.Length == 0)
                    {
                        this.ErrorMessages.Add("[ANOMALIA] nella verifica del file di chiusura della conservazione che appare vuoto.");
                        return false;
                    }

                    string txtVerifyCloseContent = File.ReadAllText(CloseFile);

                    if (VerifyCloseContent.ToLower() != txtVerifyCloseContent.ToLower())
                    {
                        this.ErrorMessages.Add("[NOTA] Il contenuto del file di chiusura e della sua versione firmata e marcata digitalmente non corrispondono.");
                        return false;
                    }
                }
            }
            else
            {
                this.ErrorMessages.Add("[NOTA] verifica del file tsd a norme EIDAS disabilitata");
            }
            logger.Info("Verifica file di chiusura END");
            #endregion

            this.ErrorMessages.Add("I file di chiusura sono firmati e marcati digitalmente e sono integri");

            //2. verifica del file di chiusura, per comodità viene usata la versione xml plain, 
            //in quanto quella firmata è già stata confrontata con questa
            logger.Info("CheckIpdaTsd END");

            return CheckIpda(CloseFile, out ipda);
        }


        private bool CheckIpda(string ipdaFile, out Ipda ipda)
        {
            string filesPath = Path.GetDirectoryName(ipdaFile);
            ipda = Ipda.Load(ipdaFile);

            this.ErrorMessages.Add("Verifica file xml IPDA");

            //verifica hash dei metadati esterni
            foreach (var item in ipda.descGenerale.extraInfo.metadatiEsterniList)
            {
                string fileName = Path.Combine(filesPath, item.indirizzo);

                if (CalculateHashCode(fileName, true) != item.impronta)
                {
                    if (CalculateHashCode(fileName, false) != item.impronta)
                    {
                        this.ErrorMessages.Add("[ANOMALIA] L'impronta del file non corrisponde: " + item.indirizzo);
                        return false;
                    }
                }

                this.ErrorMessages.Add("L'impronta del file indice corrisponde: " + item.indirizzo);
            }

            //verifica hash dei documenti
            int presenti = 0;
            int nonpresenti = 0;
            int alterati = 0;

            foreach (var item in ipda.fileGruppo.files)
            {
                string fileName = Path.Combine(filesPath, item.id);

                if (File.Exists(fileName))
                {
                    presenti++;

                    if (CalculateHashCode(fileName, true) != item.impronta)
                    {
                        if (CalculateHashCode(fileName, false) != item.impronta)
                        {
                            this.ErrorMessages.Add("[ANOMALIA] Il file '" + item.id + "' NON è integro sul supporto e l'impronta NON corrisponde.");
                            alterati++;
                        }
                    }
                }
                else
                {
                    this.ErrorMessages.Add("[ANOMALIA] Il file '" + item.id + "' NON è presente nel supporto della conservazione.");
                    nonpresenti++;
                }
            }

            bool bEsito = true;
            this.ErrorMessages.Add("Sono stati esaminati " + ipda.fileGruppo.files.Length.ToString() + " files, di cui " + presenti.ToString() + " presenti ed integri nel supporto");
            if (nonpresenti > 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] " + nonpresenti.ToString() + " files non sono presenti nel supporto");
                bEsito = false;
            }
            if (alterati > 0)
            {
                this.ErrorMessages.Add("[ANOMALIA] " + alterati.ToString() + " files sono alterati con impronta non corrispondente");
                bEsito = false;
            }

            return bEsito;
        }

        public void GenerateZipPreservationPDA(Objects.Preservation preservation, string zipFileName, bool includeDocuments)
        {
            try
            {
                string fileExtension = Path.GetExtension(zipFileName);
                if (!fileExtension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("Il parametro zipFileName deve contenere il nome dello zip da generare", nameof(zipFileName));
                }

                string destination = Path.Combine(preservation.Path, zipFileName);
                using (IWritableArchive archive = ArchiveFactory.Create(ArchiveType.Zip))
                {
                    string[] files = Directory.GetFiles(preservation.Path);
                    if (!includeDocuments)
                    {
                        files = files.Where(x => x.Contains("INDICE_") || x.Contains("CHIUSURA_") || x.Contains("IPDA_") || x.Contains("LOTTI_")).ToArray();
                    }

                    foreach (string file in files.Where(x => Path.GetExtension(x) != ".zip" && !x.Contains("Verifica conservazione con esito")))
                    {
                        archive.AddEntry(Path.GetFileName(file), file);
                    }
                    archive.SaveTo(destination, CompressionType.Deflate);
                }
                logger.Debug(string.Concat("GenerateZipPreservationPDA -> created zip file ", destination));
            }
            catch (Exception ex)
            {
                logger.Error(string.Concat("GenerateZipPreservationPDA error -> ", ex.Message), ex);
                throw;
            }
        }

        public string GetZipPreservationPDA(Objects.Preservation preservation, bool includeDocuments)
        {
            try
            {
                string zipFileName = string.Concat(GetPreservationName(preservation), includeDocuments ? "_con_documenti" : "_senza_documenti", ".zip");
                string fullPath = Path.Combine(preservation.Path, zipFileName);
                if (File.Exists(fullPath))
                {
                    logger.Debug(string.Concat("GetZipPreservationPDA -> zip ", fullPath, " already exists. Return existed content."));
                    return fullPath;
                }

                logger.Debug(string.Concat("GetZipPreservationPDA -> generate zip file ", fullPath, "."));
                GenerateZipPreservationPDA(preservation, zipFileName, includeDocuments);
                return fullPath;
            }
            catch (Exception ex)
            {
                logger.Error(string.Concat("GetZipPreservationPDA error -> ", ex.Message), ex);
                throw;
            }
        }

        public ICollection<PreservationTask> CreateClosePreviousPreservationTask(PreservationTask limitPreservationTask, int startYear)
        {
            DateTime endDocumentDate = (startYear != limitPreservationTask.StartDocumentDate.Value.Year) ? new DateTime(startYear, 12, 31) : limitPreservationTask.StartDocumentDate.Value.AddDays(-1);
            PreservationTask preservationTask = new PreservationTask()
            {
                Archive = new DocumentArchive(limitPreservationTask.Archive.IdArchive),
                Enabled = true,
                TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = PreservationTaskTypes.Verify },
                StartDocumentDate = new DateTime(startYear, 1, 1),
                EndDocumentDate = endDocumentDate,
                EstimatedDate = endDocumentDate.AddDays(1)
            };

            preservationTask.CorrelatedTasks = new BindingList<PreservationTask>();
            preservationTask.CorrelatedTasks.Add(new PreservationTask
            {
                TaskType = new PreservationTaskType { IdPreservationTaskType = Guid.Empty, Type = PreservationTaskTypes.CloseAnnualPreservation },
                Archive = new DocumentArchive(limitPreservationTask.Archive.IdArchive),
                StartDocumentDate = preservationTask.StartDocumentDate,
                EndDocumentDate = preservationTask.EndDocumentDate,
                EstimatedDate = preservationTask.EstimatedDate,
                Enabled = preservationTask.Enabled
            });
            return CreatePreservationTask(new BindingList<PreservationTask>() { preservationTask });
        }
    }
}