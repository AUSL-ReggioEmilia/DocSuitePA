using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

using BiblosDS.Library.Common.Objects;
using System.IO;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.DB;
using System.Security.Principal;
using BiblosDS.Library.Common.Objects.Response;
using System.Data.Common;
using BiblosDS.Library.Common.Objects.UtilityService;
using BiblosDS.Library.Common.Utility;
using VecompSoftware.ServiceContract.BiblosDS.Documents;
using BiblosDS.Library.Common.Objects.Enums;

namespace BiblosDS.Library.Common.Services
{
    public partial class DocumentService : ServiceBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DocumentService));
        static object objLock = new object();
        /// <summary>
        /// controlla se il provider database è connettibile 
        /// </summary>
        /// <returns>true se connesso, false altrimenti</returns>
        public static bool IsConnected()
        {
            try
            {
                DbProvider.GetDatabaseVersion();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void DeleteChain(Guid idChain, bool visibility)
        {
            if (DbProvider.GetDocument(idChain) != null)
            {
                DbProvider.DeleteChain(idChain, visibility);
            }
            else
            {
                throw new Exceptions.DocumentNotFound_Exception();
            }
        }

        public static void DeleteDocument(Guid IDDocument, bool visibility)
        {
            DbProvider.DeleteDocument(IDDocument, visibility);
        }

        public static void DeleteDocumentAttach(Guid idDocumentAttach, bool visibility)
        {
            DbProvider.DeleteDocumentAttach(idDocumentAttach, visibility);
        }

        public static BindingList<Document> GetDocumentInTransitoToProcess(int MaxRetry)
        {
            return DbProvider.GetDocumentInTransito(MaxRetry, true);
        }

        public static DocumentResponse GetDocumentInTransitoByArchive(string archiveName, int take, int skip)
        {
            return DbProvider.GetDocumentInTransitoByArchive(archiveName, take, skip);
        }

        public static BindingList<Document> GetDocumentInTransito(int MaxRetry)
        {
            return DbProvider.GetDocumentInTransito(MaxRetry, false);
        }

        public static BindingList<Document> GetDocumentInTransito(Guid IdArchive, int MaxRetry)
        {
            return DbProvider.GetDocumentInTransito(IdArchive, MaxRetry);
        }

        public static BindingList<Document> GetDocumentsInStorage(Guid IdStorage)
        {
            return DbAdminProvider.GetDocumentsInStorage(IdStorage);
        }

        /// <summary>
        /// Restituisce il documento dato un id e una versione
        /// </summary>
        /// <param name="IdDocument">Id del documento desiderato</param>
        /// <param name="VersionNumber">Versione. Parametro di default è null</param>
        /// <returns></returns>
        public static Document GetDocument(Guid IdDocument, decimal? VersionNumber = null)
        {
            return DbProvider.GetDocument(IdDocument, VersionNumber);
        }

        public static decimal GetDocumentVersion(Guid IdDocument)
        {
            return DbProvider.GetDocumentVersion(IdDocument);
        }

        public static DocumentResponse GetDocumentsPaged(IEnumerable<Guid> idDocuments, int skip = 0, int take = -1)
        {
            return DbProvider.GetDocumentsPaged(idDocuments, skip, take);
        }

        public static Document GetDocumentLatestVersion(Guid IdDocument)
        {
            var result = DbProvider.GetDocumentLatestVersion(IdDocument);
            // FG20130513: Qualora non trovassi nessun documento marcato come ultima versione, recupero il primo documento nella lista.
            // Questa casistica è stata introdotta per retrocompatibilità con i documenti archiviati quando la funzionalità di versioning non era ancora stata implementata.
            if (result == null)
                result = DbProvider.GetDocument(IdDocument);
            return result;
        }

        public static Document GetDocument(int IdBoblos, Guid IdArchive)
        {
            return DbProvider.GetDocument(IdBoblos, IdArchive);
        }

        public static Document GetDocumentChild(int IdBoblos, Guid IdArchive)
        {
            return DbProvider.GetDocumentChild(IdBoblos, IdArchive);
        }

        public static Document GetDocumentDirect(int IdBoblos, Guid IdArchive)
        {
            return DbProvider.GetDocumentDirect(IdBoblos, IdArchive);
        }

        public static BindingList<Document> GetDocumentsByAttributeValue(DocumentArchive archive, DocumentAttributeValue attributeValue, bool? stringContains, int? skip, int? take)
        {
            return DbProvider.GetDocumentsByAttributeValue(archive, attributeValue, stringContains, skip, take);
        }

        public static BindingList<Document> GetAllDocumentChains(DocumentArchive archive, int skip, int take, out int documentsInArchive)
        {
            return DbProvider.GetAllDocumentChains(archive, skip, take, out documentsInArchive);
        }

        public static BindingList<Document> GetAllDocuments(DocumentArchive archive, bool visible, int skip, int take, out int documentsInArchive)
        {
            return DbProvider.GetAllDocuments(archive, visible, skip, take, out documentsInArchive);
        }

        public static BindingList<Document> GetAllDocumentsExt(DocumentArchive archive, bool visible, int skip, int take, Func<IQueryable<Model.Document>, object, IQueryable<Model.Document>> queryExt, object pars, out int documentsInArchive)
        {
            return DbProvider.GetAllDocumentsExt(archive, visible, skip, take, queryExt, pars, out documentsInArchive);
        }

        public static BindingList<Document> GetChainDocuments(Guid IdParentDocument)
        {

            return DbProvider.GetChainDocuments(IdParentDocument);
        }

        public static BindingList<Document> GetDocumentChildren(Guid idParent)
        {
            return DbProvider.GetDocumentChildren(idParent);
        }

        public static BindingList<Document> GetDocumentChildrenIgnoreState(Guid idParent)
        {
            return DbProvider.GetDocumentChildrenIgnoreState(idParent);
        }

        public static BindingList<Document> GetChainHistoryDocuments(Guid IdParentDocument)
        {
            return DbProvider.GetChainHistoryDocuments(IdParentDocument);
        }

        public static BindingList<Document> SearchDocuments(BindingList<DocumentCondition> conditions, out int documentsInArchiveCount, int? skip = null, int? take = null, Guid? idArchive = null, bool? latestVersion = false)
        {
            return DbProvider.SearchByAttributeCondition(conditions, out documentsInArchiveCount, skip, take, null, idArchive, latestVersion);
        }

        public static BindingList<Document> SearchDocuments(BindingList<DocumentCondition> conditions, out int documentsInArchiveCount, int? skip, int? take, bool? thumbnail = null)
        {
            return DbProvider.SearchByAttributeCondition(conditions, out documentsInArchiveCount, skip, take, thumbnail);
        }

        public static BindingList<Document> SearchDocumentsExt(Guid IdArchive, BindingList<DocumentCondition> conditions, out int documentsInArchiveCount, int? skip, int? take)
        {
            return SearchDocumentsExt(IdArchive, conditions, null, null, out documentsInArchiveCount, skip, take);
        }

        public static BindingList<Document> SearchDocumentsExt(Guid IdArchive, BindingList<DocumentCondition> conditions, Func<IQueryable<Model.Document>, object, IQueryable<Model.Document>> queryExt, object pars, out int documentsInArchiveCount, int? skip, int? take, bool? thumbnail = null)
        {
            return DbProvider.SearchByAttributeConditionExt(IdArchive, conditions, queryExt, pars, out documentsInArchiveCount, skip, take, thumbnail);
        }


        /// <summary>
        /// Aggiunta di un documento
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static Document AddDocument(Document Document, Server server)
        {
            if (Document.DocumentParent != null && (!Document.DocumentParent.IdBiblos.HasValue || Document.DocumentParent.IdBiblos.Value <= 0))
            {
                EntityProvider provider = new EntityProvider();
                lock (objLock)
                    Document.DocumentParent.IdBiblos = provider.GetNextArchiveBiblosId(Document);
            }

            if (!Document.IdBiblos.HasValue || Document.IdBiblos.Value <= 0)
            {
                EntityProvider provider = new EntityProvider();
                lock (objLock)
                    Document.IdBiblos = provider.GetNextArchiveBiblosId(Document);
            }

            //inserisce l'IdAwardBatch
            if (!Document.IdAwardBatch.HasValue)
            {
                EntityProvider provider = new EntityProvider();
                lock (objLock)
                {
                    var batch = provider.GetOpenAwardBatch(Document.Archive.IdArchive);
                    Document.IdAwardBatch = batch.IdAwardBatch;
                }
            }

            //Normalizzazione nome del file.
            var cfgCharsToReplace = ConfigurationManager.AppSettings["FileNameCharsToReplace"];
            if (!string.IsNullOrEmpty(cfgCharsToReplace) && !string.IsNullOrEmpty(Document.Name))
            {
                var toReplace = cfgCharsToReplace.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var originalName = Document.Name;
                foreach (var element in toReplace)
                {
                    Document.Name = Document.Name.Replace(element, "");
                }
                //Normalizzazione attributi testuali
                if (Document.AttributeValues != null)
                {
                    foreach (DocumentAttributeValue item in Document.AttributeValues)
                    {
                        if (item.Attribute.AttributeType == "System.String" && item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            foreach (var element in toReplace)
                            {
                                item.Value = item.Value.ToString().Replace(element, "");
                            }
                        }
                    }
                }
            }

            var result = DbProvider.AddDocument(Document, server, Document.Archive.PathTransito);
            return result;
        }

        public static void UpdateDocument(Document Document)
        {
            DbProvider.UpdateDocument(Document);
        }

        public static void ConfirmDocument(Guid idDocument)
        {
            Document document = DocumentService.GetDocument(idDocument);

            document.IsConfirmed = true;
            DateTime? mainDate = null;
            var attributesValues = AttributeService.GetFullDocumentAttributeValues(idDocument);
            document.PrimaryKeyValue = AttributeService.ParseAttributeValues(document.Archive, attributesValues, out mainDate);

            if (!string.IsNullOrEmpty(document.PrimaryKeyValue))
            {
                if (!DocumentService.CheckPrimaryKey(document.IdDocument, document.Archive.IdArchive, document.PrimaryKeyValue))
                    throw new BiblosDS.Library.Common.Exceptions.DocumentPrimaryKey_Exception();
            }

            DbProvider.ConfirmDocument(document, document.PrimaryKeyValue);

            if (document.DocumentParent == null)
            {
                var childrens = DocumentService.GetChainDocuments(document.IdDocument);
                foreach (var item in childrens)
                {
                    ConfirmDocument(item.IdDocument);
                }
            }
        }

        /// <summary>
        /// Save the objects in the same transaction
        /// </summary>
        /// <param name="items"></param>
        public static void SaveAtomic(List<BiblosDSObject> addedItems, List<BiblosDSObject> editItems)
        {
            EntityProvider provider = new EntityProvider();

            using (var ts = provider.BeginNoSave())
            {
                try
                {
                    if (addedItems != null)
                        foreach (var item in addedItems)
                        {
                            if (item is Document)
                            {
                                Document doc = (item as Document);

                                //inserisce l'IdAwardBatch
                                if (!doc.IdAwardBatch.HasValue)
                                {
                                    lock (objLock)
                                    {
                                        var batch = provider.GetOpenAwardBatch(doc.Archive.IdArchive);
                                        doc.IdAwardBatch = batch.IdAwardBatch;
                                    }
                                }

                                provider.AddDocument(doc, null, doc.Archive.PathTransito);
                            }
                            else if (item is DocumentAttach)
                            {
                                provider.AddDocumentAttach(item as DocumentAttach, (item as DocumentAttach).Document.Archive.PathTransito);
                            }
                            else
                                throw new NotSupportedException("Oggetto non supportato per il salvataggio in transazione: " + item.GetType().ToString());
                        }

                    if (editItems != null)
                        foreach (var item in editItems)
                        {
                            if (item is Document)
                            {
                                provider.UpdateDocument(item as Document);
                            }
                            else if (item is DocumentAttach)
                            {
                                provider.UpdateDocumentAttach(item as DocumentAttach);
                            }
                            else if (item is DocumentTransito)
                            {
                                DocumentTransito transito = item as DocumentTransito;
                                provider.TransitoChangeState(transito, transito.TarnsitoStatus);
                            }
                            else if (item is DocumentAttachTransito)
                            {
                                var transito = item as DocumentAttachTransito;
                                provider.TransitoAttachChangeState(transito.IdDocumentAttach, transito.TarnsitoStatus);
                            }
                            else
                                throw new NotSupportedException("Oggetto non supportato per il salvataggio in transazione: " + item.GetType().ToString());
                        }

                    provider.SaveChanges();
                    ts.Commit();
                }
                catch (Exception)
                {
                    ts.Rollback();
                    throw;
                }
            }

            //try
            //{
            //    provider.BeginTransaction();
            //    if (addedItems != null)
            //        foreach (var item in addedItems)
            //        {
            //            if (item is Document)
            //            {
            //                provider.AddDocument(item as Document, (item as Document).Archive.PathTransito);
            //            }
            //            else
            //                throw new NotSupportedException("Oggetto non supportato per il salvataggio in transazione: " + item.GetType().ToString());
            //        }
            //    if (editItems != null)
            //        foreach (var item in editItems)
            //        {
            //            if (item is Document)
            //            {
            //                provider.UpdateDocument(item as Document);
            //            }else if (item is DocumentTransito)
            //            {
            //                DocumentTransito transito = item as DocumentTransito;
            //                provider.TransitoChangeState(transito.IdDocument, transito.TarnsitoStatus);
            //            }
            //            else
            //                throw new NotSupportedException("Oggetto non supportato per il salvataggio in transazione: " + item.GetType().ToString());
            //        }
            //    provider.CommitTransaction();
            //}
            //catch (Exception)
            //{
            //    provider.RollbackTransaction();
            //    throw;
            //}

        }

        public static void UpdateDocumentStorageVersion(Guid IdDocument, decimal Version)
        {
            DbProvider.UpdateDocumentStorageVersion(IdDocument, Version);
        }

        public static string GetTransitoLocalPath(DocumentTransito transito)
        {
            return DbProvider.GetTransitoLocalPath(transito);
        }

        public static string GetAttachTransitLocalPath(Guid IdDocumentAttach)
        {
            return DbProvider.GetAttachTransitLocalPath(IdDocumentAttach);
        }

        public static DocumentTransito GetTransito(Guid idDocument)
        {
            var transit = DbProvider.GetTransito(idDocument);
            if (transit == null)
                throw new Exceptions.DocumentNotFound_Exception("Document not found in transit: " + idDocument);
            return transit;
        }

        /// <summary>
        /// Lock escusivo sul file da processare
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        public static DocumentTransito CheckOutTransitoDocument(Guid idDocument)
        {
            DocumentTransito transito = DbProvider.GetTransito(idDocument);
            if (transito == null)
                throw new Exceptions.DocumentNotFound_Exception("Document not found in transit: " + idDocument);
            return DbProvider.TransitoChangeState(transito, DocumentTarnsitoStatus.StorageProcessing);
        }

        /// <summary>
        /// Rimette il file nello stato da riprocessare
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        public static DocumentTransito UndoCheckOutTransitoDocument(DocumentTransito transito)
        {
            return DbProvider.TransitoChangeState(transito, DocumentTarnsitoStatus.FaultProcessing);
        }

        /// <summary>
        /// UnLock sul file
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        public static DocumentTransito CheckInTransitoDocument(DocumentTransito transito)
        {
            return DbProvider.TransitoChangeState(transito, DocumentTarnsitoStatus.EndProcessing);
        }


        public static bool CheckPrimaryKey(Guid IdDocument, Guid IdArchive, string Value)
        {
            int? result = DbProvider.CheckPrimaryKey(IdArchive, IdDocument, Value);
            return result == null || result <= 0;
        }

        public static BindingList<DocumentPermission> GetDocumentPermissions(Guid IdDocument)
        {
            return DbProvider.GetDocumentPermissions(IdDocument);
        }

        #region Storage

        /// <summary>
        /// 
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="attributeValues"></param>
        /// <returns></returns>
        /// <example>
        /// Find the STORAGE on the role
        /// if no Rule exist Return the firts 
        /// STORAGE retrived
        /// If no STORAGE are configured for the ARCHIVE
        /// throw Exception
        /// </example>
        /// <exception cref="BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception">StorageNotFound_Exception</exception>
        public static DocumentStorage GetStorage(DocumentArchive Archive, Server server, BindingList<DocumentAttributeValue> AttributeValues)
        {
            BindingList<DocumentStorage> storages = null;
            if (server == null)
                storages = StorageService.GetStorageActiveFromArchive(Archive);
            else
                storages = StorageService.GetStorageActiveFromArchiveServer(Archive, server);
            if (storages.Count() <= 0)
                throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception();
            else
            {
                foreach (DocumentStorage item in storages)
                {
                    try
                    {
                        //Search the first role for the attribute
                        if (VerifyRole(StorageService.GetStorageRuleFromStorageArchive(item.IdStorage, Archive.IdArchive), AttributeValues))
                            return item;
                    }
                    catch (Exceptions.StorageConfiguration_Exception ex)
                    {
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "DocumentService.GetStorage",
                            ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error);

                        continue;
                    }
                }
                //Se non sono stati trovate role
                return storages[0];
            }
            ////Se non viene trovata nessuna Rule che verifica le condizioni
            ////si imposta ritorna il primo Storage per ordine           
            //throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception();
        }

        /// <summary>
        /// Find the StorageArea which verify the
        /// conditions of the AttributeValue
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="AttributeValue"></param>
        /// <returns>DocumentStorageArea</returns>
        public static DocumentStorageArea GetStorageArea(Document Document, BindingList<DocumentAttributeValue> attributeValues)
        {
            BindingList<DocumentStorageArea> storageAreas = StorageService.GetStorageAreaActiveFromStorage(Document.Storage.IdStorage);

            foreach (DocumentStorageArea item in storageAreas)
            {
                var defaultRule = StorageService.GetStorageRulesFromStorageAreaArchive(item.IdStorageArea, Document.Archive.IdArchive);
                if (defaultRule.Count() > 0)
                {
                    if (DocumentService.VerifyStorageAreaRole(defaultRule, attributeValues))
                        return item;
                }
            }
            foreach (DocumentStorageArea item in storageAreas)
            {
                foreach (var store in StorageService.GetStorageRulesFromStorageAreaArchive(item.IdStorageArea, Document.Archive.IdArchive).Where(x => x.IsCalculated.GetValueOrDefault()))
                {
                    var value = (from m in attributeValues
                                 where m.Attribute.Name == store.Attribute.Name
                                 select m).Single();
                    if (string.IsNullOrEmpty(value.Value.ToStringExt()))
                        continue;
                    var pathValue = string.IsNullOrEmpty(store.RuleFormat) ? value.Value.ToStringExt() : string.Format(store.RuleFormat, value.Value.ToStringExt());
                    if (storageAreas.Any(x => x.Name == pathValue))
                        return item;
                    item.IdStorageArea = Guid.NewGuid();
                    item.Name = pathValue;
                    item.Path = pathValue;
                    item.CurrentFileNumber = 0;
                    item.CurrentSize = 0;
                    item.Storage = Document.Storage;
                    StorageService.AddStorageArea(item);
                    StorageService.AddStorageAreaRule(new DocumentStorageAreaRule { StorageArea = item, Attribute = value.Attribute, RuleFilter = pathValue, RuleOrder = 0 });
                    return item;
                }
            }
            if (storageAreas.Count() > 0)
                return storageAreas.Any(x => x.Archive != null && x.Archive.IdArchive == Document.Archive.IdArchive) ? storageAreas.Where(x => x.Archive != null && x.Archive.IdArchive == Document.Archive.IdArchive).First() : storageAreas[0];

            throw new BiblosDS.Library.Common.Exceptions.StorageAreaConfiguration_Exception(); ;
        }

        public static bool VerifyStorageAreaRole(IEnumerable<DocumentStorageAreaRule> roles, BindingList<DocumentAttributeValue> attributeValues)
        {
            return VerifyRole(new BindingList<DocumentStorageRule>(roles.Select(x => new DocumentStorageRule { Attribute = x.Attribute, RuleFilter = x.RuleFilter, RuleFormat = x.RuleFormat, RuleOperator = x.RuleOperator, RuleOrder = x.RuleOrder }).ToList()), attributeValues);
        }
        /// <summary>
        /// Find the first role 
        /// with the predicate
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static bool VerifyRole(BindingList<DocumentStorageRule> roles, BindingList<DocumentAttributeValue> attributeValues)
        {
            //TODO Log incongruence
            //Se lo storage non ha role storage di default.
            //Utilizzato per archivi senza regole, esempio tutti i documenti in uno storage
            if (roles.Count <= 0)
                return true;
            foreach (DocumentStorageRule item in roles)
            {
                var value = (from m in attributeValues
                             where m.Attribute.Name == item.Attribute.Name
                             select m).Single();

                if (!GetKeyFilter(value.Value, value.Attribute.AttributeType, item.RuleFilter, item.RuleFormat, (item.RuleOperator == null ? RuleOperator.IsEqual : (RuleOperator)item.RuleOperator.IdRuleOperator)))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <param name="filter"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal static bool GetKeyFilter(object AttributeValue, string AttributeType, object Filter, string Format, RuleOperator opr)
        {
            try
            {
                var value = AttributeValue;
                if (!string.IsNullOrEmpty(Format))
                    value = string.Format(Format, value);
                switch (opr)
                {
                    case RuleOperator.IsGreatherThan:
                        switch (AttributeType)
                        {
                            case "System.Int64":
                                return value.TryConvert<Int64>() > Filter.TryConvert<Int64>();
                            case "System.Double":
                                return value.TryConvert<double>() > Filter.TryConvert<double>();
                            case "System.DateTime":
                                return value.TryConvert<DateTime>() > Filter.TryConvert<DateTime>();
                            default:
                                throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato for IsGreatherThan.", AttributeType));
                        }
                    case RuleOperator.IsGreatherOrEqualThan:
                        switch (AttributeType)
                        {
                            case "System.Int64":
                                return value.TryConvert<Int64>() >= Filter.TryConvert<Int64>();
                            case "System.Double":
                                return value.TryConvert<double>() >= Filter.TryConvert<double>();
                            case "System.DateTime":
                                return value.TryConvert<DateTime>() >= Filter.TryConvert<DateTime>();
                            default:
                                throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato for IsGreatherOrEqualThan.", AttributeType));
                        }
                    case RuleOperator.IsLessThan:
                        switch (AttributeType)
                        {
                            case "System.Int64":
                                return value.TryConvert<Int64>() < Filter.TryConvert<Int64>();
                            case "System.Double":
                                return value.TryConvert<double>() < Filter.TryConvert<double>();
                            case "System.DateTime":
                                return value.TryConvert<DateTime>() < Filter.TryConvert<DateTime>();
                            default:
                                throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato for IsLessThan.", AttributeType));
                        }
                    case RuleOperator.IsLessOrEqualThan:
                        switch (AttributeType)
                        {
                            case "System.Int64":
                                return (Int64)value.TryConvert<Int64>() <= (Int64)Filter.TryConvert<Int64>();
                            case "System.Double":
                                return value.TryConvert<double>() <= Filter.TryConvert<double>();
                            case "System.DateTime":
                                return value.TryConvert<DateTime>() <= Filter.TryConvert<DateTime>();
                            default:
                                throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato for IsLessOrEqualThan.", AttributeType));
                        }
                    case RuleOperator.IsEqual:
                    default:
                        return value.ToStringExt().Equals(Filter.ToStringExt());
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "DocumentService.GetKeyFilter",
                    ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);

                throw ex;
            }
        }

        #endregion

        #region State Manage

        public static void CheckIn(Guid IdDocument, string UserId)
        {
            DbProvider.CheckIn(IdDocument, UserId);
        }

        public static void CheckOut(Guid IdDocument, string UserId)
        {
            DbProvider.CheckOut(IdDocument, UserId);
        }

        public static void UndoCheckOut(Guid IdDocument, string UserId)
        {
            DbProvider.UndoCheckOut(IdDocument, UserId);
        }

        #endregion

        public static decimal IncrementVersion(decimal Version)
        {
            return Version + (decimal)0.01;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// First certificate Mark as Default
        /// </returns>
        public static DocumentCertificate GetCertificateDefault()
        {
            return DbProvider.GetCertificateDefault();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdCertificate"></param>
        /// <returns></returns>
        public static DocumentCertificate GetCertificate(Guid IdCertificate)
        {
            return DbProvider.GetCertificate(IdCertificate);
        }

        /// <summary>
        /// Calculate the document Hash
        /// </summary>
        /// <remarks>
        /// Use the Object for next implementation ex:using doc feature into the hash
        /// </remarks>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string GetDocumentHash(Document Document)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return UtilityService.GetStringFromBob(sha.ComputeHash(Document.Content.Blob));
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdDocumento"></param>
        /// <param name="IdParent"></param>
        /// <param name="Content"></param>
        /// <param name="Ext"></param>
        /// <param name="date"></param>
        /// <param name="Attibutes"></param>
        /// <param name="NomeCert"></param>
        /// <returns></returns>
        public static Byte[] Sign(Guid IdDocumento, Guid IdParent, DocumentContent Content,
            string Ext, string date, BindingList<DocumentAttributeValue> Attibutes, DocumentCertificate certificate)
        {
            int MAX_ATTR = 100;
            List<string> Nomi_Valori = new List<string>();
            foreach (DocumentAttributeValue item in Attibutes)
            {
                Nomi_Valori.Add(item.Attribute.Name + "=" + item.Value);
            }

            StringBuilder hexValue = new StringBuilder();
            for (int i = 0; i < Content.Blob.Length; i++)
                hexValue.AppendFormat("{0:X2}", Content.Blob[i]);

            StringBuilder strSign = new StringBuilder();
            strSign.Append(IdDocumento.ToString());
            strSign.Append("|");
            strSign.Append(IdParent.ToString());
            strSign.Append("|");
            strSign.Append(hexValue.ToString());
            strSign.Append("|");
            strSign.Append(Ext);
            strSign.Append("|");
            strSign.Append(date);
            strSign.Append("|");

            //List Of Nome/Valore
            strSign.Append(string.Join("|", Nomi_Valori.ToArray(), 0, Math.Min(MAX_ATTR, Attibutes.Count())));

            try
            {
                X509Certificate2 cert = null;
                if (!certificate.IsOnDisk)
                {
                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                    store.Open(OpenFlags.ReadOnly);

                    X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, certificate.Name, false);

                    if (certs.Count > 0)
                    {
                        cert = certs[0];
                    }
                    else
                        throw new Exception(" Certificate not Found.");

                    store.Close();
                }
                else
                    cert = new X509Certificate2(certificate.Path, certificate.Password);
                //CspParameters param = new CspParameters();
                //param.KeyContainerName = "Decryption.RSACryptoServiceProvider." + Guid.NewGuid().ToString();
                //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param);
                //RSA rsa = (cert.PrivateKey as RSA);                
                RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;
                RSAPKCS1SignatureFormatter rf = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa);
                rf.SetHashAlgorithm("SHA1");
                #region Old Code
                //DPAPI.KeyType.UserKey anzichè DPAPI.KeyType.MachineKey
                //X509CertificateStore store = X509CertificateStore.CurrentUserStore(X509CertificateStore.MyStore);
                //store.OpenRead();

                //X509Certificate sender;
                //X509CertificateCollection certColl = store.FindCertificateBySubjectString(NomeCert);
                //sender = certColl[0];

                //RSAPKCS1SignatureFormatter rf = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(sender.Key);
                //rf.SetHashAlgorithm("SHA1");
                #endregion
                System.Text.ASCIIEncoding oEncoder = new System.Text.ASCIIEncoding();
                byte[] data = oEncoder.GetBytes(strSign.ToString());
                SHA1 sha = new SHA1CryptoServiceProvider();
                Byte[] hash = sha.ComputeHash(data);

                byte[] signature = rf.CreateSignature(hash);

                return signature;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception("The data was not signed or verified " + e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="SignOld"></param>
        /// <param name="SignNew"></param>
        /// <param name="Recalc"></param>
        /// <returns></returns>
        public static DocumentCheckState CheckSignedDocument(Document Document, Guid IdParentDocument, out Byte[] SignOld, out Byte[] SignNew, bool Recalc)
        {

            SignOld = new byte[0];
            SignNew = new byte[0];
            //Verify if attribute is loaded
            if (Document.AttributeValues == null)
                throw new Exceptions.Attribute_Exception("Nessun attributo definito");

            string hashDocStr = DocumentService.GetDocumentHash(Document);
            Byte[] hashDoc = UtilityService.GetBlobFromString(hashDocStr);
            Byte[] signDB = null;

            if (!Recalc)
            {
                // check file modified
                if (Document.DocumentHash.Length != hashDocStr.Length)
                    return DocumentCheckState.FileModified;
                for (int i = 0; i < Document.DocumentHash.Length; i++)
                    if (Document.DocumentHash[i] != hashDocStr[i])
                        return DocumentCheckState.FileModified;
            }
            //Verify Content
            if (Document.Content == null)
                throw new Exception("Content non definito");
            //
            DocumentCertificate certificate = DocumentService.GetCertificate(Document.Certificate.IdCertificate);
            if (certificate == null)
                throw new Exceptions.CertificateNotFound_Exception();
            Byte[] signNow = Sign(Document.IdDocument,
                Document.DocumentParent.IdDocument,
                Document.Content,
                Path.GetExtension(Document.Name),
                string.Format("{0:yyyyMMdd}", Document.DateCreated),
                AttributeService.GetAttributeValues(Document.IdDocument), certificate);
            //
            signDB = UtilityService.GetBlobFromString(Document.FullSign);
            if (SignOld != null)
                SignOld = signDB;
            if (SignNew != null)
                SignNew = signNow;

            if (signNow.Length != signDB.Length)  // check MetaData sign
                return DocumentCheckState.MetaDataModified;
            for (int i = 0; i < signNow.Length; i++)
                if (signNow[i] != signDB[i])
                    return DocumentCheckState.FileModified;
            return DocumentCheckState.Ok;
        }


        /// <summary>
        /// Inserisce il padre di una Chain
        /// </summary>
        /// <param name="Document">Document da inserire</param>
        /// <returns>Document</returns>
        public static Document InsertDocumentChain(Document Document)
        {
            try
            {
                if (Document.Archive == null || Document.Archive.IdArchive == Guid.Empty)
                    throw new Exception("Nessun archivio selezionato. Impossibile continuare");
                //
                //if (Document.AttributeValues == null || Document.AttributeValues.Count == 0)
                //    throw new Exception("Nessun attributo passa. Impossibile continuare");
                //TODO Modificare se non vengono passati gli attibuti
                //Try to find Storage on selected Attributes
                //Document.Storage = DocumentService.GetStorage(Document.Archive, Document.AttributeValues);
                //if (Document.Storage == null)
                //    throw new Exception("Storage not found");
                //Set the new ID
                Document.IdDocument = Guid.NewGuid();
                Document.IsVisible = true;
                Document.Status = new Status(DocumentStatus.Undefined);
                //Save
                return DocumentService.AddDocument(Document, null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        private static BindingList<DocumentAttribute> GetAttributesByDocument(Guid idArchive, AttributeGroupType groupType)
        {
            var attributes = AttributeService.GetAttributesFromArchive(idArchive);
            // Se non esistono attributi di gruppo "catena" mantengo invariata la procedura.
            if (!attributes.Any(a => a.AttributeGroup.GroupType == AttributeGroupType.Chain))
                return attributes;

            // Se il documento è "padre-catena" considero solo attributi di tipo "catena".
            if (groupType == AttributeGroupType.Chain)
                return new BindingList<DocumentAttribute>(attributes.Where(a => a.AttributeGroup.GroupType == AttributeGroupType.Chain).ToList());

            // Altrimenti considero gli altri gruppi di attributi.
            return new BindingList<DocumentAttribute>(attributes.Where(a => a.AttributeGroup.GroupType != AttributeGroupType.Chain).ToList());
        }


        /// <summary>
        /// Collega Document a Chain se specificata, altrimenti ne crea una
        /// </summary>
        /// <param name="Document">Document da inserire</param>
        /// <param name="IdParent">Chain di appartenenza</param>
        /// <returns>Document</returns>
        public static Document AddDocumentToChain(Document Document, Guid? IdParent)
        {
            try
            {
                Journaling.WriteJournaling(LoggingSource.BiblosDS_CommonLibrary,
                    WindowsIdentity.GetCurrent().Name,
                    "AddDocumentToChain",
                    string.Empty,
                     LoggingOperationType.BiblosDS_InsertDocument,
                      LoggingLevel.BiblosDS_Trace,
                      Document.Archive != null ? Document.Archive.IdArchive : Guid.Empty,
                      null,
                      IdParent);
                //
                if (Document.Storage == null && Document.StorageArea == null && ((Document.Content == null || Document.Content.Blob == null || Document.Content.Blob.Length <= 0) && IdParent.HasValue))
                    throw new Exception("Nessun file passato in DocumentContent. Impossibile continuare");
                //
                if (string.IsNullOrEmpty(Document.Name) && ConfigurationManager.AppSettings["VerifyDocumentName"].ToStringExt() == "true")
                    throw new Exception("Nessun nome file impostato. Impossibile continuare");
                if (Document.Archive == null || Document.Archive.IdArchive == Guid.Empty)
                    throw new Exception("Nessun archivio selezionato. Impossibile continuare");
                Document.Archive = ArchiveService.GetArchive(Document.Archive.IdArchive);
                var currentServer = ServerService.GetCurrentServer();
                if (currentServer != null)
                {
                    if (currentServer.ServerRole == ServerRole.Proxy)
                    {
                        var masterServer = ServerService.GetMasterServer();
                        if (masterServer == null)
                            throw new Exceptions.Generic_Exception("Nessun server Master definito.");
                        logger.DebugFormat("Server {0} is a proxy, Call Master: {1}", currentServer.ServerName, masterServer.ServerName);
                        using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName, masterServer.ServerName))
                        {
                            return (clientChannel as IDocuments).AddDocumentToChain(Document, IdParent, DocumentContentFormat.Binary);
                        }
                    }
                    var archiveConfig = ServerService.GetArchiveServerConfig(currentServer.IdServer, Document.Archive.IdArchive);
                    if (archiveConfig != null)
                    {
                        Document.Archive.TransitoEnabled = archiveConfig.TransitEnabled;
                        Document.Archive.PathTransito = archiveConfig.TransitPath;
                    }
                }
                if (Document.AttributeValues == null)
                    Document.AttributeValues = new BindingList<DocumentAttributeValue>();

                var attributes = GetAttributesByDocument(Document.Archive.IdArchive, AttributeGroupType.Primary);

                //Check dei metadati
                //Viene assunto che l'Archive non possa essere modificato in modifica del documento
                //questo vincolo mi garantisce che lo stesso documento sia memorizzato nello stesso storage per tutte le versioni
                BindingList<Exception> exceptions = AttributeService.CheckMetaData(Document, Document.Archive, Document.AttributeValues, attributes, false);
                if (exceptions.Count > 0)
                {
                    StringBuilder fault = new StringBuilder();
                    foreach (var item in exceptions)
                    {
                        fault.Append(item.Message + Environment.NewLine);
                    }
                    throw new Exceptions.Attribute_Exception(fault.ToString());
                }
                foreach (var item in Document.AttributeValues)
                {
                    logger.DebugFormat("N '{0}' -> V '{1}'", item.Attribute.Name, item.Value);
                    string errMessage = string.Format("Il documento contiene dei metadati non definiti.");
                    if (item.Attribute == null)
                    {
                        item.Attribute = attributes.Where(x => x.IdAttribute == item.IdAttribute).FirstOrDefault();
                    }
                    else
                    {
                        errMessage = string.Format("{2} ({0} - {1})", item.Attribute.Name, item.Attribute.IdAttribute, errMessage);
                        if (item.Attribute.IdAttribute == Guid.Empty)
                            item.Attribute = attributes.Where(x => x.Name == item.Attribute.Name).FirstOrDefault();
                        else
                            item.Attribute = attributes.Where(x => x.IdAttribute == item.Attribute.IdAttribute).FirstOrDefault();
                    }
                    if (item.Attribute == null)
                        throw new Exceptions.Attribute_Exception(errMessage);
                }
                foreach (var item in attributes.Where(x => !string.IsNullOrEmpty(x.DefaultValue)))
                {
                    if (!Document.AttributeValues.Any(x => x.Attribute.IdAttribute == item.IdAttribute))
                        Document.AttributeValues.Add(new DocumentAttributeValue { Attribute = item, IdAttribute = item.IdAttribute, Value = item.DefaultValue });
                }
                DateTime? mainDate = null;
                BindingList<DocumentAttributeValue> documentAttributeValues = Document.AttributeValues;
                if (IdParent.HasValue && IdParent != Guid.Empty)
                {
                    BindingList<DocumentAttributeValue> parentAttributeValues = AttributeService.GetAttributeValues(IdParent.Value);
                    documentAttributeValues = new BindingList<DocumentAttributeValue>(documentAttributeValues.Union(parentAttributeValues.Where(x => !documentAttributeValues.Any(xx => xx.IdAttribute == x.IdAttribute))).ToList());
                }
                var primaryKey = AttributeService.ParseAttributeValues(Document.Archive, documentAttributeValues, out mainDate);
                logger.DebugFormat("MAIN DATE '{0}' -> ", mainDate);
                if (ConfigurationManager.AppSettings["VerifyDocumentMainDate"].ToStringExt() == "true" && Document.Archive.IsLegal)
                {
                    if (!string.IsNullOrEmpty(primaryKey))
                    {
                        if (!DocumentService.CheckPrimaryKey(Document.IdDocument, Document.Archive.IdArchive, primaryKey))
                            throw new BiblosDS.Library.Common.Exceptions.DocumentPrimaryKey_Exception();
                    }
                    //Verifica che la data del documento sia maggiore della data dell'ultimo documento 
                    var lastPresrvedDocumentMainDate = GetLastPreservationDate(Document.Archive.IdArchive);
                    if (mainDate.HasValue && DateTime.Compare(mainDate.Value, lastPresrvedDocumentMainDate) < 0)
                        throw new BiblosDS.Library.Common.Exceptions.DocumentDateNotValid_Exception();
                }
                else
                {
                    Document.PrimaryKeyValue = primaryKey;
                    if (!string.IsNullOrEmpty(Document.PrimaryKeyValue))
                    {
                        if (!DocumentService.CheckPrimaryKey(Document.IdDocument, Document.Archive.IdArchive, Document.PrimaryKeyValue))
                            throw new BiblosDS.Library.Common.Exceptions.DocumentPrimaryKey_Exception();
                    }
                }
                //If the parent exist select the parent chain...if not exist create the chain
                Document ParentDocument = null;
                if (!IdParent.HasValue || IdParent.Value == Guid.Empty)
                {
                    ParentDocument = new Document();
                    ParentDocument.IdDocument = Guid.NewGuid();
                    ParentDocument.IsVisible = true;
                    ParentDocument.Status = new Status(DocumentStatus.Undefined);
                    ParentDocument.Archive = Document.Archive;
                    //Save
                    //ParentDocument = DocumentService.AddDocument(ParentDocument);
                }
                else
                {
                    ParentDocument = DocumentService.GetDocument((Guid)IdParent);
                    if (ParentDocument == null)
                        throw new Exception("Parent non valido");
                }
                Document.DocumentParent = ParentDocument;
                //Se non vienen specificato nessun permesso viene aggiunto il diritto 
                //full control al gruppo che ha caricato il documento
                //if (Document.Permissions == null || Document.Permissions.Count <= 0)
                //{
                //    Document.Permissions = new BindingList<DocumentPermission>();
                //    foreach (var item in WindowsIdentity.GetCurrent().Groups)
                //    {
                //        if (!(Document.Permissions.Where( x=> x.Name ==item.Translate(typeof(NTAccount)).Value).FirstOrDefault() == null))
                //            Document.Permissions.Add(new DocumentPermission()
                //            {
                //                Name = item.Translate(typeof(NTAccount)).Value,
                //                Mode = DocumentPermissionMode.FullControl,
                //                IsGroup = true
                //            });
                //    }
                //}
                //
                logger.DebugFormat("MAIN DATE SET '{0}' -> ", mainDate);
                Document.DateMain = mainDate;
                //AttributeService.GetAttribute
                //Set the new Id of the new document.
                Document.IdDocument = Guid.NewGuid();
                //Force the visibility of the document.
                Document.IsVisible = true;
                //Set the version of the document.
                Document.Version = 1;
                //
                Document.DateCreated = DateTime.Now;
                //Save the document to Transito Path
                if (Document.Archive.TransitoEnabled)
                {
                    if ((Document.Content != null && Document.Content.Blob != null && Document.Content.Blob.Length > 0))
                    {
                        FileService.SaveFileToTransitoLocalPath(Document, Document.Content.Blob);
                        //TODO if the insert is aborted delete the Transito Path file. 
                        Document.Size = Document.Content.Blob.Length;
                        //Set the status
                        Document.Status = new Status((short)DocumentStatus.InTransito);
                    }
                    else
                    {
                        if (Document.Storage != null && Document.StorageArea != null)
                        {
                            if (Document.Status == null)
                                Document.Status = new Status((short)DocumentStatus.InStorage);
                        }
                        else
                        {
                            if (ConfigurationManager.AppSettings["AllowZeroByteDocument"].ToStringExt() != "true")
                                throw new Exception("Impossibile inserire un documento di zero byte.");
                            else
                                Document.Status = new Status((short)DocumentStatus.ProfileOnly);
                        }
                    }
                }
                else
                {
                    if ((Document.Content != null && Document.Content.Blob != null && Document.Content.Blob.Length > 0))
                        Document.Status = new Status((short)DocumentStatus.Undefined);
                    else if (Document.Storage != null && Document.StorageArea != null)
                    {
                        if (Document.Status == null)
                            Document.Status = new Status((short)DocumentStatus.InStorage);
                    }
                    else
                    {
                        if (ConfigurationManager.AppSettings["AllowZeroByteDocument"].ToStringExt() != "true")
                            throw new Exception("Impossibile inserire un documento di zero byte.");
                        else
                            Document.Status = new Status((short)DocumentStatus.ProfileOnly);
                    }
                }
                //Add the docuement and the attibutes value into the DB
                AddDocument(Document, currentServer);

                return Document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_CommonLibrary,
                                        "AddDocumentToChain",
                                        ex.ToString(),
                                        LoggingOperationType.BiblosDS_InsertDocument,
                                        LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        private static DateTime GetLastPreservationDate(Guid idArchive)
        {
            try
            {
                var dtResult = DbProvider.GetLastPreservedDate(idArchive);
                return dtResult.GetValueOrDefault(DateTime.MinValue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Detach of a document
        /// </summary>
        /// <param name="document"></param>
        public static void DetachDocument(Document document)
        {
            try
            {
                if (document.IsConservated.GetValueOrDefault())
                    throw new Exception("Impossibile settare il detach ad un documento che è conservato.");
                DbProvider.DetachDocument(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Detach of a document
        /// </summary>
        /// <param name="document"></param>
        public static void DetachDocuments(IEnumerable<Document> documents)
        {
            try
            {
                if (documents.Any(x => x.IsConservated == true))
                {
                    throw new Exception("Impossibile settare il detach ad un documento che è conservato.");
                }

                EntityProvider provider = new EntityProvider();
                using (DbTransaction ts = provider.BeginNoSave())
                {
                    try
                    {
                        foreach (Document document in documents)
                        {
                            DbProvider.DetachDocument(document);
                        }
                        provider.SaveChanges();
                        ts.Commit();
                    }
                    catch (Exception)
                    {
                        ts.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        #region Old Code

        /*
            public static int AddDocumentStatic(string Server, string DataBase, int Chain, stDoc Doc)
            {
                //Server = CheckChiamata(Server, DataBase, Chain.ToString(), Doc.XmlValues);

                string NomeCert = "";
                int idChain = 0;
                SqlConnection sqlCon = null;
                bool GestAttributi = false, GestDiretta = false;

                //Thread.CurrentThread.ApartmentState = ApartmentState.STA;
                //if (Doc.XmlValues != null && Doc.XmlValues != "")
                //{
                //    // controlla DB per usare in caso BDSDCOM
                //    try
                //    {
                //        string ConStr = BiblosDSConnectionString;// ConfigurationSettings.AppSettings["SQLConnection." + Server];
                //        if (ConStr != null)
                //        {
                //            sqlCon = new SqlConnection(ConStr);
                //            sqlCon.Open();

                //            SqlCommand sqlCmd = new SqlCommand("SELECT Valore FROM Parametro WHERE Nome='" + ACTIVE_CERT + "'", sqlCon);
                //            SqlDataReader sqlDataR = sqlCmd.ExecuteReader();
                //            if (sqlDataR.Read())
                //            {
                //                GestAttributi = true;
                //                GestDiretta = true;
                //                NomeCert = sqlDataR["Valore"].ToString();
                //            }
                //            sqlDataR.Close();
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        if (sqlCon != null)
                //            if (sqlCon.State == ConnectionState.Open)
                //                sqlCon.Close();
                //    }
                //}
                //else
                //    GestDiretta = true;

                //if (GestDiretta == false)
                //{
                //    // DCOM connection (with attribute)
                //    //HttpContext.Current.Trace.Write("AD 1");
                //    BDSCOMLib.Connection BDSCon = null;
                //    Type ServerType;
                //    Object ServerObject = null;

                //    try
                //    {
                //        //BDSCon = new BDSCOMLib.Connection();
                //        ServerType = Type.GetTypeFromProgID("BiblosDS.Connection", Server, false);
                //        ServerObject = Activator.CreateInstance(ServerType);
                //        BDSCon = (BDSCOMLib.Connection)Marshal.CreateWrapperOfType(ServerObject, ServerType);
                //    }
                //    catch (System.Exception exp)
                //    {
                //        HttpContext.Current.Response.AppendToLog("AddDocument, errore instanziazione BDSDCOM: " + Doc.XmlValues);
                //        HttpContext.Current.Response.AppendToLog("AddDocument, errore instanziazione BDSDCOM: " + exp.Message + " " + exp.StackTrace);
                //        throw new Exception("AddDocument, errore instanziazione BDSDCOM: " + exp.Message + " " + exp.StackTrace);
                //    }

                //    BDSCOMLib.DocumentCollections BDSCols = null;
                //    BDSCOMLib.DocumentCollection BDSCol = null;
                //    BDSCOMLib.Documents BDSDocs = null;
                //    BDSCOMLib.Document BDSDoc = null;
                //    try
                //    {
                //        BDSCon.OpenConnection("BiblosDS WebSvc", DataBase);

                //        if (Chain == 0)
                //            BDSCon.NewDocumentCollection();
                //        else
                //            BDSCon.OpenDocumentCollection(Chain);

                //        BDSCols = (BDSCOMLib.DocumentCollections)BDSCon.DocumentCollections;
                //        BDSCol = (BDSCOMLib.DocumentCollection)BDSCols.get_Item(1);
                //        BDSDocs = (BDSCOMLib.Documents)BDSCol.Documents;
                //        BDSCol.NewDocument();
                //        BDSDoc = (BDSCOMLib.Document)BDSDocs.get_Item(BDSDocs.Count);

                //        BDSDoc.Type = Doc.FileExtension;

                //        // attributes
                //        XmlTextReader reader;
                //        XmlDocument document = new XmlDocument();
                //        NameTable nt = new NameTable();
                //        XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                //        XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                //        //nsmgr.AddNamespace("bk", "urn:BiblosDS");

                //        try
                //        {
                //            reader = new XmlTextReader(Doc.XmlValues, XmlNodeType.Element, context);
                //            document.Load(reader);

                //            XmlNode root = document.SelectSingleNode("Object");

                //            // read attributes
                //            XmlNodeList atts = root.SelectNodes("Attribute");
                //            foreach (XmlNode att in atts)
                //            {
                //                //InputFilesMask=InpDir.Attribute("Mask",null);
                //                BDSDoc.set_Attribute(att.Attributes["Name"].InnerText.ToString(),
                //                    att.InnerText.ToString());
                //            }
                //            int Patch = 1;
                //            string pt = ConfigurationSettings.AppSettings["BdsDcomAdd1ToBlob." + DataBase];
                //            if (pt != null)
                //                Patch = string.Compare(pt, "true", true) == 0 ? 1 : 0;
                //            pt = ConfigurationSettings.AppSettings["BdsDcomAdd1ToBlob"];
                //            if (pt != null)
                //                Patch = string.Compare(pt, "true", true) == 0 ? 1 : 0;

                //            Byte[] buf = System.Convert.FromBase64String(Doc.Blob);
                //            Byte[] BdsDcomBug = new Byte[buf.Length + Patch];
                //            buf.CopyTo(BdsDcomBug, 0);
                //            BDSDoc.Content = BdsDcomBug;
                //            BDSDoc.Save();
                //        }
                //        catch (Exception e)
                //        {
                //            if (BDSDoc != null) Marshal.ReleaseComObject(BDSDoc);
                //            if (BDSDocs != null) Marshal.ReleaseComObject(BDSDocs);
                //            if (BDSCols != null) Marshal.ReleaseComObject(BDSCols);
                //            if (BDSCol != null) Marshal.ReleaseComObject(BDSCol);
                //            if (BDSCon != null) Marshal.ReleaseComObject(BDSCon);
                //            if (ServerObject != null) Marshal.ReleaseComObject(ServerObject);
                //            ServerObject = null;
                //            HttpContext.Current.Response.AppendToLog("AddDocument, errore instanziazione BDSDCOM: " + Doc.XmlValues);
                //            HttpContext.Current.Response.AppendToLog("AddDocument, errore nel salvataggio: " + e.Message);
                //            if (sqlCon != null)
                //                if (sqlCon.State == ConnectionState.Open)
                //                    sqlCon.Close();
                //            throw new Exception("AddDocument, errore nel salvataggio: " + e.Message);
                //        }
                //        idChain = BDSCol.Id;

                //        Marshal.ReleaseComObject(BDSDoc);
                //        Marshal.ReleaseComObject(BDSDocs);
                //        Marshal.ReleaseComObject(BDSCols);
                //        Marshal.ReleaseComObject(BDSCol);
                //        Marshal.ReleaseComObject(BDSCon);
                //        Marshal.ReleaseComObject(ServerObject);
                //        ServerObject = null;
                //    }
                //    catch (System.Exception exp)
                //    {
                //        if (BDSDoc != null) Marshal.ReleaseComObject(BDSDoc);
                //        if (BDSDocs != null) Marshal.ReleaseComObject(BDSDocs);
                //        if (BDSCols != null) Marshal.ReleaseComObject(BDSCols);
                //        if (BDSCol != null) Marshal.ReleaseComObject(BDSCol);
                //        if (BDSCon != null) Marshal.ReleaseComObject(BDSCon);
                //        if (ServerObject != null) Marshal.ReleaseComObject(ServerObject);
                //        ServerObject = null;
                //        HttpContext.Current.Response.AppendToLog("AddDocument, errore generale: " + Doc.XmlValues);
                //        HttpContext.Current.Response.AppendToLog("AddDocument, errore generale: " + exp.Message + " " + exp.StackTrace);
                //        if (sqlCon != null)
                //            if (sqlCon.State == ConnectionState.Open)
                //                sqlCon.Close();
                //        throw new Exception("AddDocument, errore generale: " + exp.Message + " " + exp.StackTrace);
                //    }
                //}

                int idDoc = 0;
                byte[] docHash = null;

                if (GestDiretta)  // Se gestione diretta usa RPC con servizio
                {
                    try
                    {
                        // direct RPC connection

                        uint Ext = 0;

                        // prepare file type for 
                        if (String.Compare(Doc.FileExtension, "Spool", true) == 0)
                            Ext = BCSTObjWmf1;

                        //if (Ext == 0 && Doc.FileExtension.Length >= 5)
                        //    if (Doc.FileExtension.ToUpper().EndsWith(".P7M"))
                        //    {
                        //        uint i;
                        //        // gestione P7M
                        //        for (i = 1; i < 32 && i * 4 < Doc.FileExtension.Length; i++)
                        //            if (Doc.FileExtension.Substring(Doc.FileExtension.Length - (int)i * 4, 4).ToUpper() != ".P7M")
                        //                break;

                        //        //1. bit a true e gli ultimi 6 bit di ogni char - 32
                        //        Ext = 0x800000 |
                        //            (((uint)Doc.FileExtension[0] - 32) << 17) |
                        //            (((uint)Doc.FileExtension[1] - 32) << 11) |
                        //            (((uint)Doc.FileExtension[2] - 32) << 5) | (i - 2);
                        //    }

                        if (Ext == 0 && Doc.FileExtension.Length > 0)
                        {
                            // ext normale
                            Ext = ((uint)Doc.FileExtension[0]) << 16;
                            if (Doc.FileExtension.Length > 1)
                                Ext |= ((uint)Doc.FileExtension[1]) << 8;
                            if (Doc.FileExtension.Length > 2)
                                Ext |= (uint)Doc.FileExtension[2];
                        }

                        if (Ext == 0)
                        {
                            if (sqlCon != null)
                                if (sqlCon.State == ConnectionState.Open)
                                    sqlCon.Close();
                            return (0);
                        }
                        Ext |= 0x01000000;

                        //TODO Chiamare il WS di storage
                        //int BDSCon = BCSLOpenLink("LWebSrv", Server, DataBase);

                        if (BDSCon == 0)
                        {
                            //HttpContext.Current.Response.AppendToLog("AddDocument, errore connessione");
                            if (sqlCon != null)
                                if (sqlCon.State == ConnectionState.Open)
                                    sqlCon.Close();
                            throw new Exception("AddDocument, errore connessione");
                        }

                        if (Chain == 0)
                            idChain = BCSLNewObj(BDSCon, 0);//TODO Creazione della catena
                        else
                            idChain = Chain;
                        //TODO Creazione dell'oggetto
                        //idDoc = BCSLNewObj(BDSCon, idChain);
                        if (idDoc == 0)
                        {
                            //HttpContext.Current.Response.AppendToLog("AddDocument, errore NewDoc");
                            if (sqlCon != null)
                                if (sqlCon.State == ConnectionState.Open)
                                    sqlCon.Close();
                            throw new Exception("AddDocument, errore NewDoc");
                        }
                        //Apertura oggetto su file System
                        //int BDSIdOOpn = BCSLOpenObj(BDSCon, idDoc, BCSOOMWrite | BCSOOMWrtCts);
                        if (BDSIdOOpn == 0)
                        {
                            //HttpContext.Current.Response.AppendToLog("AddDocument, errore OpnObj");
                            if (sqlCon != null)
                                if (sqlCon.State == ConnectionState.Open)
                                    sqlCon.Close();
                            throw new Exception("AddDocument, errore OpnObj");
                        }

                        //Carico caratteristiche dell'oggetto.
                        //Salva nella tabella oggetto.
                        BCSObjCts bufCts;
                        bufCts.Type = (int)Ext;
                        unsafe
                        {
                            BCSLSetObjCts(BDSCon, BDSIdOOpn, BCSSetCtsType, &bufCts);
                        }

                        //
                        Byte[] buf = System.Convert.FromBase64String(Doc.Blob);
                        if (GestAttributi)
                        {
                            SHA1 sha = new SHA1CryptoServiceProvider();
                            docHash = sha.ComputeHash(buf);
                        }

                        //OLD Salva il file fisico.....
                        //int sz = 32768, ps = 0;
                        //for (ps = 0; ps < buf.Length; )
                        //{
                        //    sz = ((buf.Length - ps > sz) ? sz : buf.Length - ps);
                        //    unsafe
                        //    {
                        //        fixed (Byte* ptr = &buf[ps])
                        //            if (BCSLSetObjData(BDSCon, BDSIdOOpn, ps, sz, ptr) == 0)
                        //            {
                        //                //HttpContext.Current.Response.AppendToLog("BCSLSetObjData, errore generico");
                        //                throw new Exception("BCSLSetObjData, errore generico");
                        //            }
                        //    }
                        //    ps += sz;
                        //}
                        //if (BCSLCloseObj(BDSCon, BDSIdOOpn) == 0)
                        //{
                        //    HttpContext.Current.Response.AppendToLog("BCSLSetObjData, errore generico");
                        //    throw new Exception("BCSLSetObjData, errore generico");
                        //}
                        //BCSLCloseLink(BDSCon);
                    }
                    catch (System.Exception exp)
                    {
                        if (sqlCon != null)
                            if (sqlCon.State == ConnectionState.Open)
                                sqlCon.Close();
                        HttpContext.Current.Response.AppendToLog("AddDocument, errore generale: " + exp.Message + " " + exp.StackTrace);
                        throw new Exception("AddDocument, errore generale: " + exp.Message + " " + exp.StackTrace);
                    }
                }
                //HttpContext.Current.Trace.Write("AD 13");
                //TODO: GestAttributi nel nuovo sempre true
                if (GestAttributi)
                {
                    int ret = AddAttributi(Server, DataBase, idChain, Doc, idDoc, docHash, sqlCon, NomeCert, false);
                    if (sqlCon != null)
                        if (sqlCon.State == ConnectionState.Open)
                            sqlCon.Close();
                    return (ret);
                }
                else
                    return (idChain);
            }

            static int AddAttributi(string Server, string DataBase, int idChain, stDoc Doc, int idOgg,
                                      Byte[] docHash, SqlConnection sqlCon, string NomeCert, bool Modify)
            {
                string sCmd = "";
                SqlTransaction sqlTran = null;
                SqlCommand sqlCmd;
                Hashtable htCheckMetaData = new Hashtable(),
                    htAutoInc = new Hashtable();

                SqlDataReader sqlDataR;
                try
                {
                    // begin transaction
                    sqlTran = sqlCon.BeginTransaction();

                    // se update cancella precedenti
                    if (Modify)
                    {
                        // before collect data for checking
                        sqlCmd = new SqlCommand("SELECT ao.Nome, ao.Modificabile, vao.Valore"
                                               + "  FROM ValoreAttributoOggetto vao "
                                   + "       INNER JOIN AttributoOggetto ao ON vao.NomeAttributoOggetto=ao.Nome"
                                               + " WHERE vao.IdOggetto=" + idOgg.ToString(), sqlCon, sqlTran);
                        sqlDataR = sqlCmd.ExecuteReader();
                        while (sqlDataR.Read())
                            htCheckMetaData.Add(sqlDataR["Nome"].ToString(),
                                                sqlDataR["Modificabile"].ToString() + sqlDataR["Valore"].ToString());
                        sqlDataR.Close();

                        sqlCmd = new SqlCommand("DELETE ValoreAttributoOggetto WHERE IdOggetto=" + idOgg.ToString(), sqlCon, sqlTran);
                        sqlCmd.ExecuteNonQuery();

                        sqlCmd = new SqlCommand("DELETE EstensioniOggetto WHERE IdOggetto=" + idOgg.ToString(), sqlCon, sqlTran);
                        sqlCmd.ExecuteNonQuery();

                        sqlCmd = new SqlCommand("DELETE Oggetto_Conservazione WHERE IdOggetto=" + idOgg.ToString(), sqlCon, sqlTran);
                        sqlCmd.ExecuteNonQuery();
                    }

                    // gestione autoincrementanti
                    sqlCmd = new SqlCommand("SELECT ao.Nome, ao.TipoCampo"
                                          + "  FROM AttributoOggetto ao"
                                          + " WHERE ao.TipoCampo LIKE 'AUTOINC%'", sqlCon, sqlTran);
                    sqlDataR = sqlCmd.ExecuteReader();
                    while (sqlDataR.Read())
                        if (!htCheckMetaData.Contains(sqlDataR.GetString(0)))
                        {
                            string[] par = sqlDataR.GetString(1).Split('(');
                            string Linked = par[1].Replace(")", "").Trim();
                            htAutoInc.Add(sqlDataR.GetString(0), Linked);
                        }
                    sqlDataR.Close();

                    string[] Nomi_Valori, Nomi, Valori;
                    string DataDocumento, ChiaveUnivoca;
                    int row;

                    LocCheckMetaData(sqlCon, sqlTran, Doc.XmlValues, htAutoInc, out row, out Nomi_Valori, out Nomi, out Valori,
                                       out ChiaveUnivoca, out DataDocumento);

                    // calc sign
                    DateTime dtn = DateTime.Now;
                    string date = dtn.ToString("yyyyMMdd");

                    Byte[] sign = Sign(idOgg, idChain, docHash, Doc.FileExtension, date, Nomi_Valori, NomeCert);


                    // add EstensioniOggetto
                    for (int i = 0; i < MAX_ATTR && Nomi[i] != null; i++)
                    {
                        // chech per modificabile
                        if (Modify)
                        {
                            string sMod = (string)htCheckMetaData[Nomi[i]];
                            switch (int.Parse(sMod.Substring(0, 1)))
                            {
                                case 0: //Non modificabile dopo inserimento
                                    if (string.Compare(sMod.Substring(1), Valori[i]) == 0)
                                        break;
                                    throw new Exception("Errore manca diritto modifica sul campo: " + Nomi[i]);
                                case 1: //Modificabile se vuoto
                                    if (string.Compare(sMod.Substring(1), Valori[i]) != 0 && sMod.Length > 1)
                                        throw new Exception("Errore manca diritto modifica sul campo valorizzato: " + Nomi[i]);
                                    break;
                                case 2: //Modificabile se non archiviato
                                    if (string.Compare(sMod.Substring(1), Valori[i]) == 0)
                                        break;
                                    sqlCmd = new SqlCommand("SELECT IdOggetto FROM Oggetto_Conservazione"
                                        + " WHERE IdOggetto=" + idOgg.ToString() + " AND IdConservazione IS NOT NULL", sqlCon, sqlTran);
                                    sqlDataR = sqlCmd.ExecuteReader();
                                    if (sqlDataR.Read())
                                        throw new Exception("Errore manca diritto modifica sul oggetto Conservato: " + Nomi[i]);
                                    sqlDataR.Close();
                                    break;
                                case 3: //Modificabile sempre
                                    break;
                            }
                        }

                        sCmd = "INSERT INTO ValoreAttributoOggetto(NomeAttributoOggetto,IdOggetto,Valore)"
                            + "VALUES('" + Nomi[i].Replace("'", "''") + "'," + idOgg.ToString() + ",'"
                            + Valori[i].Replace("'", "''") + "')";
                        sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                        sqlCmd.ExecuteNonQuery();
                    }
                    // creazione Oggetto_Conservazione
                    if (ChiaveUnivoca == null)
                        sCmd = "INSERT INTO Oggetto_Conservazione(IdOggetto,ChiaveUnivoca,DataOggetto)"
                            + "SELECT " + idOgg.ToString() + ",null,'" + DataDocumento + "'";
                    else
                        sCmd = "INSERT INTO Oggetto_Conservazione(IdOggetto,ChiaveUnivoca,DataOggetto)"
                          + "SELECT " + idOgg.ToString() + ",'" + ChiaveUnivoca + "','" + DataDocumento + "'"
                          + " WHERE NOT EXISTS(SELECT * FROM Oggetto_Conservazione WHERE ChiaveUnivoca='" + ChiaveUnivoca + "')";

                    sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                    if (sqlCmd.ExecuteNonQuery() != 1)
                    {
                        // gestione chiave univoca...
                        HttpContext.Current.Response.AppendToLog("<b>AddAttributi, errore Chiave principale duplicata</b>");
                        throw new Exception("<b>AddAttributi, errore Chiave principale duplicata</b>");
                    }

                    // creazione EstensioniOggetto
                    sCmd = "INSERT INTO EstensioniOggetto(IdOggetto,DataCreazione,Impronta,Firma,NomeCertificato)"
                        + "VALUES(" + idOgg.ToString() + ",'" + date + "',@impronta,@firma,'" + NomeCert + "')";

                    sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                    sqlCmd.Parameters.Add("@impronta", docHash);
                    sqlCmd.Parameters.Add("@firma", sign);
                    sqlCmd.ExecuteNonQuery();

                    // flush
                    sqlTran.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        sqlTran.Rollback();
                    }
                    catch (Exception ex) { }
                    HttpContext.Current.Response.AppendToLog("AddAttributi, errore generale: " + e.Message + " " + e.StackTrace);
                    throw new Exception("AddAttributi, errore generale: " + e.Message + " " + e.StackTrace + " " + sCmd);
                }
                return (idChain);
            }

            static void LocCheckMetaData(SqlConnection sqlCon, SqlTransaction sqlTran, string XmlMetaData, Hashtable htAutoInc,
                                     out int row, out string[] Nomi_Valori, out string[] Nomi, out string[] Valori,
                                                   out string ChiaveUnivoca, out string DataDocumento)
            {
                string[] ValCU = new string[MAX_ATTR];
                string sCmd;
                SqlCommand sqlCmd;

                Nomi_Valori = new string[MAX_ATTR];
                Nomi = new string[MAX_ATTR];
                Valori = new string[MAX_ATTR];
                ValCU = new string[MAX_ATTR];
                ChiaveUnivoca = null;
                DataDocumento = "";
                row = 0;

                // costruisci array ValoreAttributoOggetto
                XmlDocument document = new XmlDocument();
                NameTable nt = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
                XmlParserContext context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                XmlTextReader reader = new XmlTextReader(XmlMetaData, XmlNodeType.Element, context);
                document.Load(reader);

                XmlNode root = document.SelectSingleNode("Object");
                XmlNodeList atts = root.SelectNodes("Attribute");		// read attributes

                SqlDataReader sqlDataR;

                // gestione autoincrementanti
                Hashtable htAutoIncVal = new Hashtable();
                if (htAutoInc != null)
                    foreach (DictionaryEntry obj in htAutoInc)
                    {
                        string Name = (string)obj.Key, LnkdName = (string)obj.Value;
                        bool exists = false;

                        foreach (XmlNode att in atts)
                            if (string.Compare(att.Attributes["Name"].InnerText.ToString(), Name, false) == 0)
                                exists = true;
                        if (!exists)
                        {
                            string LnkdValue = "null";
                            foreach (XmlNode att in atts)
                                if (string.Compare(att.Attributes["Name"].InnerText.ToString(), LnkdName, false) == 0)
                                    LnkdValue = (string)att.InnerText;

                            // check count
                            sCmd = "SELECT Valore FROM Parametro WHERE Nome='AUTOINC_" + LnkdName + "_" + LnkdValue + "'";
                            if (sqlTran != null)
                                sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                            else
                                sqlCmd = new SqlCommand(sCmd, sqlCon);

                            string value = "";
                            sqlDataR = sqlCmd.ExecuteReader();
                            if (sqlDataR.Read())
                            {
                                value = sqlDataR.GetString(0);
                                sqlDataR.Close();
                                int i;
                                // loop per concomitanza
                                for (i = 0; i < 10; i++)
                                {
                                    sCmd = "UPDATE Parametro SET Valore=" + value + "+1 "
                                          + " WHERE Valore=" + value + " AND Nome='AUTOINC_" + LnkdName + "_" + LnkdValue + "'";
                                    if (sqlTran != null)
                                        sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                                    else
                                        sqlCmd = new SqlCommand(sCmd, sqlCon);
                                    if (sqlCmd.ExecuteNonQuery() == 1)
                                    {
                                        value = (int.Parse(value) + 1).ToString();
                                        break;
                                    }
                                    // reload counter
                                    sCmd = "SELECT Valore FROM Parametro WHERE Nome='AUTOINC_" + LnkdName + "_" + LnkdValue + "'";
                                    if (sqlTran != null)
                                        sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                                    else
                                        sqlCmd = new SqlCommand(sCmd, sqlCon);

                                    sqlDataR = sqlCmd.ExecuteReader();
                                    value = sqlDataR.GetString(0);
                                    sqlDataR.Close();
                                }
                                if (i == 10)
                                    throw new Exception("<b>Errore in inserimento campo autonicrementante</b>");
                            }
                            else
                            {
                                sqlDataR.Close();
                                sCmd = "INSERT INTO Parametro(Nome,Valore) VALUES('AUTOINC_" + LnkdName + "_" + LnkdValue + "',1)";
                                if (sqlTran != null)
                                    sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                                else
                                    sqlCmd = new SqlCommand(sCmd, sqlCon);
                                sqlCmd.ExecuteNonQuery();
                                value = "1";
                            }
                            htAutoIncVal.Add(Name, value);
                        }
                    }

                sCmd = "SELECT Nome,Obbligatorio,DataPrincipale,PosizioneInChiaveUnivoca,"
                    + "       PorzioneInChiaveUnivoca,FormatoInChiaveUnivoca,Validazione, Formato"
                    + "  FROM AttributoOggetto"
                    + " WHERE Disabilitato=0 OR Disabilitato IS NULL"
                    + " ORDER BY Nome";
                if (sqlTran != null)
                    sqlCmd = new SqlCommand(sCmd, sqlCon, sqlTran);
                else
                    sqlCmd = new SqlCommand(sCmd, sqlCon);
                sqlDataR = sqlCmd.ExecuteReader();
                while (sqlDataR.Read())
                {
                    string Valore = null;
                    // get data
                    int PosizioneInChiaveUnivoca;
                    string PorzioneInChiaveUnivoca, FormatoInChiaveUnivoca, Validazione;
                    bool DataPrincipale, Obbligatorio;

                    string Nome = sqlDataR["Nome"].ToString();

                    foreach (XmlNode att in atts)
                        if (string.Compare(att.Attributes["Name"].InnerText.ToString(), Nome, false) == 0)
                            Valore = att.InnerText.ToString();

                    if (Valore == null && htAutoIncVal.Contains(Nome))
                        Valore = htAutoIncVal[Nome].ToString();

                    if (sqlDataR["Obbligatorio"] != System.DBNull.Value)
                        Obbligatorio = string.Compare(sqlDataR["Obbligatorio"].ToString(), "1") == 0;
                    else
                        Obbligatorio = false;
                    if (sqlDataR["DataPrincipale"] != System.DBNull.Value)
                        DataPrincipale = string.Compare(sqlDataR["DataPrincipale"].ToString(), "1") == 0;
                    else
                        DataPrincipale = false;
                    if (sqlDataR["PosizioneInChiaveUnivoca"] != System.DBNull.Value)
                        PosizioneInChiaveUnivoca = int.Parse(sqlDataR["PosizioneInChiaveUnivoca"].ToString());
                    else
                        PosizioneInChiaveUnivoca = 0;
                    if (sqlDataR["PorzioneInChiaveUnivoca"] != System.DBNull.Value)
                        PorzioneInChiaveUnivoca = sqlDataR["PorzioneInChiaveUnivoca"].ToString();
                    else
                        PorzioneInChiaveUnivoca = "";
                    if (sqlDataR["FormatoInChiaveUnivoca"] != System.DBNull.Value)
                        FormatoInChiaveUnivoca = sqlDataR["FormatoInChiaveUnivoca"].ToString();
                    else
                        FormatoInChiaveUnivoca = "";
                    if (sqlDataR["Validazione"] != System.DBNull.Value)
                        Validazione = sqlDataR["Validazione"].ToString();
                    else
                        Validazione = "";

                    // check
                    if (DataPrincipale == true)
                        if (sqlDataR["Formato"] != System.DBNull.Value && sqlDataR["Validazione"] != System.DBNull.Value)
                            DataDocumento = Format(Valore, sqlDataR["Validazione"].ToString(), sqlDataR["Formato"].ToString());
                        else
                            DataDocumento = DateTime.Parse(Valore).ToString("yyyyMMdd");
                    //DataDocumento=Valore; //to do check

                    if (Obbligatorio && Valore == null)
                    {
                        sqlDataR.Close();
                        throw new Exception("<b>Valore obbligatorio mancante (" + Nome + ")</b>");
                    }
                    if (PosizioneInChiaveUnivoca != 0)
                    {
                        //if(Valore==null)
                        //  ChiaveUnivoca=null;
                        //else
                        //{
                        try
                        {
                            ValCU[PosizioneInChiaveUnivoca] =
                              FormatChiaveUnivoca(PorzioneInChiaveUnivoca, FormatoInChiaveUnivoca, Valore);
                        }
                        catch (Exception e)
                        {
                            sqlDataR.Close();
                            throw new Exception("Errore in validazione chiave univoca (" + Nome + "): " + e.Message);
                        }
                        //}
                    }
                    if (Validazione.Length > 0 && Valore != null)
                    {
                        try
                        {
                            Regex r = new Regex(Validazione);
                            if (!r.Match(Valore).Success)
                                throw new Exception("Validazione fallita (" + Nome + ")");
                        }
                        catch (Exception e)
                        {
                            sqlDataR.Close();
                            throw new Exception("<b>Errore in check validazione (" + Nome + "): " + e.Message + "</b>");
                        }
                    }
                    if (Valore != null)
                    {
                        // add to array
                        Nomi_Valori[row] = Nome + "=" + Valore;
                        Nomi[row] = Nome;
                        Valori[row] = Valore;
                        row++;
                    }
                }
                sqlDataR.Close();

                // check campi in più
                foreach (XmlNode att in atts)
                {
                    int i;
                    string Nome = att.Attributes["Name"].InnerText.ToString();
                    for (i = 0; i < row; i++)
                        if (string.Compare(Nome, Nomi[i], false) == 0)
                            break;
                    if (i == row)
                        throw new Exception("<b>Metadato non esistente (" + Nome + ")</b>");
                }

                // genera chiave univoca
                //if(ChiaveUnivoca!=null)
                for (int i = 0; i < ValCU.Length; i++)
                    if (ValCU[i] != null)
                        if (ChiaveUnivoca != null)
                            ChiaveUnivoca = ChiaveUnivoca + "|" + ValCU[i];
                        else
                            ChiaveUnivoca = ValCU[i];
            }

            static string FormatChiaveUnivoca(string PorzioneInChiaveUnivoca, string FormatoInChiaveUnivoca, string Value)
            {
                try
                {
                    if (PorzioneInChiaveUnivoca.Length == 0)
                        return Value;
                    Regex r = new Regex(PorzioneInChiaveUnivoca);
                    if (FormatoInChiaveUnivoca.Length == 0)
                        return r.Match(Value).Value;

                    if (FormatoInChiaveUnivoca.IndexOf("|") < 0)
                        return r.Replace(Value, FormatoInChiaveUnivoca);

                    string[] arg = FormatoInChiaveUnivoca.Split('|');
                    object[] argo = new object[arg.Length];

                    for (int i = 1; i < arg.Length; i++)
                    {
                        string Valore = r.Replace(Value, arg[i].Substring(1));
                        switch (arg[i].Substring(0, 1))
                        {
                            case "d":
                                argo[i] = int.Parse(Valore);
                                break;
                            case "n":
                                argo[i] = decimal.Parse(Valore);
                                break;
                            default:
                            case "s":
                                argo[i] = Valore;
                                break;
                        }
                    }
                    return string.Format(new System.Globalization.CultureInfo("it-IT"), arg[0], argo);
                }
                catch (Exception ex)
                {
                    return "";
                }

                // sintassi  {1,00}/{2,00}/{3}|n${day}|n${month}|n${year}
                // La prima parte è la sintassi di Formattazione standard di .Net seguita dai parametri in sequenza.
                // I parametri iniziano con il tipo in cui vengono convertiti (n=number, d=decimal... secondo lo standard .Net), in un secondo tempo si potrebbe specificare tra [] la nazionalità della decodifica, es [en] per l'Inghlterra da passare al parser. Attualmente opera come formato italiano->italiano
            }



            static string GetInfo(string Server, string DataBase, int Chain, int Enum, string Attr, string Value)
            {
                const int BCSTObjWmf1 = 0x00000200;
                SqlConnection sqlCon = null;
                string DocDI = "", Bib2kTabName = "", Bib2kIdField = "";
                Type ServerType;
                //Per ora la ignoriamo, serve per la sicurezza sull'accesso
                //Server = CheckChiamata(Server, DataBase, Chain.ToString(), "");            
                bool Biblos2000 = false, OmitInv = false;

                string val = ConfigurationSettings.AppSettings["OmitInvisible"];
                OmitInv = val != null && val == "true" ? true : false;

                val = ConfigurationSettings.AppSettings["ForceFileName"];
                bool ForceFileName = val != null && val == "true" ? true : false;

                // Biblos2000/BiblosDS direct/BDSDCOM
                try
                {
                    string ConStr = BiblosDSConnectionString; //ConfigurationSettings.AppSettings["SQLConnection." + Server];

                        // BiblosDS direct

                    sqlCon = new SqlConnection(BiblosDSConnectionString); //ConStr + DataBase);
                    sqlCon.Open();


                    if (sqlCon != null)
                    {
                        DocDI = "<?xml version=\"1.0\" ?>\r\n" +
                            "<BiblosDS Server=\"" + Server + "\" Database=\"" + DataBase + "\">\r\n";

                        string sCmd;
                        SqlCommand sqlCmd = null;
                        SqlDataReader sqlDataR = null;

                        //OLD Opzione per ricerca
                        //_________________________
                            //if (Attr.Length > 0)
                            //{
                            //    if (string.Compare(Attr, "Chiave univoca", true) == 0)
                            //        sCmd = "SELECT o.IdOggetto,o.IdOggettoPadre,eo.DataCreazione,u.DimNonComp,"
                            //          + "        o.Attributi,o.IdTipoOggetto, vao.NomeAttributoOggetto, vao.Valore"
                            //          + "  FROM Oggetto o"
                            //          + "       LEFT JOIN ValoreAttributoOggetto vao ON o.IdOggetto=vao.IdOggetto"
                            //          + "       LEFT JOIN EstensioniOggetto eo ON  o.IdOggetto=eo.IdOggetto"
                            //          + "       INNER JOIN Ubicazione u ON o.IdOggetto=u.IdOggetto"
                            //          + "       INNER JOIN Oggetto_Conservazione flt ON o.IdOggetto=flt.IdOggetto"
                            //          + " WHERE flt.ChiaveUnivoca LIKE '" + Value + "'"
                            //          + " ORDER BY o.IdOggettoPadre,o.IdOggetto, vao.NomeAttributoOggetto";
                            //    else
                            //        sCmd = "SELECT o.IdOggetto,o.IdOggettoPadre,eo.DataCreazione,u.DimNonComp,"
                            //          + "        o.Attributi,o.IdTipoOggetto, vao.NomeAttributoOggetto, vao.Valore"
                            //          + "  FROM Oggetto o"
                            //          + "       LEFT JOIN EstensioniOggetto eo ON o.IdOggetto=eo.IdOggetto"
                            //          + "       LEFT JOIN ValoreAttributoOggetto vao ON o.IdOggetto=vao.IdOggetto"
                            //          + "       INNER JOIN Ubicazione u ON o.IdOggetto=u.IdOggetto"
                            //          + "       INNER JOIN ValoreAttributoOggetto flt ON o.IdOggetto=flt.IdOggetto"
                            //          + " WHERE flt.Valore LIKE '" + Value + "'"
                            //          + " ORDER BY o.IdOggettoPadre,o.IdOggetto, vao.NomeAttributoOggetto";
                            //    sqlCmd = new SqlCommand(sCmd, sqlCon);
                            //    sqlDataR = sqlCmd.ExecuteReader();
                            //}
                            //else
                            //{
                                //TODO DEFINIRE SE PORTARE NEL NUOVO.......
                                //Database Biblos-LEX
                                sCmd = "SELECT o.IdOggetto,o.IdOggettoPadre,eo.DataCreazione,u.DimNonComp,"
                                  + "        o.Attributi,o.IdTipoOggetto, vao.NomeAttributoOggetto, vao.Valore, flt.IdConservazione"
                                  + "  FROM Oggetto o"
                                  + "       LEFT JOIN EstensioniOggetto eo ON o.IdOggetto=eo.IdOggetto"
                                  + "       LEFT JOIN ValoreAttributoOggetto vao ON o.IdOggetto=vao.IdOggetto"
                                  + "       LEFT JOIN Oggetto_Conservazione flt ON o.IdOggetto=flt.IdOggetto"
                                  + "       INNER JOIN Ubicazione u ON o.IdOggetto=u.IdOggetto"
                                  + " WHERE o.IdOggettoPadre=" + Chain.ToString()
                                  + " ORDER BY o.IdOggetto, vao.NomeAttributoOggetto";

                                sqlCmd = new SqlCommand(sCmd, sqlCon);

                                try
                                {
                                    sqlDataR = sqlCmd.ExecuteReader();
                                }
                                catch (Exception e)
                                {                                
                                    try
                                    {
                                        //Database Biblos-DOCSUITE
                                        sCmd = "SELECT o.IdOggetto,o.IdOggettoPadre,eo.DataCreazione,u.DimNonComp,"
                                          + "        o.Attributi,o.IdTipoOggetto, vao.NomeAttributoOggetto, vao.Valore"
                                          + "  FROM Oggetto o"
                                          + "       LEFT JOIN EstensioniOggetto eo ON o.IdOggetto=eo.IdOggetto"
                                          + "       LEFT JOIN ValoreAttributoOggetto vao ON o.IdOggetto=vao.IdOggetto"
                                          + "       INNER JOIN Ubicazione u ON o.IdOggetto=u.IdOggetto"
                                          + " WHERE o.IdOggettoPadre=" + Chain.ToString()
                                          + " ORDER BY o.IdOggetto, vao.NomeAttributoOggetto";

                                        sqlCmd = new SqlCommand(sCmd, sqlCon);
                                        sqlDataR = sqlCmd.ExecuteReader();
                                    }
                                    catch (Exception ex)
                                    {
                                        //Database Biblos-1.0
                                        sCmd = "SELECT o.IdOggetto,o.IdOggettoPadre, GETDATE() AS DataCreazione,u.DimNonComp,"
                                             + "        o.Attributi,o.IdTipoOggetto, NULL AS NomeAttributoOggetto, NULL AS Valore"
                                             + "  FROM Oggetto o INNER JOIN Ubicazione u ON o.IdOggetto=u.IdOggetto"
                                             + " WHERE o.IdOggettoPadre=" + Chain.ToString()
                                             + " ORDER BY o.IdOggetto, vao.NomeAttributoOggetto";

                                        sqlCmd = new SqlCommand(sCmd, sqlCon);
                                        sqlDataR = sqlCmd.ExecuteReader();
                                    }

                                }
                            //}

                        int lastId = 0, lastCat = 0, curEnum = -1; ;
                        //if (!sqlDataR.HasRows && !Biblos2000)
                            DocDI += "<Chain>\r\n";

                        int lastNode = 0, lastVis = 0;
                        string type = "";

                        while (sqlDataR.Read())                       
                            {
                                int idOgg = sqlDataR.GetInt32(0),
                                    idCat = sqlDataR.GetInt32(1);
                                if (idCat != lastCat)
                                {
                                    if (lastCat > 0)
                                        DocDI += "</Chain>\r\n";
                                    DocDI += "<Chain Id=\"" + idCat.ToString() + "\">\r\n";
                                    lastCat = idCat;
                                }
                                if (idOgg != lastId)
                                    curEnum++;
                                if (Enum != -1 && curEnum != Enum)
                                {
                                    lastId = idOgg;
                                    continue;
                                }
                                if (idOgg != lastId)
                                {
                                    if (lastId != 0 && Enum == -1)
                                        if (lastVis != 0 || !OmitInv)
                                            DocDI += "</Object>\r\n";

                                    lastVis = sqlDataR.GetInt32(4) & 0x00000001 ^ 0x00000001;
                                    string IdCons = "";
                                    if (sqlDataR.GetInt32(5) == BCSTObjWmf1)
                                        type = "WMF";
                                    else
                                        type = ((char)((sqlDataR.GetInt32(5) / 65536) & 0x000000ff)).ToString() +
                                                                ((char)((sqlDataR.GetInt32(5) / 256) & 0x000000ff)).ToString() +
                                                                ((char)((sqlDataR.GetInt32(5)) & 0x000000ff)).ToString();//Conversione esadecimale dell'estenzione del file.
                                    try
                                    {
                                        IdCons = sqlDataR["IdConservazione"].ToString();
                                    }
                                    catch (Exception e) { }

                                    lastNode = DocDI.Length;
                                    if (lastVis != 0 || !OmitInv)
                                        DocDI += "<Object Enum=\"" + curEnum.ToString() + "\"" +
                                                              " DataCreazione=\"" + (sqlDataR["DataCreazione"] == System.DBNull.Value ? "" : sqlDataR.GetDateTime(2).ToString("yyyy/MM/dd")) + "\"" +
                                                              " IdConservazione=\"" + IdCons + "\"" +
                                                              " Size=\"" + sqlDataR["DimNonComp"].ToString() + "\"" +
                                                              " Background=\"\"" +
                                                              " Visible=\"" + lastVis.ToString() + "\"" +
                                                              " Type=\"" + type + "\">\r\n";

                                    lastId = idOgg;
                                }
                                if (sqlDataR["NomeAttributoOggetto"] != System.DBNull.Value && sqlDataR["Valore"] != System.DBNull.Value)
                                {
                                    if (lastVis != 0 || !OmitInv)
                                        DocDI += "<Attribute Name=\"" + sqlDataR["NomeAttributoOggetto"].ToString() + "\">" +
                                        sqlDataR["Valore"].ToString() + "</Attribute>\r\n";
                                    if (string.Compare(sqlDataR["NomeAttributoOggetto"].ToString(), "FileName", true) == 0)
                                    {
                                        string[] ss = sqlDataR["Valore"].ToString().Split('.');
                                        if (ss.Length > 1)
                                            if (lastVis != 0 || !OmitInv)
                                                DocDI = DocDI.Substring(0, lastNode) +
                                                    DocDI.Substring(lastNode).Replace(" Type=\"" + type + "\">\r\n", " Type=\"" + ss[ss.Length - 1] + "\">\r\n");
                                    }
                                }
                                else
                                    if (ForceFileName)
                                    {
                                        // force def file name if not present atributes
                                        DocDI += "<Attribute Name=\"Filename\">file." + type + "</Attribute>\r\n";
                                    }
                            }
                        sqlDataR.Close();
                        if (curEnum >= 0)
                            if (lastVis != 0 || !OmitInv)
                                DocDI += "</Object>\r\n";
                        DocDI += "</Chain>\r\n</BiblosDS>\r\n";

                        if (sqlCon != null)
                            if (sqlCon.State == ConnectionState.Open)
                                sqlCon.Close();
                    }
                    //else
                    //{
                    //    // use BDSDCOM

                    //    Object ServerObject = null;
                    //    BDSCOMLib.Connection BDSCon = null;

                    //    try
                    //    {
                    //        //BDSCon = new BDSCOMLib.Connection();
                    //        ServerType = Type.GetTypeFromProgID("BiblosDS.Connection", Server, false);
                    //        ServerObject = Activator.CreateInstance(ServerType);
                    //        BDSCon = (BDSCOMLib.Connection)Marshal.CreateWrapperOfType(ServerObject, ServerType);

                    //    }
                    //    catch (System.Exception exp)
                    //    {
                    //        HttpContext.Current.Response.AppendToLog("GetInfo, errore in instanziazione BDSDCOM: " + exp.Message + " " + exp.StackTrace);
                    //        throw new Exception(exp.Message + " " + exp.StackTrace);
                    //    }

                    //    BDSCOMLib.DocumentCollections BDSCols = null;
                    //    BDSCOMLib.DocumentCollection BDSCol = null;
                    //    BDSCOMLib.Documents BDSDocs = null;
                    //    BDSCOMLib.Document BDSDoc = null;

                    //    try
                    //    {
                    //        BDSCon.OpenConnection("BiblosDS WebSvc", DataBase);
                    //        BDSCon.OpenDocumentCollection(Chain);
                    //        BDSCols = (BDSCOMLib.DocumentCollections)BDSCon.DocumentCollections;
                    //        BDSCol = (BDSCOMLib.DocumentCollection)BDSCols.get_Item(1);
                    //        BDSDocs = (BDSCOMLib.Documents)BDSCol.Documents;

                    //        if (Enum == -1)
                    //        {
                    //            DocDI = "<?xml version=\"1.0\" ?>\r\n" +
                    //                "<BiblosDS Server=\"" + Server + "\" Database=\"" + DataBase + "\">\r\n" +
                    //                "<Chain Id=\"" + Chain.ToString() + "\">\r\n";
                    //            for (int i = 0; i < BDSDocs.Count; i++)
                    //            {
                    //                BDSDoc = (BDSCOMLib.Document)BDSDocs.get_Item(i + 1);
                    //                DocDI += DocumentInfoXml(BDSDoc, i, null);
                    //                Marshal.ReleaseComObject(BDSDoc);
                    //            }
                    //            DocDI += "</Chain>\r\n</BiblosDS>\r\n";
                    //        }
                    //        else
                    //        {
                    //            BDSDoc = (BDSCOMLib.Document)BDSDocs.get_Item(Enum + 1);
                    //            string s = DocumentInfoXml(BDSDoc, Enum, sqlCon);
                    //            DocDI = "<?xml version=\"1.0\" ?>\r\n" +
                    //                "<BiblosDS Server=\"" + Server + "\" Database=\"" + DataBase + "\">\r\n" +
                    //                "<Chain Id=\"" + Chain.ToString() + "\">\r\n" +
                    //                s +
                    //                "</Chain>\r\n</BiblosDS>\r\n";
                    //            Marshal.ReleaseComObject(BDSDoc);
                    //        }
                    //    }
                    //    catch (System.Exception exp)
                    //    {
                    //        if (BDSDocs != null) Marshal.ReleaseComObject(BDSDocs);
                    //        if (BDSCols != null) Marshal.ReleaseComObject(BDSCols);
                    //        if (BDSCol != null) Marshal.ReleaseComObject(BDSCol);
                    //        if (BDSCon != null) Marshal.ReleaseComObject(BDSCon);
                    //        if (ServerObject != null) Marshal.ReleaseComObject(ServerObject);
                    //        ServerObject = null;
                    //        if (sqlCon != null)
                    //            if (sqlCon.State == ConnectionState.Open)
                    //                sqlCon.Close();
                    //        HttpContext.Current.Response.AppendToLog("GetInfo, errore generale: " + exp.Message + " " + exp.StackTrace);
                    //        throw new Exception(exp.Message + " " + exp.StackTrace);
                    //    }

                    //    Marshal.ReleaseComObject(BDSDocs);
                    //    Marshal.ReleaseComObject(BDSCols);
                    //    Marshal.ReleaseComObject(BDSCol);
                    //    Marshal.ReleaseComObject(BDSCon);
                    //    Marshal.ReleaseComObject(ServerObject);
                    //    ServerObject = null;
                    //}
                }
                catch (System.Exception exp)
                {
                    if (sqlCon != null)
                        if (sqlCon.State == ConnectionState.Open)
                            sqlCon.Close();
                    //HttpContext.Current.Response.AppendToLog("GetInfo, errore generale: " + exp.Message + " " + exp.StackTrace);
                    throw new Exception(exp.Message + " " + exp.StackTrace);
                }
                return DocDI;
            }

            public static stDoc GetDocumentStatic(string Server, string DataBase, int Chain, int Enum)
            {
                //Server = CheckChiamata(Server, DataBase, Chain.ToString(), "");
                stDoc rv;
                rv.FileExtension = "";
                rv.XmlValues = "";
                rv.Blob = "";

                string att = ConfigurationSettings.AppSettings["ControlloAttributi"];
                string mod = ConfigurationSettings.AppSettings["AttributiObbligatori"];
                string moddb = ConfigurationSettings.AppSettings["AttributiObbligatori." + DataBase];


                // get document DATA using RPC 
                int idObj;

                Byte[] buf, docHash;
                try
                {
                    // get document
                    string FileType;
                    //Recupera il file dall'oggetto com
                    //La gestione degli ivisibili viene gestito a basso livello.
                    //buf = GetDSDocBlob(Server, DataBase, Chain, Enum, out FileType, out idObj);
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    docHash = sha.ComputeHash(buf);
                    rv.FileExtension = FileType;
                    rv.Blob = System.Convert.ToBase64String(buf, 0, buf.Length);
                }
                catch (System.Exception exp)
                {
                    //HttpContext.Current.Response.AppendToLog("GetDocument errore GetDSDocBlob (" + Server + "," + DataBase + "," + Chain.ToString() + "," + Enum.ToString() + ") : " + exp.Message + " " + exp.StackTrace);
                    throw new Exception(exp.Message + " " + exp.StackTrace);
                }

                // on the fly conversion BDO
                //TODO non gestiti nella nuova versione (WMF,BDO)
                //if (String.Compare(rv.FileExtension, "BDO") == 0 || String.Compare(rv.FileExtension, "WMF") == 0)
                //{
                //    String back = "";
                //    int i, l;
                //    for (i = 0; buf[i] != '=' && i < buf.Length; i++) ;
                //    for (l = ++i; buf[l] != '\r' && buf[l] > 0 && l < buf.Length; l++) ;
                //    back = System.Text.Encoding.Default.GetString(buf, i, l - i);

                //    string pars = ConfigurationSettings.AppSettings[DataBase + "_" + back];
                //    if (pars != null)
                //    {
                //        try
                //        {
                //            string[] par = pars.Split(';');


                //            string tmpiFile = System.IO.Path.GetTempFileName(),
                //                     tmpoFile = System.IO.Path.GetTempFileName();

                //            System.IO.FileStream fs = new System.IO.FileStream(tmpiFile, System.IO.FileMode.Create);
                //            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);

                //            bw.Write(buf, 0, buf.Length);
                //            bw.Close();
                //            fs.Close();
                //            Bdo2Pdf b2p = new Bdo2Pdf();

                //            //"c:/sfondi,0,720,0.05,-0.09,c:/WINDOWS/Fonts/ltype.ttf,8.9,sizeX,SizeY" />

                //            if (par.Length > 7)
                //            {
                //                b2p.width = float.Parse(par[7]);
                //                b2p.height = float.Parse(par[8]);
                //            }
                //            b2p.RenderBdo(tmpiFile, par[0], tmpoFile, float.Parse(par[1]), float.Parse(par[2]), float.Parse(par[3]), float.Parse(par[4]),
                //                par[5], float.Parse(par[6]));

                //            fs = new System.IO.FileStream(tmpoFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //            System.IO.BinaryReader sr = new System.IO.BinaryReader(fs);
                //            Byte[] lbuf = sr.ReadBytes((int)fs.Length);
                //            sr.Close();
                //            fs.Close();
                //            rv.FileExtension = "PDF";
                //            rv.Blob = System.Convert.ToBase64String(lbuf, 0, lbuf.Length);

                //            System.IO.File.Delete(tmpiFile);
                //            System.IO.File.Delete(tmpoFile);
                //            return rv;
                //        }
                //        catch (Exception ex)
                //        {
                //            HttpContext.Current.Response.AppendToLog("BDO Convert exception (" + Server + "," + DataBase + "," + Chain.ToString() + "," + Enum.ToString() + ") : " + ex.Message + " " + ex.StackTrace);
                //            throw new Exception("BDO Convert exception: " + ex.Message + " " + ex.StackTrace);
                //        }
                //    }
                //}

                // check del tipo di firma
                SqlConnection sqlCon = null;
                string GestDiretta = "";

                // controlla DB per usare in caso BDSDCOM
                //Non gestiamo BDSDCOM
                try
                {
                    string ConStr = ConfigurationSettings.AppSettings["SQLConnection." + Server];
                    if (ConStr != null)
                    {
                        sqlCon = new SqlConnection(ConStr + DataBase);
                        sqlCon.Open();
                        string sCmd = "SELECT NomeCertificato FROM EstensioniOggetto WHERE IdOggetto=" + idObj.ToString();
                        SqlCommand sqlCmd = new SqlCommand(sCmd, sqlCon);
                        SqlDataReader sqlDataR = sqlCmd.ExecuteReader();
                        if (sqlDataR.Read())
                            if (sqlDataR["NomeCertificato"] != System.DBNull.Value)
                                GestDiretta = sqlDataR["NomeCertificato"].ToString();
                        sqlDataR.Close();
                    }
                }
                catch (Exception e)
                {
                }

                if (GestDiretta.Length > 0)
                {
                    try
                    {
                        byte[] sO, sN;
                        int ver = CheckSignedDocument(sqlCon, Chain, idObj, rv.FileExtension, docHash, GestDiretta, out sO, out sN, false);
                        if ((ver == 0 && string.Compare(att, "true", true) == 0)
                            || (ver == -1 && string.Compare(mod, "true", true) == 0)
                            || (ver == -1 && string.Compare(moddb, "true", true) == 0))
                            throw new Exception("Verifica FALSE: " + ver.ToString());
                    }
                    catch (Exception e)
                    {
                        if (sqlCon != null)
                            if (sqlCon.State == ConnectionState.Open)
                                sqlCon.Close();
                        throw new Exception("Errore in Verifica: " + e.Message);
                    }
                    if (sqlCon != null)
                        if (sqlCon.State == ConnectionState.Open)
                            sqlCon.Close();
                }            
            }
             * */
        #endregion

        public static void SetDocumentPdf(Guid idDocument, string pdfName)
        {
            DbProvider.SetDocumentPdf(idDocument, pdfName);
        }

        public static void SetDocumentThumbnail(Guid idDocument, string thumbnailName)
        {
            DbProvider.SetDocumentThumbnail(idDocument, thumbnailName);
        }

        public static void UpdateDocumentName(Document item)
        {
            DbProvider.UpdateDocumentName(item);
        }


        public static void ResetDocumentAttributeValue(Document document)
        {
            try
            {
                document.SignHeader = AttributeService.GetAttributesHash(document.AttributeValues);
                DbProvider.UpdateDocumentAttributeValue(document, document.AttributeValues);
                AttributeService.UpdateAttributesHash(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void UpdateDocumentAttributeValue(Document document, DocumentAttributeValue attribute)
        {
            try
            {
                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                var docAttribute = document.AttributeValues.Where(x => x.IdAttribute == attribute.IdAttribute).FirstOrDefault();
                if (docAttribute == null)
                    document.AttributeValues.Add(attribute);
                else
                    docAttribute.Value = attribute.Value;
                document.SignHeader = AttributeService.GetAttributesHash(document.AttributeValues);
                DbProvider.UpdateDocumentAttributeValue(document, attribute);
                AttributeService.UpdateAttributesHash(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<string> GetPreservationFiscalDocumentsTypes()
        {
            return DbProvider.GetPreservationFiscalDocumentsTypes();
        }

        public static bool AddDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            try
            {
                if (DbProvider.CheckConnectionExists(IdDocument, IdDocumentLink))
                    return true;
                return DbProvider.AddDocumentLink(IdDocument, IdDocumentLink);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static bool DeleteDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            try
            {
                return DbProvider.DeleteDocumentLink(IdDocument, IdDocumentLink);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<Document> GetDocumentLink(Guid IdDocument)
        {
            try
            {
                return DbProvider.GetDocumentLink(IdDocument);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static DocumentAttach AddDocumentAttach(DocumentAttach att)
        {
            try
            {
                if (att == null)
                    throw new NullReferenceException("No attachment passed.");

                if (att.IdDocument == Guid.Empty)
                    throw new Exceptions.DocumentNotFound_Exception("The document ID is empty.");

                //Non si può inserire un allegato senza file
                if ((att.Content == null || att.Content.Blob == null || att.Content.Blob.Length <= 0))
                    throw new Exceptions.DocumentNotFound_Exception("Nessun file passato");

                var document = GetDocument(att.IdDocument);

                if (document == null)
                    throw new Exceptions.DocumentNotFound_Exception();

                if (document.Archive == null)
                    throw new Exceptions.Archive_Exception("Invalid archive configuration for document with IdDocument = " + document.IdDocument);

                document.Content = att.Content;
                att.Size = document.Content.Blob.Length;
                att.IsVisible = true;
                att.Document = document;
                att.IdDocumentAttach = Guid.NewGuid();
                //TODO Create DocumentContent_Null

                //Save the document to Transito Path
                if (document.Archive.TransitoEnabled)
                {
                    FileService.SaveFileToTransitoAttachLocalPath(att, document.Content.Blob);
                    //TODO if the insert is aborted delete the Transito Path file. 
                    //Set the status
                    att.Status = new Status((short)DocumentStatus.InTransito);
                }
                else
                {
                    if (document.Storage == null || document.StorageArea == null)
                        throw new Exceptions.DocumentNotReadyForAttach_Exception();

                    att.Status = new Status((short)DocumentStatus.InStorage);
                }


                return DbProvider.AddDocumentAttach(att, document.Archive.PathTransito);
            }
            catch (Exception ex)
            {
                try
                {
                    FileService.DeleteFileToTransitoAttachLocalPath(new DocumentAttachTransito { LocalPath = att.Document.Archive.PathTransito, Attach = att });
                }
                catch { }
                logger.Error(ex);
                throw;
            }
        }

        public static DocumentAttach UpdateDocumentAttach(DocumentAttach att)
        {
            try
            {
                return DbProvider.UpdateDocumentAttach(att);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<DocumentAttach> GetDocumentAttachesFromDocument(Guid? idDocument)
        {
            try
            {
                return DbProvider.GetDocumentAttachesFromDocument(idDocument);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static DocumentAttachTransito CheckOutTransitoAttach(Guid idDocumentAttach)
        {
            return DbProvider.TransitoAttachChangeState(idDocumentAttach, DocumentTarnsitoStatus.StorageProcessing);
        }

        public static DocumentAttachTransito CheckInTransitoAttach(Guid idDocumentAttach)
        {
            return DbProvider.TransitoAttachChangeState(idDocumentAttach, DocumentTarnsitoStatus.EndProcessing);
        }

        public static DocumentTransito UndoCheckOutTransitoAttach(Guid idDocumentAttach)
        {
            return DbProvider.TransitoAttachChangeState(idDocumentAttach, DocumentTarnsitoStatus.FaultProcessing);
        }

        public static BindingList<DocumentAttach> GetDocumentAttachesInTransito(Guid guid, int p)
        {
            throw new NotImplementedException();
        }

        public static BindingList<DocumentAttach> GetDocumentAttachesInTransito(int MaxRetry)
        {
            return DbProvider.GetDocumentAttachesInTransito(MaxRetry, false);
        }

        public static DocumentAttach GetDocumentAttach(Guid idDocumentAttach)
        {
            try
            {
                return DbProvider.GetDocumentAttach(idDocumentAttach);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void UndoLatestVersion(Guid undoDocumentId, Guid originalDocumentId)
        {
            try
            {
                DbProvider.UndoLatestVersion(undoDocumentId, originalDocumentId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Assegna i documenti appartenenti al batch passato (dovrebbe essere chiuso) ad un batch aperto ( se questo non esiste lo crea ).
        /// </summary>
        /// <param name="IdAwardBatch">Identifica il batch sorgente</param>
        /// <param name="isConservated">Sposta se true documenti conservati altrimenti da conservare</param>
        /// <param name="closeIfOpen">Se richiesto chiude il lotto prima di spostare i documenti</param>
        public static void MoveDocumentsAwardBatch(Guid idAwardBatch, bool isConservated, bool closeIfOpen)
        {
            try
            {
                //1. determina l'elenco dei documenti non conservati del batch
                var batch = DbProvider.GetAwardBatch(idAwardBatch);

                if (batch == null)
                    throw new Exceptions.AwardBatch_Exception("Lotto di versamento non trovato");

                if (batch.IsOpen)
                {
                    //se richiesto lo chiude
                    if (closeIfOpen)
                        DbProvider.CloseAwardBatch(idAwardBatch);
                    else
                        throw new Exceptions.AwardBatch_Exception("Lotto di versamento ancora aperto - Non è possibile spostare i documenti in nuovo lotto");
                }

                DbProvider.MoveDocumentsAwardBatch(batch, isConservated, closeIfOpen);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void AddDocumentToSQL(Document document, string fileName, byte[] content, DocumentType documentType)
        {
            try
            {
                AddToFileTable(document, fileName, content, documentType);
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void AddSearchableToFileTable(Document document, string fileName, byte[] content, DocumentType documentType)
        {
            try
            {
                AddToFileTable(document, fileName, content, documentType);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        private static void AddToFileTable(Document document, string fileName, byte[] content, DocumentType documentType)
        {
            string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
            DbProvider.AddToFileTable(safeStorageName, fileName, document, content, (int)documentType);
        }

        public static void RemoveDocumentFromSQLStorage(Document document)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
                DbProvider.RemoveDocumentFromSQLStorage(safeStorageName, document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static void RemoveSearchableDocumentFromStorage(Document document)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
                DbProvider.RemoveSearchableDocumentFromStorage(safeStorageName, document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static FileTableModel GetDocumentByNameFromSQLStorage(Document document, string name)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
                return DbProvider.GetDocumentByNameFromSQLStorage(safeStorageName, document, name);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static FileTableModel GetAttributesByDocumentFromSQLStorage(Document document)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
                return DbProvider.GetDocumentByIdFromSQLStorage(safeStorageName, document, (int)DocumentType.Attributes);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static FileTableModel GetDocumentFromSQLStorage(Document document)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(document.Storage.Name);
                return DbProvider.GetDocumentByIdFromSQLStorage(safeStorageName, document, (int)DocumentType.Default);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<Document> GetRemovableDetachedDocuments(Guid idArchive, DateTime? fromDate, DateTime? toDate)
        {
            return DbProvider.GetRemovableDetachedDocuments(idArchive, fromDate, toDate);
        }

        public static BindingList<Document> GetAllVersionedDocuments(Document document)
        {
            return DbProvider.GetAllVersionedDocuments(document);
        }

        public static void DeleteDocumentDetached(Guid idDocument)
        {
            DbProvider.DeleteDocumentDetached(idDocument);
        }

        public static Document GetById(Guid idDocument)
        {
            return DbProvider.GetById(idDocument);
        }

        public static Document GetDocumentIgnoreState(Guid idDocument)
        {
            return DbProvider.GetDocumentIgnoreState(idDocument);
        }

        public static bool HasDocumentChildren(Guid idParent)
        {
            return DbProvider.HasDocumentChildren(idParent);
        }

        public static Status GetDocumentStatus(Document document)
        {
            return DbProvider.GetDocumentStatus(document);
        }
        public static string GetPreservationDocumentPath(Objects.Document document)
        {
            return DbProvider.GetPreservationDocumentPath(document);
        }

        public static void UpdateDocumentStatus(Document document, Status status)
        {
            DbProvider.UpdateDocumentStatus(document, status);
        }
    }
}
