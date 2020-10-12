using BiblosDS.Library.Common.DB;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BiblosDS.Library.Common.Services
{
    public class ArchiveService : ServiceBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ArchiveService));
        /// <summary>
        /// Ritorna il DocumentArchive individuato dall'ID
        /// </summary>
        /// <param name="idArchive">GUID del document Archive</param>
        /// <returns>DocumentArchive</returns>
        public static DocumentArchive GetArchive(Guid idArchive)
        {
            try
            {
                return DbProvider.GetArchive(idArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ArchiveService." + MethodBase.GetCurrentMethod().Name, e.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }
        /// <summary>
        /// Ritorna il DocumentArchive individuato dall'ID, più le eventuali configurazioni { ARCHIVIO , SERVER } ad esso associato.
        /// </summary>
        /// <param name="idArchive">GUID del document Archive</param>
        /// <returns>DocumentArchive</returns>
        public static DocumentArchive GetArchiveWithServerConfigs(Guid idArchive)
        {
            try
            {
                return DbProvider.GetArchiveWithServerConfigs(idArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ArchiveService." + MethodBase.GetCurrentMethod().Name, e.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Ritorna il DocumentArchive selezionato 
        /// </summary>
        /// <param name="archiveName">nome archivio</param>
        /// <returns>DocumentArchive</returns>
        public static DocumentArchive GetArchiveByName(string archiveName)
        {
            try
            {
                return GetArchives().Where(x => x.Name == archiveName).FirstOrDefault();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Ritorna la lista degli archivi legali.
        /// </summary>
        /// <returns></returns>
        public static BindingList<PreservationArchiveInfoResponse> GetLegalArchives(string domainUserName, Guid? idCompany)
        {
            return DbProvider.GetLegalArchives(domainUserName, idCompany);
        }

        /// <summary>
        /// Ritorna l'elenco dei document archive
        /// </summary>
        /// <returns>BindingList<DocumentArchive/></returns>
        public static BindingList<DocumentArchive> GetArchives()
        {
            try
            {
                return DbProvider.GetArchives();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentArchive> GetArchives(int skip, int take, DocumentCondition filter, List<DocumentSortCondition> sort, out int totalItems, Guid idCompany)
        {
            try
            {
                return DbProvider.GetArchives(skip, take, filter, sort, out totalItems, idCompany);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentArchive> GetArchivesById(IEnumerable<Guid> idsArchive)
        {
            try
            {
                return DbProvider.GetArchivesById(idsArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentArchive> GetArchivesByIdPaged(IEnumerable<Guid> idsArchive, int skip, int take, out int total, Guid idCompany)
        {
            try
            {
                return DbProvider.GetArchivesByIdPaged(idsArchive, skip, take, out total, idCompany);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Restituisce l'elenco dei DocumentArchive assoociati allo Storage
        /// </summary>
        /// <param name="storage">DocumentStorage di cui ottenere gli archivi associati</param>
        /// <returns>BindingList<DocumentArchive/></returns>
        public static BindingList<DocumentArchive> GetArchivesFromStorage(DocumentStorage storage)
        {
            try
            {
                return DbProvider.GetArchivesFromStorage(storage.IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Restituisce l'elenco dei DocumentArchive NON assoociati allo Storage
        /// </summary>
        /// <param name="Storage">DocumentStorage per cui ottenere gli archivi NON ancora associati</param>
        /// <returns>BindingList<DocumentArchive/></returns>
        public static BindingList<DocumentArchive> GetArchivesNotRelatedToStorage(DocumentStorage Storage)
        {
            try
            {
                return DbProvider.GetArchivesNotRelatedToStorage(Storage.IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        #region Archive Admin

        /// <summary>
        /// Inserisce un nuovo Archive
        /// </summary>
        /// <param name="Archive">DocumentArchive da inserire</param>
        public static Guid AddArchive(DocumentArchive Archive)
        {
            // IsLegal è sempre true o false e LastIdBiblos non è mai null
            if (String.IsNullOrEmpty(Archive.Name))
            {
                throw new Exception("Impossibile proseguire. Name deve essere valorizzato");
            }

            return DbAdminProvider.AddArchive(Archive);
        }

        /// <summary>
        /// Salva le modifiche a un DocumentArchive
        /// </summary>
        /// <param name="Archive">DocumentArchive modificato</param>
        public static void UpdateArchive(DocumentArchive Archive, bool convertDateMain = true)
        {
            // IsLegal è sempre true o false e LastIdBiblos non è mai null
            DocumentArchive dbArchive = DbProvider.GetArchive(Archive.IdArchive);
            if (Archive.IdArchive == Guid.Empty
                || String.IsNullOrEmpty(Archive.Name))
            {
                throw new Exception("Impossibile proseguire. IdArchive e Name devono essere valorizzati");
            }

            DbAdminProvider.UpdateArchive(Archive);

            if (convertDateMain && dbArchive.IsLegal != Archive.IsLegal && Archive.IsLegal && DbProvider.ExistMainDateAttribute(Archive.IdArchive))
            {
                ConvertArchiveConservationEnabled(Archive);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idArchive"></param>
        /// <returns></returns>
        public static ArchiveStatistics GetArchiveStatistics(Guid idArchive)
        {
            try
            {
                return DbProvider.GetArchiveStatistics(idArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static Guid CloneArchive(string templateName, string archiveName)
        {
            DocumentArchive archiveToClone = null, existingArchive;
            List<DocumentAttribute> attributesAdded = new List<DocumentAttribute>();
            List<DocumentStorageArea> storageAreasAdded = new List<DocumentStorageArea>();
            List<DocumentStorage> storagesAdded = new List<DocumentStorage>();
            try
            {
                if (string.IsNullOrWhiteSpace(archiveName))
                {
                    throw new BiblosDS.Library.Common.Exceptions.Archive_Exception(string.Format("Destination archive name is null or empty for source archive {0}.", templateName));
                }

                archiveToClone = ArchiveService.GetArchiveByName(templateName);
                if (archiveToClone == null)
                {
                    throw new BiblosDS.Library.Common.Exceptions.Archive_Exception(string.Format("Archive with name {0} not found.", templateName));
                }
                //Il nome può essere lungo al massimo 63 caratteri.
                if (archiveName.Length > 63)
                {
                    archiveName = archiveName.Remove(63);
                }
                //Controlla se per caso l'archivio che si desidera clonare è già presente in banca dati.
                existingArchive = ArchiveService.GetArchiveByName(archiveName);

                if (existingArchive != null)
                {
                    return existingArchive.IdArchive;
                }

                Guid idArchiveToClone = archiveToClone.IdArchive;
                archiveToClone.IdArchive = Guid.NewGuid();
                archiveToClone.Name = Regex.Replace(archiveName, @"[^a-zA-Z0-9]", "-");
                if (!string.IsNullOrEmpty(archiveToClone.PathTransito) && archiveToClone.PathTransito.LastIndexOf('\\') > 0)
                {
                    archiveToClone.PathTransito = Path.Combine(archiveToClone.PathTransito.Substring(0, archiveToClone.PathTransito.LastIndexOf('\\')), archiveToClone.Name);
                }

                if (!string.IsNullOrEmpty(archiveToClone.PathPreservation) && archiveToClone.PathPreservation.LastIndexOf('\\') > 0)
                {
                    archiveToClone.PathPreservation = Path.Combine(archiveToClone.PathPreservation.Substring(0, archiveToClone.PathPreservation.LastIndexOf('\\')), archiveToClone.Name);
                }

                DbAdminProvider.AddArchive(archiveToClone);
                //Clone attributes
                var attributes = AttributeService.GetAttributesFromArchive(idArchiveToClone);
                foreach (var attributeItem in attributes)
                {
                    var attribute = new DocumentAttribute { IdAttribute = Guid.NewGuid(), Archive = archiveToClone, AttributeType = attributeItem.AttributeType, ConservationPosition = attributeItem.ConservationPosition, DefaultValue = attributeItem.DefaultValue, Description = attributeItem.Description, Disabled = attributeItem.Disabled, Format = attributeItem.Format, IsAutoInc = attributeItem.IsAutoInc, IsChainAttribute = attributeItem.IsChainAttribute, IsEnumerator = attributeItem.IsEnumerator, IsMainDate = attributeItem.IsMainDate, IsRequired = attributeItem.IsRequired, IsRequiredForPreservation = attributeItem.IsRequiredForPreservation, IsSectional = attributeItem.IsSectional, IsUnique = attributeItem.IsUnique, IsVisible = attributeItem.IsVisible, IsVisibleForUser = attributeItem.IsVisibleForUser, KeyFilter = attributeItem.KeyFilter, KeyFormat = attributeItem.KeyFormat, KeyOrder = attributeItem.KeyOrder, MaxLenght = attributeItem.MaxLenght, Mode = attributeItem.Mode, Name = attributeItem.Name, Validation = attributeItem.Validation };
                    DbAdminProvider.AddAttribute(attribute);
                    attributesAdded.Add(attribute);
                }
                //Retrive storage
                var storage = DbProvider.GetStoragesActiveFromArchive(idArchiveToClone);
                foreach (var storageItem in storage)
                {
                    var storageArea = DbProvider.GetStorageAreaActiveFromStorage(storageItem.IdStorage);
                    if (storageArea.Any(x => x.Archive != null && x.Archive.IdArchive == idArchiveToClone))
                    {
                        storageArea = new BindingList<DocumentStorageArea>(storageArea.Where(x => x.Archive != null && x.Archive.IdArchive == idArchiveToClone).ToList());
                    }

                    foreach (var storageAreaItem in storageArea)
                    {
                        var newStorageArea = new DocumentStorageArea { IdStorageArea = Guid.NewGuid(), Archive = archiveToClone, Storage = storageItem, Enable = true, MaxFileNumber = storageAreaItem.MaxFileNumber, MaxSize = storageAreaItem.MaxSize, Name = archiveToClone.Name, Path = archiveToClone.Name, Priority = storageAreaItem.Priority, Status = storageAreaItem.Status };
                        DbAdminProvider.AddStorageArea(newStorageArea);
                        storageAreasAdded.Add(newStorageArea);
                    }
                    DbAdminProvider.AddArchiveStorage(new DocumentArchiveStorage { Storage = storageItem, Archive = archiveToClone, Active = true });
                    storagesAdded.Add(storageItem);
                }
                return archiveToClone.IdArchive;
            }
            catch (Exception)
            {
                try
                {
                    if (archiveToClone != null)
                    {
                        foreach (var item in storagesAdded)
                        {
                            DbAdminProvider.DeleteArchiveStorage(new DocumentArchiveStorage { Archive = archiveToClone, Storage = item }); ;
                        }
                        foreach (var item in storageAreasAdded)
                        {
                            DbAdminProvider.DeleteStorageArea(item.IdStorageArea);
                        }
                        foreach (var item in attributesAdded)
                        {
                            DbAdminProvider.DeleteAttribute(item.IdAttribute, false);
                        }
                        DbAdminProvider.DeleteArchive(archiveToClone);
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn(ex);
                    throw;
                }
                throw;
            }
        }

        public static Company GetCompany(Guid idCompany)
        {
            logger.InfoFormat("GetCompany - id company {0}", idCompany);
            try
            {
                var ret = DbProvider.GetCompany(idCompany);
                logger.Info("GetCompany - END");
                return ret;
            }
            catch (Exception e)
            {
                logger.Error(e);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static Company GetCompanyFromArchive(Guid idArchive)
        {
            logger.InfoFormat("GetCompanyFromArchive - id company {0}", idArchive);
            try
            {
                var ret = DbProvider.GetCompanyFromArchive(idArchive);
                logger.Info("GetCompanyFromArchive - END");
                return ret;
            }
            catch (Exception e)
            {
                logger.Error(e);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static Company AddCompany(Company company)
        {
            logger.Info("AddCompany - INIT");
            try
            {
                var ret = DbProvider.AddCompany(company);
                logger.Info("AddCompany - END");
                return ret;
            }
            catch (Exception e)
            {
                logger.Error(e);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                      "ArchiveService." + MethodBase.GetCurrentMethod().Name,
                                      e.ToString(),
                                      LoggingOperationType.BiblosDS_General,
                                      LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static ArchiveCompany AddArchiveCompany(ArchiveCompany archComp)
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

        public static void AddArchiveCertificate(Guid idArchive, string usreName, string pin, string fileName, byte[] certificateBlob)
        {
            if (certificateBlob == null || string.IsNullOrEmpty(usreName) || string.IsNullOrEmpty(pin))
            {
                throw new Exceptions.Generic_Exception("Parametri insufficienti.");
            }

            var archive = GetArchive(idArchive);
            if (archive == null)
            {
                throw new Exceptions.Archive_Exception("Archive not exist.");
            }

            DbProvider.AddArchiveCertificate(idArchive, usreName, pin, fileName, certificateBlob);
        }

        public static DocumentArchiveCertificate GetArchiveCertificate(Guid idArchive)
        {
            try
            {
                return DbProvider.GetArchiveCertificate(idArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ArchiveService." + MethodBase.GetCurrentMethod().Name, e.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        private static void ConvertArchiveConservationEnabled(DocumentArchive archive)
        {
            EntityProvider currentProvider = DbProvider;
            ICollection<Document> toUpdateDocuments = currentProvider.GetPreservationDocumentsNoDateMain(archive);
            currentProvider.BeginNoSaveNoTransaction();
            foreach (Document doc in toUpdateDocuments)
            {
                BindingList<DocumentAttributeValue> docAttributes = currentProvider.GetFullDocumentAttributeValues(doc.IdDocument);
                AttributeService.ParseAttributeValues(archive, docAttributes, out DateTime? mainDate);
                if (mainDate.HasValue)
                {
                    Guid mainDateAttributeId = docAttributes.Where(x => x.Attribute.IsMainDate == true).Single().IdAttribute;
                    doc.DateMain = mainDate;
                    currentProvider.UpdateDateMain(doc.IdDocument, mainDate);
                    logger.Info($"ConvertArchiveConservationEnabled -> IdDocument: {doc.IdDocument} - IdArchive: {archive.IdArchive} - MainDate attribute Id: {mainDateAttributeId} - MainDate value: {mainDate:dd/MM/yyyy HH:mm:ss}");
                }                
            }
            currentProvider.SaveChanges();
        }

        public static bool IsArchiveRedirectable(Guid idArchive, out ArchiveServerConfig archiveServerConfig)
        {
            DocumentArchive currentArchive = DbProvider.GetArchive(idArchive);
            archiveServerConfig = null;
            if (currentArchive == null)
            {
                throw new Exceptions.Archive_Exception($"Archive {idArchive} not found");
            }

            ArchiveServerConfig serverConfig = DbProvider.GetArchiveServerConfig(idArchive);
            if (serverConfig != null && serverConfig.Server.ServerRole == ServerRole.Remote)
            {
                archiveServerConfig = serverConfig;
                return true;
            }
            return false;
        }

        public static void UpdateArchiveLastIdBiblos(Guid idArchive, int lastIdBiblos)
        {
            DocumentArchive currentArchive = DbProvider.GetArchive(idArchive);
            if (currentArchive == null)
            {
                throw new Exceptions.Archive_Exception($"Archive {idArchive} not found");
            }

            if (lastIdBiblos < currentArchive.LastIdBiblos)
            {
                throw new Exceptions.Archive_Exception($"Non è possibile impostare il valore {lastIdBiblos} in quanto è inferiore all'ultimo IdBiblos ({currentArchive.LastIdBiblos}) impostato per l'archivio.");
            }

            DbProvider.UpdateArchiveBiblosId(idArchive, lastIdBiblos);
        }

        public static DocumentArchive GetArchiveFromPreservation(Guid idPreservation)
        {
            return DbProvider.GetArchiveFromPreservation(idPreservation);
        }

        /// <summary>
        /// Ritorna la lista degli archivi non definiti per la conservazione.
        /// </summary>
        /// <returns></returns>
        public static ICollection<DocumentArchive> GetPreservationArchivesConfigurable(Guid? idCompany, string archiveName, int? skip, int? top, out int totalItems)
        {
            return DbProvider.GetPreservationArchivesConfigurable(idCompany, archiveName, skip, top, out totalItems);
        }

        public static ArchiveCompany UpdateArchiveCompany(ArchiveCompany archComp)
        {
            try
            {
                return DbProvider.UpdateArchiveCompany(archComp);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
