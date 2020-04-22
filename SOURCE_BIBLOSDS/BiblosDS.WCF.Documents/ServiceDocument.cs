using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.ComponentModel;

using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Enums;
using System.Security.Principal;
using System.Configuration;
using BiblosDS.Library.Common.Objects.UtilityService;

using iTextSharp.text.pdf;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

using BiblosDS.Library.Common.Utility;
using BiblosDS.Library.Common.Objects.Response;
using VecompSoftware.Common;
using VecompSoftware.CompEd;
using VecompSoftware.ServiceContract.BiblosDS.Documents;
using VecompSoftware.DataContract.Documents;

#if WCF_Documents
using BiblosDS.WCF.Documents.BoblosDSConvWs;
using BiblosDS.WCF.Documents.ServiceReferenceDigitalSigned;
using BiblosDS.Library.Common.Objects.Response;
#else
using BiblosDS.WCF.WCFServices.BoblosDSConvWs;
using BiblosDS.WCF.WCFServices.ServiceReferenceDigitalSign;
using System.ServiceModel.Activation;

#endif

#if WCF_Documents
namespace BiblosDS.WCF.Documents
{
#endif
    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.    
#if WCF_Documents
    public class ServiceDocument : IDocuments
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServiceDocument));
#else
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Documents : IDocuments
{
    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Documents));
#endif
        #region Document

        #region DocumentContent
        public DocumentContent GetDocumentContent(Guid idDocument, decimal? version, DocumentContentFormat outputFormat, bool? lastVersion)
        {
            try
            {
                DocumentContent result = GetDocumentContentById(idDocument);
                switch (outputFormat)
                {
                    case DocumentContentFormat.Binary:
                        return result;
                    case DocumentContentFormat.Base64:
                        return new DocumentContent { BlobString = Convert.ToBase64String(result.Blob), Description = result.Description };
                    case DocumentContentFormat.ConformBinary:
                        if (ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"].ToString()))
                        {

                            BiblosDSConv conv = new BiblosDSConv();
                            conv.Url = AzureService.GetSettingValue("BoblosDSConvWs.BiblosDSConv").ToString();

                            var doc = conv.ToRasterFormatEx(new stDoc { FileExtension = result.Description, Blob = Convert.ToBase64String(result.Blob) }, "pdf", "");
                            result.Blob = Convert.FromBase64String(doc.Blob);
                            result.Description += ".pdf";
                            return result;
                        }
                        else
                            throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Configurazione \"BoblosDSConvWs.BiblosDSConv\" non presente. Configurare per utilizzare il formato conforme.");
                    default:
                        throw new BiblosDS.Library.Common.Exceptions.Generic_Exception("Formato non supportato: " + outputFormat.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ex is FaultException)
                    throw;
                else
                {
                    logger.Error(ex);
                    throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
                }
            }
        }


        public DocumentContent GetDocumentContentById(Guid IdDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentContent IdDocument:{0}", IdDocument);
                DocumentContent content = null;
                Document document = DocumentService.GetDocument(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();

                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                ServerRole serverRole = ServerRole.Undefined;
                //BiblosDS Proxy
                var server = ServerService.GetCurrentServer();
                if (server != null)
                {
                    serverRole = server.ServerRole;
                }
                //
                string serverName = string.Empty;
                if (document.DocumentLink != null || document.Status.IdStatus != (int)DocumentStatus.ProfileOnly)
                {
                    if (serverRole == ServerRole.Proxy)
                    {
                        //Retrive document from master
                        var masterServer = ServerService.GetMasterServer();
                        logger.DebugFormat("Server is a proxy, Redirect to: {1} - IdDocument:{0}", IdDocument, masterServer.ServerName);
                        if (masterServer != null)
                        {
                            serverName = masterServer.ServerName;
                        }
                        else
                            throw new BiblosDS.Library.Common.Exceptions.ServerNotDefined_Exception("Nessun server master definito. Configurazione non valida per il server proxy: " + MachineService.GetServerName() + ".");
                    }
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName, serverName))
                    {
                        content = (clientChannel as IServiceDocumentStorage).GetDocument(document).Content;
                    }

                    if (content != null && string.IsNullOrEmpty(content.Description))
                        content.Description = document.Name ?? string.Empty;
                }
                else
                    throw new BiblosDS.Library.Common.Exceptions.DocumentWithoutContent_Exception(DocumentStatus.ProfileOnly);
                //
                if (document.Certificate != null)
                {
                    document.Content = content;
                    byte[] sO, sN;
                    DocumentCheckState result = DocumentService.CheckSignedDocument(
                        document,
                        document.DocumentParent.IdDocument,
                        out sO,
                        out sN,
                        false);
                    if ((int)result < 0)
                        throw new FaultException("Verifica fallita: " + result);
                }
                return content;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public DocumentContent GetDocumentConformContent(Guid idDocument, DocumentContentFormat outputFormat, string xmlLabel)
        {
            try
            {
                logger.DebugFormat("GetDocumentConformContent {0} {1}", idDocument, xmlLabel);
                var document = DocumentService.GetDocument(idDocument);
                if (document == null)
                    throw new DocumentNotFound_Exception();

                byte[] resFromCache = null;
                if ((resFromCache = CacheService.GetFromChache(document, document.Name, xmlLabel)) != null)
                    return new DocumentContent(resFromCache, document.Name);
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    if (!string.IsNullOrEmpty(document.IdPdf))
                        document = (clientChannel as IServiceDocumentStorage).GetDocumentConformAttach(document, document.IdPdf);
                    else
                        document = (clientChannel as IServiceDocumentStorage).GetDocument(document);
                }
                if (ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["BoblosDSConvWs.BiblosDSConv"].ToString()))
                {

                    BiblosDSConv conv = new BiblosDSConv();
                    conv.Url = AzureService.GetSettingValue("BoblosDSConvWs.BiblosDSConv").ToString();
                    if (string.IsNullOrEmpty(document.IdPdf) || !string.IsNullOrEmpty(xmlLabel))
                    {
                        var doc = conv.ToRasterFormatEx(new stDoc { FileExtension = string.IsNullOrEmpty(document.IdPdf) ? document.Name : document.IdPdf, Blob = Convert.ToBase64String(document.Content.Blob) }, "pdf", xmlLabel);
                        document.Content.Blob = Convert.FromBase64String(doc.Blob);
                        document.Content.Description = document.Name + ".pdf";
                        CacheService.AddCache(document, document.Content, document.Content.Description, xmlLabel);
                    }
                }
                else
                    document.Content.Description = document.Name;
                return document.Content;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Ritorna una specifica versione di un documento. Ritorna l'ultima versione del documento se non richiesta una specifica.
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="version"></param>
        /// <param name="lastVersion"></param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentInfo(Guid idDocument, decimal? version, bool? lastVersion)
        {
            try
            {
                logger.DebugFormat("GetDocumentInfo {0} {1} {2}", idDocument, version, lastVersion);
                BindingList<Document> documents = new BindingList<Document>();
                Document document;
                if (lastVersion == false)
                    document = DocumentService.GetDocument(idDocument, version);
                else
                    document = DocumentService.GetDocument(idDocument);
                if (document == null)
                    throw new DocumentNotFound_Exception();
                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                documents.Add(document);
                logger.DebugFormat("GetDocumentInfo Returns {0} documents", documents.Count);
                return documents;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document GetDocumentInfoById(Guid IdDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentInfo IdDocument:{0}", IdDocument);
                Document document = null;
                if (ConfigurationManager.AppSettings["ReturnAlwaysLatestVersion"] != null && ConfigurationManager.AppSettings["ReturnAlwaysLatestVersion"].ToString().Equals("false"))
                    document = DocumentService.GetDocument(IdDocument);
                else
                    document = DocumentService.GetDocumentLatestVersion(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BiblosDS.Library.Common.Objects.Response.DocumentResponse GetDocumentsInfoByIdPaged(BindingList<Guid> idDocuments, int skip, int take)
        {
            try
            {
                logger.Debug("GetDocumentsInfoByIds - INIT");
                var ret = DocumentService.GetDocumentsPaged(idDocuments, skip, take);
                logger.DebugFormat("GetDocumentsInfoByIds - END. Something found? {0}", (ret != null && ret.TotalRecords > 0) ? "YES" : "NO");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #region CreateDocument
        /// <summary>
        /// Inserisce il padre di una Chain
        /// </summary>
        /// <param name="Document">Document da inserire</param>
        /// <returns>Document</returns>
        public Document InsertDocumentChain(Document Document)
        {
            try
            {
                logger.Debug("InsertDocumentChain");
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
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Confirm the Parent Chain
        /// </summary>
        /// <param name="IdParent"></param>
        /// <returns></returns>
        public bool ConfirmChain(Guid IdParent)
        {
            try
            {
                logger.DebugFormat("ConfirmChain IdParent:{0}", IdParent);
                //Document document = DocumentService.GetDocument(IdParent);
                //document.IsConfirmed = true;
                //DateTime? mainDate = null;
                //var attributesValues = AttributeService.GetAttributeValues(IdParent);
                //document.PrimaryKeyValue = AttributeService.ParseAttributeValues(document.Archive, attributesValues, out mainDate);

                //if (!string.IsNullOrEmpty(document.PrimaryKeyValue))
                //{
                //    if (!DocumentService.CheckPrimaryKey(document.IdDocument, document.Archive.IdArchive, document.PrimaryKeyValue))
                //        throw new BiblosDS.Library.Common.Exceptions.DocumentPrimaryKey_Exception();
                //}

                //DocumentService.UpdateDocument(document);
                DocumentService.ConfirmDocument(IdParent);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }

        }

        public Guid CreateDocumentChain(string archiveName, BindingList<DocumentAttributeValue> attributeValues)
        {
            try
            {
                logger.DebugFormat("CreateDocumentChain {0}", archiveName);
                var archive = ArchiveService.GetArchiveByName(archiveName);
                if (archive == null)
                    throw new Archive_Exception();
                var document = new Document { Archive = archive, AttributeValues = attributeValues };
                document = InsertDocumentChain(document);
                logger.DebugFormat("CreateDocumentChain IdDocument: {0}", document.IdDocument);
                return document.IdDocument;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public bool ConfirmDocument(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("ConfirmDocument {0}", idDocument);
                var document = DocumentService.GetDocument(idDocument);
                if (document == null)
                    throw new DocumentNotFound_Exception();
                document.IsConfirmed = true;
                DocumentService.UpdateDocument(document);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document AddDocumentChainWithWorkflow(Document document, Guid? idParent, DocumentContentFormat inputFormat, string uriWorkflow)
        {
            throw new NotImplementedException();
        }


        public DocumentServer AddDocumentToMaster(Document document)
        {
            try
            {
                logger.DebugFormat("AddDocumentToMaster Document:{0} - {1}", document == null ? "Documento nullo" : document.Name, document == null ? "Documento nullo" : document.IdDocument.ToString());
                var resultDocumentServer = DocumentService.AddDocumentToMaster(document);
                return resultDocumentServer;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                        "AddDocumentToMaster",
                                        ex.ToString(),
                                        LoggingOperationType.BiblosDS_InsertDocument,
                                        LoggingLevel.BiblosDS_Errors);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Inserisce un documento, clonandone il blob da uno già presente.
        /// </summary>
        /// <param name="document">Documento da inserire.</param>
        /// <param name="idParent">Padre catena di destinazione, se nullo crea una nuova catena.</param>
        /// <param name="inputFormat">Formato di input.</param>
        /// <param name="cloneable">Documento di cui clonare il blob.</param>
        /// <returns></returns>
        public Document CloneDocumentToChain(Document document, Guid? idParent, DocumentContentFormat inputFormat, Document cloneable)
        {
            // Verifico che estensione e contenuto siano coerenti.
            if (!Path.GetExtension(document.Name)
                .Equals(Path.GetExtension(cloneable.Name), StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("l'estensione non è compatibile con il blob da clonare.");

            byte[] blob = CacheService.GetFromChache(cloneable, cloneable.Name, string.Empty);
            if (blob == null)
            {
                using (var client = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    blob = (client as IServiceDocumentStorage).GetDocument(cloneable).Content.Blob;

                try
                {
                    CacheService.AddCache(cloneable, new DocumentContent(blob), cloneable.Name, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.InfoFormat("Error on load document in cache: {0} {1}", Environment.NewLine, ex.ToString());
                }
            }

            // Completo il documento da inserire con il blob recuperato dal documento da clonare.
            document.Content = new DocumentContent(blob);
            return AddDocumentToChain(document, idParent, inputFormat);
        }

        /// <summary>
        /// Collega Document a Chain se specificata, altrimenti ne crea una
        /// </summary>
        /// <param name="Document">Document da inserire</param>
        /// <param name="IdParent">Chain di appartenenza</param>
        /// <returns>Document</returns>
        public Document AddDocumentToChain(Document document, Guid? idParent, DocumentContentFormat inputFormat)
        {
            try
            {
                logger.DebugFormat("AddDocumentToChain Document:{0}", document == null ? "Documento nullo" : document.Name);
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                    Identity(),
                    "AddDocumentToChain",
                    string.Empty,
                     LoggingOperationType.BiblosDS_InsertDocument,
                      LoggingLevel.BiblosDS_Trace,
                      document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                      null,
                      idParent);
                if (document.Archive == null || document.Archive.IdArchive == Guid.Empty)
                    throw new Archive_Exception("Nessun archivio selezionato. Impossibile continuare");
                document.IsConfirmed = document.Archive.TransitoEnabled;
                Document resultDocument = DocumentService.AddDocumentToChain(document, idParent);
                if (document.Content != null && document.Content.Blob != null && document.Content.Blob.Length > 0)
                {
                    if (!document.Archive.TransitoEnabled)
                    {
                        logger.DebugFormat("Call AddDocument Transito DISABLED Init {0}", document.IdDocument);
                        using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                        {
                            (clientChannel as IServiceDocumentStorage).AddDocument(resultDocument);
                        }
                        DocumentService.ConfirmDocument(document.IdDocument);
                        logger.DebugFormat("Call AddDocument End");
                    }
                }
                return resultDocument;
                #region Old Code
                ////
                //if ((Document.Content.Blob == null || Document.Content.Blob.Length <= 0))
                //    throw new Exception("Nessun file passato in DocumentContent. Impossibile continuare");
                ////
                //if (string.IsNullOrEmpty(Document.Name))
                //    throw new Exception("Nessun nome file impostato. Impossibile continuare");
                //if (Document.Archive == null || Document.Archive.IdArchive == Guid.Empty)
                //    throw new Exception("Nessun archivio selezionato. Impossibile continuare");
                ////
                //BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);                
                ////Check dei metadati
                ////Viene assunto che l'Archive non possa essere modificato in modifica del documento
                ////questo vincolo mi garantisce che lo stesso documento sia memorizzato nello stesso storage per tutte le versioni
                //BindingList<Exception> exceptions = AttributeService.CheckMetaData(Document, Document.Archive, Document.AttributeValues, attributes, false);
                //if (exceptions.Count > 0)
                //{
                //    StringBuilder fault = new StringBuilder();
                //    foreach (var item in exceptions)
                //    {
                //        fault.Append(item.Message + Environment.NewLine);
                //    }
                //    throw new FaultException(fault.ToString());
                //}
                //foreach (var item in Document.AttributeValues)
                //{
                //    item.Attribute = attributes.Where(x => x.IdAttribute == item.Attribute.IdAttribute).FirstOrDefault();
                //}
                ////
                //DateTime? mainDate = null;
                //Document.PrimaryKey = AttributeService.ParseAttributeValues(Document.AttributeValues, attributes, out mainDate);
                //if (!string.IsNullOrEmpty(Document.PrimaryKey))
                //{
                //    if (!DocumentService.CheckPrimaryKey(Document.IdDocument, Document.Archive.IdArchive, Document.PrimaryKey))
                //        throw new BiblosDS.Library.Common.Exceptions.DocumentPrimaryKey_Exception();
                //}
                ////If the parent exist select the parent chain...if not exist create the chain
                //Document ParentDocument = null;
                //if (!IdParent.HasValue || IdParent.Value == Guid.Empty)
                //{
                //    ParentDocument = new Document();
                //    ParentDocument.IdDocument = Guid.NewGuid();
                //    ParentDocument.IsVisible = true;
                //    ParentDocument.Status = new Status(DocumentStatus.Undefined);
                //    ParentDocument.Archive = Document.Archive;
                //    //Save
                //    //ParentDocument = DocumentService.AddDocument(ParentDocument);
                //}
                //else
                //{
                //    ParentDocument = DocumentService.GetDocument((Guid)IdParent);
                //    if (ParentDocument == null)
                //        throw new Exception("Parent non valido");
                //}
                //Document.DocumentParent = ParentDocument;                
                ////Se non vienen specificato nessun permesso viene aggiunto il diritto 
                ////full control al gruppo che ha caricato il documento
                //if (Document.Permissions == null || Document.Permissions.Count <= 0)
                //{                                    
                //    Document.Permissions = new BindingList<DocumentPermission>();
                //    foreach (var item in ServiceSecurityContext.Current.WindowsIdentity.Groups)
                //    {
                //        Document.Permissions.Add(new DocumentPermission()
                //            {
                //                Name = item.Translate(typeof(NTAccount)).Value,
                //                Mode = DocumentPermissionMode.FullControl,
                //                IsGroup = true
                //            });
                //    }
                //}
                ////
                //Document.DateMain = mainDate;
                ////AttributeService.GetAttribute
                ////Set the new Id of the new document.
                //Document.IdDocument = Guid.NewGuid();
                ////Force the visibility of the document.
                //Document.IsVisible = true;
                ////Set the version of the document.
                //Document.Version = 1;       
                ////
                //Document.DateCreated = DateTime.Now;
                ////Save the document to Transito Path
                //FileService.SaveFileToTransitoLocalPath(Document.IdDocument + Path.GetExtension(Document.Name), Document.Content.Blob);               
                ////TODO if the insert is aborted delete the Transito Path file. 
                ////Set the status
                //Document.Status = new Status((short)DocumentStatus.InTransito);
                ////                
                ////Add the docuement and the attibutes value into the DB
                //DocumentService.AddDocument(Document);
                ////Call the storage interface            
                ////using(ServiceReferenceStorage.ServiceDocumentStorageClient client = new BiblosDS.WCF.Documents.ServiceReferenceStorage.ServiceDocumentStorageClient())
                ////{                                 
                ////    client.AddDocument(Document);
                ////}
                //ServiceReferenceStorage.ServiceDocumentStorageClient client = new BiblosDS.WCF.Documents.ServiceReferenceStorage.ServiceDocumentStorageClient();
                //    //Uncommented to add the document Async
                //client.AddDocumentAsync(Document);
                //    //client.BeginAddDocument(
                //    //    Document, 
                //    //    new AsyncCallback(delegate (IAsyncResult aSyncResult)
                //    //    {
                //    //        try
                //    //        {
                //    //            ServiceReferenceStorage.ServiceDocumentStorageClient result = (ServiceReferenceStorage.ServiceDocumentStorageClient)aSyncResult.AsyncState;
                //    //            Guid DocumentAddedId = result.EndAddDocument(aSyncResult);
                //    //            result.Close();
                //    //            //TODO Write the result in the LOG table
                //    //        }
                //    //        catch (Exception)
                //    //        {
                //    //            //TODO Write the ERROR in the LOG table                
                //    //        }
                //    //    }),
                //    //    client);
                //return Document; 
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                        "AddDocumentToChain",
                                        ex.ToString(),
                                        LoggingOperationType.BiblosDS_InsertDocument,
                                        LoggingLevel.BiblosDS_Errors);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Check the metadata for the insert
        /// //TODO Chiedere se testare per la modifica o rendere configurabile
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public bool CheckMetaData(Document Document)
        {
            try
            {
                logger.DebugFormat("CheckMetaData Document:{0}", Document == null ? "Documento Nullo" : Document.IdDocument.ToString());
                BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
                if (AttributeService.CheckMetaData(Document, Document.Archive, Document.AttributeValues, attributes, false).Count <= 0)
                {
                    DateTime? mainDate = null;
                    Document.PrimaryKeyValue = AttributeService.ParseAttributeValues(Document.AttributeValues, attributes, out mainDate);
                    if (!string.IsNullOrEmpty(Document.PrimaryKeyValue))
                    {
                        if (!DocumentService.CheckPrimaryKey(Document.IdDocument, Document.Archive.IdArchive, Document.PrimaryKeyValue))
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }

        }

        public BindingList<Document> GetChainInfo(Guid idChain, decimal? version, bool? lastVersion)
        {
            try
            {
                logger.DebugFormat("GetChainInfo IdDocument:{0}, Versione:{1}, lastVersion:{2}", idChain, version, lastVersion);
                BindingList<Document> documents = DocumentService.GetChainDocuments(idChain);
                foreach (Document item in documents)
                {
                    item.AttributeValues = AttributeService.GetAttributeValues(item.IdDocument);
                }
                return documents;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// List of document of a Chain
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns>
        /// Return the Chain with the last version of all the document
        /// </returns>
        public BindingList<Document> GetChainInfoById(Guid IdDocument)
        {
            try
            {
                logger.DebugFormat("GetChainInfo IdDocument:{0}", IdDocument);
                BindingList<Document> documents = DocumentService.GetChainDocuments(IdDocument);
                foreach (Document item in documents)
                {
                    item.AttributeValues = AttributeService.GetAttributeValues(item.IdDocument);
                }
                return documents;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Return all the document filtered for 
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <param name="IsVisible"></param>
        /// <param name="Version"></param>
        /// <returns></returns>
        public BindingList<Document> GetChainInfoDetails(Guid idDocument, bool? isVisible, decimal? version, bool? lastVersion)
        {
            try
            {
                logger.DebugFormat("GetChainInfoDetails IdDocument:{0}, IsVisible:{1}, Version:{2}", idDocument, isVisible, version);
                bool visible = true;
                if (isVisible != null)
                    visible = (bool)isVisible;
                BindingList<Document> documents = null;
                documents = DocumentService.GetChainHistoryDocuments(idDocument);
                //            
                var query = from m in documents
                            where m.IsVisible == visible
                            &&
                            ((version != null && (decimal)version > 0 && m.Version == version)
                            || version == null
                            || version.Value <= 0)
                            select m;

                documents = new BindingList<Document>(query.ToList());
                foreach (Document item in documents)
                {
                    item.AttributeValues = AttributeService.GetAttributeValues(item.IdDocument);
                }
                return documents;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Attribute associated to a Archive
        /// </summary>
        /// <param name="IdArchive"></param>
        /// <returns>
        /// BindingList of <see cref="DocumentAttribute">DocumentAttribute</see> with the signature
        /// of the Attribute
        /// </returns>
        public BindingList<DocumentAttribute> GetMetadataStructure(Guid IdArchive)
        {
            try
            {
                return AttributeService.GetAttributesFromArchive(IdArchive);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<DocumentArchive> GetArchives()
        {
            return ArchiveService.GetArchives();
        }

        #endregion

        #region Lock

        public DocumentContent CheckOutDocumentContent(Guid idDocument, string userId, DocumentContentFormat outputFormat)
        {
            try
            {
                logger.DebugFormat("CheckOutDocumentContent {0} {1}", idDocument, userId);
                Document document = DocumentCheckOut(idDocument, true, userId);
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    return (clientChannel as IServiceDocumentStorage).GetDocument(document).Content;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document CheckOutDocument(Guid idDocument, string userId, DocumentContentFormat outputFormat, bool? returnContent)
        {
            try
            {
                logger.DebugFormat("CheckOutDocument {0} {1}", idDocument, userId);
                logger.DebugFormat("CheckOutDocument log di test");
                Document document = DocumentCheckOut(idDocument, true, userId);
                logger.DebugFormat("CheckOutDocument: document extract : {0}", document.IdDocument);
                if (returnContent.HasValue && returnContent.Value)
                {
                    logger.DebugFormat("CheckOutDocument: clientChannel to: {0}", ServerService.WCF_DocumentStorage_HostName);
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {
                        document.Content = (clientChannel as IServiceDocumentStorage).GetDocument(document).Content;
                    }
                }
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void UndoCheckOutDocument(Guid idDocument, string userId)
        {
            try
            {
                logger.DebugFormat("UndoCheckOutDocument {0} {1}", idDocument, userId);
                DocumentUndoCheckOut(idDocument, userId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Update the document
        /// Return the new IdDocument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="userId"></param>
        /// <param name="inputFormat">Not in use</param>
        /// <param name="version">Not in use</param>
        /// <returns></returns>
        public Document CheckInDocument(Document document, string userId, DocumentContentFormat inputFormat, decimal? version)
        {
            try
            {
                logger.DebugFormat("CheckInDocument {0} {1} {2} {3}", document.IdDocument, userId, document.IsConservated, document.Name);                
                document.IdDocument = DocumentCheckIn(document, userId);
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document DocumentCheckOut(Guid IdDocument, bool latestVersion, string UserId)
        {
            logger.DebugFormat("DocumentCheckOut {0} {1}", IdDocument, UserId);
            Document document = null;
            try
            {
                logger.DebugFormat("DocumentCheckOut: latestVersion -> {0}", latestVersion);
                if (latestVersion)
                    document = DocumentService.GetDocumentLatestVersion(IdDocument);
                else
                    document = DocumentService.GetDocument(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                if (document.IsCheckOut && !document.IdUserCheckOut.Equals(UserId))
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception();
                //Se il docuemtno è conservato allora non è modificabile.
                if (document.IsConservated != null && (bool)document.IsConservated)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentReadOnly_Exception();
                //Permission
                if (document.Archive.EnableSecurity)
                {
                    document.Permissions = DocumentService.GetDocumentPermissions(document.IdDocument);
                    var query = from p in document.Permissions
                                where (from g in ServiceSecurityContext.Current.WindowsIdentity.Groups
                                       select g.Translate(typeof(NTAccount)).Value).Contains(p.Name)
                                       && p.IsGroup == true
                                       && (p.Mode == DocumentPermissionMode.FullControl
                                       || p.Mode == DocumentPermissionMode.Write
                                       || p.Mode == DocumentPermissionMode.Modify)
                                select p;
                    //Check sul singolo utente
                    if (query.Count() <= 0)
                    {
                        var queryUser = from p in document.Permissions
                                        where p.Name == Identity()
                                               && p.IsGroup == false
                                               && (p.Mode == DocumentPermissionMode.FullControl
                                            || p.Mode == DocumentPermissionMode.Write
                                            || p.Mode == DocumentPermissionMode.Modify)
                                        select p;
                        if (queryUser.Count() <= 0)
                            throw new BiblosDS.Library.Common.Exceptions.Permission_Exception();
                    }
                }
                //Anche se la fase di verifica non è in transazione il checkout deve avvenire in transazione (Lettura e Modifica)
                DocumentService.CheckOut(document.IdDocument, UserId);
                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                return document;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "DocumentCheckOut",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "DocumentCheckOut",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void DocumentUndoCheckOut(Guid IdDocument, string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
                throw new Exception("Nessuna UserId specificata. Impossibile continuare.");
            Document document = null;
            try
            {
                document = DocumentService.GetDocument(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                if (document.IdUserCheckOut != null && !document.IdUserCheckOut.Equals(UserId, StringComparison.InvariantCultureIgnoreCase))
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception("File in Check-Out dall'utente:" + document.IdUserCheckOut);
                DocumentService.UndoCheckOut(IdDocument, UserId);
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "DocumentUndoCheckOut",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "DocumentUndoCheckOut",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }

        }

        /// <summary>
        /// Update the docuemtn 
        /// Return the new IdDocument
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public Guid DocumentCheckIn(Document Document, string UserId)
        {
            logger.InfoFormat("DocumentCheckIn Document:{0}, UserId:{1}", Document.IdDocument, UserId);
            try
            {
                //L'estrazione avvinene sempre sull'ultimo documento inserito Versione Max
                //la versione viene incrementata sulla versione corrente del documento
                Document localDocument = DocumentService.GetDocument(Document.IdDocument);
                //
                if (localDocument == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                if (!localDocument.IsCheckOut)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception(
                        "Documento non estratto. Impossibile continuare!");
                //
                if (!string.IsNullOrEmpty(localDocument.IdUserCheckOut) &&
                    !localDocument.IdUserCheckOut.Equals(UserId, StringComparison.InvariantCultureIgnoreCase))
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception(
                        "File in Check-Out dall'utente:" + localDocument.IdUserCheckOut);
                //
                if (string.IsNullOrEmpty(Document.Name) && ConfigurationManager.AppSettings["VerifyDocumentName"].ToStringExt() == "true")
                    throw new Exception("Nessun nome file impostato. Impossibile continuare");
                //The name of the file can't change?
                //Document.Name = localDocument.Name;
                Document.Archive = localDocument.Archive;
                Document.DocumentParent = localDocument.DocumentParent != null ? localDocument.DocumentParent : localDocument;
                Document.IsCheckOut = false;
                Document.IdUserCheckOut = null;
                //Set to rertrive the original id of the list of file version
                Document.DocumentParentVersion = localDocument.DocumentParentVersion == null
                                                     ? localDocument
                                                     : localDocument.DocumentParentVersion;
                //Check dei metadati
                BindingList<DocumentAttribute> attributes =
                    AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
                //Viene assunto che l'Archive non possa essere modificato in modifica del documento
                //questo vincolo mi garantisce che lo stesso documento sia memorizzato nello stesso storage per tutte le versioni
                BindingList<Exception> exceptions = AttributeService.CheckMetaData(Document, localDocument.Archive,
                                                                                   Document.AttributeValues, true);
                if (exceptions.Count > 0)
                {
                    StringBuilder fault = new StringBuilder();
                    foreach (var item in exceptions)
                    {
                        fault.Append(item.Message + Environment.NewLine);
                    }
                    throw new Attribute_Exception(fault.ToString());
                }
                //Assegnazione dell'Nuovo id al documento modificato
                Document.IdDocument = Guid.NewGuid();
                localDocument.IsCheckOut = false;
                localDocument.IdUserCheckOut = null;
                localDocument.IsLatestVersion = false;
                localDocument.PrimaryKeyValue = null;
                Document.IsLatestVersion = true;
                //Incremento alla versione del documento corrente (Valido solo se l'estrazione è sul documento corrente)
                Document.Version = DocumentService.IncrementVersion(localDocument.Version);
                //__________________
                DateTime? mainDate = null;
                Document.PrimaryKeyValue = AttributeService.ParseAttributeValues(Document.AttributeValues, attributes,
                                                                            out mainDate);
                //Se non vienen specificato nessun permesso viene aggiunto il diritto 
                //full control al gruppo che ha caricato il documento
                //TODO...
                //if (Document.Permissions == null || Document.Permissions.Count <= 0)
                //{
                //    Document.Permissions = new BindingList<DocumentPermission>();
                //    foreach (var item in ServiceSecurityContext.Current.WindowsIdentity.Groups)
                //    {
                //        Document.Permissions.Add(new DocumentPermission()
                //        {
                //            Name = item.Translate(typeof(NTAccount)).Value,
                //            Mode = DocumentPermissionMode.FullControl,
                //            IsGroup = true
                //        });
                //    }
                //}
                //
                Document.DateMain = mainDate;
                if ((Document.Content != null && Document.Content.Blob != null && Document.Content.Blob.Length > 0))
                {
                    Document.DocumentLink = null;
                }
                else
                    Document.DocumentLink = localDocument.DocumentLink == null ? localDocument : localDocument.DocumentLink;
                if (string.IsNullOrEmpty(Document.Name))
                    Document.Name = localDocument.Name;

                if (Document.Content != null && Document.Content.Blob != null && Document.Content.Blob.Length > 0 && Document.Storage != null && Document.Storage.EnableFulText)
                {
                    //Rimuovo eventuali informazioni della fulltext
                    RemoveFullTextDataForDocument(localDocument.IdDocument);
                }
                
                //-----------------------------
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
                    {
                        Document.Status = new Status((short)DocumentStatus.Undefined);
                    }
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
                DocumentService.SaveAtomic(new List<BiblosDSObject> { Document }, new List<BiblosDSObject> { localDocument });
                //if (Document.AttributeValues != null)
                //    foreach (var item in Document.AttributeValues)
                //    {
                //        if (localDocument.AttributeValues.Any(x => x.Attribute.IdAttribute == item.Attribute.IdAttribute))
                //        {
                //            if (localDocument.AttributeValues.First(x => x.Attribute.IdAttribute == item.Attribute.IdAttribute).Value != item.Value)
                //                DocumentService.UpdateDocumentAttributeValue(Document, item);
                //        }
                //        else
                //            DocumentService.UpdateDocumentAttributeValue(Document, item);
                //    }
                //Call the storage interface   
                if ((Document.Content != null && Document.Content.Blob != null && Document.Content.Blob.Length > 0))
                {
                    if (!Document.Archive.TransitoEnabled)
                    {
                        using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                        {

                            try
                            {
                                (clientChannel as IServiceDocumentStorage).AddDocument(Document);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                DocumentService.UndoLatestVersion(localDocument.IdDocument, Document.IdDocument);
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    if (Document.Storage != null && Document.StorageArea != null)
                        using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                        {
                            (clientChannel as IServiceDocumentStorage).RestoreAttribute(Document.IdDocument);
                        }
                }
                DocumentService.ConfirmDocument(Document.IdDocument);
                //
                return Document.IdDocument;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "DocumentUndoCheckOut",
                                                    Document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    Document.Archive != null ? Document.Archive.IdArchive : Guid.Empty,
                                                    Document.Storage.IdStorage,
                                                    Document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "DocumentUndoCheckOut",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion

        #region IServiceDocument Members

        /// <summary>
        /// ritorna la versione di BiblosDS installata 
        /// </summary>
        /// <returns></returns>
        public string BiblosDSVersion()
        {
            return "2013.1";
        }

        /// <summary>
        /// Verifica se il servizio è pronto per accettare richieste
        /// </summary>
        /// <returns>true se DocumentService si connette, false altrimenti</returns>
        public bool IsAlive()
        {
            try
            {

                //}
                //CloudDriveManager manager = new CloudDriveManager();
                //bool isMounted = false;
                //foreach (var item in manager.GetMountedDrive())
                //{
                //    logger.InfoFormat("GetMountedDrive: {0}-{1}", item.Key, item.Value);
                //    isMounted = true;
                //}
                //string DCACHE_NAME = "LocalDriveCache";
                //string DRIVE_SETTINGS = "BiblosDS.Cdm.DriveSettings";
                ///*if (!isMounted)
                //    CloudDriveManager.MountAllDrives(DRIVE_SETTINGS, DCACHE_NAME);
                // * */
                //string str = manager.GetMountedDrive().FirstOrDefault().Key;
                //string path = Path.Combine(str, "TestDocument");
                //if (!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                //File.WriteAllText(Path.Combine(path, "prova.txt"), "Test");

                return DocumentService.IsConnected();
            }
            catch (Exception e)
            {
                logger.Error(e);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                    "ServiceDocument.IsAlive",
                    "DocumentService.IsConnected report an error : " + e.Message,
                    LoggingOperationType.BiblosDS_GetAlive,
                    LoggingLevel.BiblosDS_Managed_Error);
                return false;
            }
        }

        public bool CheckIsReady(Document Document)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Move


        public void DocumentMoveToArchive(Guid IdDocument, DocumentArchive Archive, bool? ForceDelete)
        {
            Document document = null;
            try
            {
                document = DocumentService.GetDocument(IdDocument);
                document.AttributeValues = AttributeService.GetAttributeValues(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    document.Content = (clientChannel as IServiceDocumentStorage).GetDocument(document).Content;
                }
                document.Archive = Archive;
                BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Archive.IdArchive);
                BindingList<DocumentAttributeValue> attributeValues = new BindingList<DocumentAttributeValue>();
                DocumentAttributeValue attributeValue;
                foreach (DocumentAttribute item in attributes)
                {
                    if (document.AttributeValues.Where(x => x.Attribute.Name == item.Name).Count() == 1)
                    {
                        attributeValue = new DocumentAttributeValue();
                        attributeValue.Attribute = item;
                        attributeValue.Value = document.AttributeValues.Where(x => x.Attribute.Name == item.Name).Single().Value;
                        attributeValues.Add(attributeValue);
                    }
                }
                document.AttributeValues = attributeValues;
                if (ForceDelete != null && (bool)ForceDelete)
                {
                    //TODO DELETE DOCUMENT
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {
                        (clientChannel as IServiceDocumentStorage).DeleteDocument(document.IdDocument);
                    }
                    AddDocumentToChain(document, null, DocumentContentFormat.Binary);
                }
                else
                    AddDocumentToChain(document, document.DocumentParent.IdDocument, DocumentContentFormat.Binary);
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "DocumentMoveToArchive",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "DocumentMoveToArchive",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void DocumentMoveToStorage(Guid IdDocument, DocumentStorage Storage, bool? ForceDelete)
        {
            Document document = null;
            try
            {
                document = DocumentService.GetDocument(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                document.AttributeValues = AttributeService.GetAttributeValues(IdDocument);
                document.Permissions = DocumentService.GetDocumentPermissions(IdDocument);
                //
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    document.Content = (clientChannel as IServiceDocumentStorage).GetDocument(document).Content;
                }
                Guid idNewDocument = Guid.NewGuid();
                //
                if (ForceDelete.HasValue && ForceDelete.Value)
                {
                    //TODO write to a table the state of delete
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {
                        (clientChannel as IServiceDocumentStorage).DeleteDocument(IdDocument);
                    }
                }
                document.IsVisible = false;
                document.IsLinked = true;
                document.DocumentLink = new Document(idNewDocument);
                DocumentService.UpdateDocument(document);
                //
                document.Storage = Storage;
                //AttributeService.GetAttribute
                //Set the new Id of the new document.
                document.IdDocument = Guid.NewGuid();
                //Force the visibility of the document.
                document.IsVisible = true;
                document.IsLinked = false;
                document.DocumentLink = null;
                //Set the version of the document.
                document.Version = DocumentService.IncrementVersion(document.Version);
                //
                document.DateCreated = DateTime.Now;
                //Save the document to Transito Path
                FileService.SaveFileToTransitoLocalPath(document, document.Content.Blob);
                //TODO if the insert is aborted delete the Transito Path file. 
                //Set the status
                document.Status = new Status((short)DocumentStatus.InTransito);
                //
                DocumentService.AddDocument(document, null);
                //
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).AddDocument(document);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "DocumentMoveToStorage",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "DocumentMoveToStorage",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException(ex.Message);
            }
        }

        #endregion

        public void RestoreAttribute(Guid IdDocument)
        {
            try
            {
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).RestoreAttribute(IdDocument);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "RestoreAttribute",
                                                    null,
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    Guid.Empty,
                                                    Guid.Empty,
                                                    IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "RestoreAttribute",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void SignDocument(Guid IdDocument, DocumentContent SignerCert, DocumentContent SignedDigest, DocumentContent Content)
        {
            Document document = null;
            try
            {
                document = DocumentService.GetDocument(IdDocument);
                if (Content == null)
                {
                    using (ServiceDigitalSignClient client = new ServiceDigitalSignClient())
                    {
                        document.Content = client.AddRawSignature(document.Name, SignerCert, SignedDigest, document.Content);
                    }
                }
                else
                    document.Content = Content;
                document.Name = Path.GetFileNameWithoutExtension(document.Name) + ".p7m";
                AddDocumentToChain(document, document.DocumentParent.IdDocument, DocumentContentFormat.Binary);
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "SignDocument",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "SignDocument",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<DocumentSignInfo> GetDocumentSignInfo(Guid IdDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentSignInfo IdDocument:{0}", IdDocument);
                BindingList<DocumentSignInfo> response = new BindingList<DocumentSignInfo>();
                BindingList<Document> documents = DocumentService.GetDocumentChildren(IdDocument);
                if (documents.Count == 0)
                    return null;

                foreach (Document document in documents.Where(x => (x.IsDetached.HasValue && !x.IsDetached.Value) || (!x.IsDetached.HasValue)))
                {
                    DocumentContent content = GetDocumentContentById(document.IdDocument);
                    CompEdContentInfo contentInfo = new CompEdContentInfo(content.Blob);
                    if (contentInfo.HasSignatures)
                    {
                        contentInfo.Signatures.ToList().ForEach(f => response.Add(new DocumentSignInfo(f){
                            IdDocument = document.IdDocument,
                            DocumentName = document.Name
                        }));
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #region IServiceDocument Members


        public void UpdateDocument(Document Document)
        {
            throw new NotImplementedException();
        }

        public void UpdateDocumentName(Guid idDocument, string documentName)
        {
            try
            {
                DocumentService.UpdateDocumentName(new Document { IdDocument = idDocument, Name = documentName });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Guid DocumentAttributeCheckIn(Document Document, string UserId)
        {
            try
            {
                if ((Document == null || Document.AttributeValues == null || Document.AttributeValues.Count <= 0))
                    throw new Exception("Attenzione, mancano attributi da modificare o il documento è nullo. Impossibile continuare");
                //L'estrazione avvinene sempre sull'ultimo documento inserito Versione Max
                //la versione viene incrementata sulla versione corrente del documento
                Document localDocument = DocumentService.GetDocument(Document.IdDocument);
                //
                if (localDocument == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                if (!localDocument.IsCheckOut)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception("Documento non estratto. Impossibile continuare!");
                //
                if (string.IsNullOrEmpty(localDocument.IdUserCheckOut) || !localDocument.IdUserCheckOut.Equals(UserId, StringComparison.InvariantCultureIgnoreCase))
                    throw new BiblosDS.Library.Common.Exceptions.DocumentCheckOut_Exception("File in Check-Out dall'utente:" + localDocument.IdUserCheckOut);
                //
                //Assegnazione dell'Nuovo id al documento modificato
                Document.IdDocument = Guid.NewGuid();
                Document.Name = localDocument.Name;
                Document.Archive = localDocument.Archive;
                Document.StorageArea = localDocument.StorageArea;
                Document.Storage = localDocument.Storage;
                Document.Status = new Status(DocumentStatus.Undefined);
                //Link al documento originale (Blob del file)
                //Gianni 27-01-2010 Link sempre al documento originale per evitare di ricercare il content a cascata
                Document.DocumentLink = new Document(localDocument.DocumentLink == null ? localDocument.IdDocument : localDocument.DocumentLink.IdDocument);
                //Check dei metadati
                BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
                //Viene assunto che l'Archive non possa essere modificato in modifica del documento
                //questo vincolo mi garantisce che lo stesso documento sia memorizzato nello stesso storage per tutte le versioni
                BindingList<Exception> exceptions = AttributeService.CheckMetaData(Document, Document.Archive, Document.AttributeValues, true);
                if (exceptions.Count > 0)
                {
                    StringBuilder fault = new StringBuilder();
                    foreach (var item in exceptions)
                    {
                        fault.Append(item.Message + Environment.NewLine);
                    }
                    throw new FaultException(fault.ToString());
                }
                localDocument.IsLinked = true;
                localDocument.IsCheckOut = false;
                localDocument.IdUserCheckOut = string.Empty;
                //Incremento alla versione del documento corrente (Valido solo se l'estrazione è sul documento corrente)
                Document.Version = DocumentService.IncrementVersion(localDocument.Version);
                //__________________
                DateTime? mainDate = null;
                BindingList<DocumentAttributeValue> documentAttributeValues = Document.AttributeValues;
                if (Document.DocumentParent != null)
                {
                    BindingList<DocumentAttributeValue> parentAttributeValues = AttributeService.GetAttributeValues(Document.DocumentParent.IdDocument);
                    documentAttributeValues = new BindingList<DocumentAttributeValue>(documentAttributeValues.Union(parentAttributeValues.Where(x => !documentAttributeValues.Any(xx => xx.IdAttribute == x.IdAttribute))).ToList());
                }
                Document.PrimaryKeyValue = AttributeService.ParseAttributeValues(documentAttributeValues, attributes, out mainDate);
                //Se non vienen specificato nessun permesso viene aggiunto il diritto 
                //full control al gruppo che ha caricato il documento
                if (Document.Permissions == null || Document.Permissions.Count <= 0)
                {
                    Document.Permissions = new BindingList<DocumentPermission>();
                    foreach (var item in ServiceSecurityContext.Current.WindowsIdentity.Groups)
                    {
                        Document.Permissions.Add(new DocumentPermission()
                        {
                            Name = item.Translate(typeof(NTAccount)).Value,
                            Mode = DocumentPermissionMode.FullControl,
                            IsGroup = true
                        });
                    }
                }
                //
                Document.DateMain = mainDate;
                //AttributeService.GetAttribute                
                //Force the visibility of the document.
                Document.IsVisible = true;
                //
                Document.DateCreated = DateTime.Now;
                //Save the document to Transito Path
                //Get the content to sign the Document

                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    Document.Content = (clientChannel as IServiceDocumentStorage).GetDocument(localDocument).Content;

                    //
                    //Calculate the document Sign Header
                    Document.SignHeader = AttributeService.GetAttributesHash(Document.AttributeValues);

                    //Calculate the document Hash                
                    Document.DocumentHash = DocumentService.GetDocumentHash(Document);

                    //Sign the document...
                    //firma e sign
                    //Document.Certificate = DocumentService.GetCertificateDefault();
                    //Document.FullSign = UtilityService.GetStringFromBob(
                    //    DocumentService.Sign(
                    //        Document.IdDocument,
                    //        Document.DocumentParent.IdDocument,
                    //        Document.Content,
                    //        Path.GetExtension(Document.Name),
                    //        DateTime.Now.ToString("yyyyMMdd"),
                    //        Document.AttributeValues,
                    //        Document.Certificate.Name));                
                    //Add the docuement and the attibutes value into the DB                
                    DocumentService.AddDocument(Document, null);
                    //Update the linked document             
                    DocumentService.UpdateDocument(localDocument);
                    (clientChannel as IServiceDocumentStorage).WriteAttribute(Document);
                }
                //
                return Document.IdDocument;
            }
            catch (Exception ex)
            {
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion

        #region DocumentMoveFromTransito


        public void DocumentMoveFromTransito(Guid IdDocument)
        {
            Document document = null;
            try
            {
                document = DocumentService.GetDocument(IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                document.Permissions = DocumentService.GetDocumentPermissions(IdDocument);
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).AddDocument(document);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.GetType().Namespace.Equals("BiblosDS.Library.Common.Exceptions"))
                        Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Documents,
                                                    Identity(),
                                                    "SignDocument",
                                                    document.IdDocument.ToString(),
                                                    LoggingOperationType.BiblosDS_General,
                                                    LoggingLevel.BiblosDS_Trace,
                                                    document.Archive != null ? document.Archive.IdArchive : Guid.Empty,
                                                    document.Storage.IdStorage,
                                                    document.DocumentParent.IdDocument);
                    else
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Documents,
                                            "SignDocument",
                                            ex.ToString(),
                                            LoggingOperationType.BiblosDS_InsertDocument,
                                            LoggingLevel.BiblosDS_Errors);
                }
                catch { /*Ignore*/}
                throw new FaultException(ex.Message);
            }
        }

        #endregion

        string Identity()
        {
            string identity = "";
            if (OperationContext.Current != null && OperationContext.Current.ServiceSecurityContext != null && OperationContext.Current.ServiceSecurityContext.PrimaryIdentity != null)
                identity = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
            return identity;
        }

        public void SetVisibleChain(Guid idChain, bool isVisible)
        {
            try
            {
                logger.DebugFormat("SetVisibleChain idChain:{0} isVisible:{1}", idChain, isVisible);
                DocumentService.DeleteChain(idChain, isVisible);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void SetVisibleDocument(Guid idDocument, bool isVisible)
        {
            try
            {
                logger.DebugFormat("SetVisibleChain SetVisibleDocument:{0} isVisible:{1}", idDocument, isVisible);
                DocumentService.DeleteDocument(idDocument, isVisible);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        public Document GetDocumentId(Guid idArchive, int idBiblos)
        {
            try
            {
                logger.DebugFormat("GetDocumentId idArchive:{0} idBiblos:{1}", idArchive, idBiblos);
                var document = DocumentService.GetDocument(idBiblos, idArchive);
                if (document == null)
                    throw new DocumentNotFound_Exception("Impossibile recuperare un documento con IdBiblos:" + idBiblos + " nell'archivio:" + idArchive);
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Ritorna se il documento è firmato digitalmente
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        /// <remarks>supporta anche PAdES</remarks>
        public bool IsDocumentSigned(Guid idDocument)
        {
            bool result = false;
            var content = GetDocumentContentById(idDocument);
            using (ServiceDigitalSignClient client = new ServiceDigitalSignClient())
            {
                result = client.IsBlobSigned(content);
            }

            return result;
        }

        /// <summary>
        /// Ritorna l'elenco completo dei certificati di firma del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        /// <remarks>supporta anche PAdES</remarks>
        public BindingList<DocumentCertificate> GetDocumentSigned(Guid idDocument)
        {
            logger.DebugFormat("GetDocumentSigned idDocument:{0}", idDocument);

            BindingList<DocumentCertificate> result = null;
            var content = GetDocumentContentById(idDocument);
            DocumentCertificate firstCertificate = null;
            using (ServiceDigitalSignClient client = new ServiceDigitalSignClient())
            {
                result = client.GetAllExpireDates(out firstCertificate, content.Description, content);
            }
            logger.DebugFormat("GetDocumentSigned - ritorno al chiamante");
            return result;
        }

        public bool AddDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            try
            {
                logger.DebugFormat("AddDocumentLink IdDocument:{0} IdDocumentLink:{1}", IdDocument, IdDocumentLink);

                bool res = DocumentService.AddDocumentLink(IdDocument, IdDocumentLink);

                logger.DebugFormat("AddDocumentLink - ritorno al chiamante");
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public int DetachDocument(string archive, int chain, int Enum)
        {
            try
            {
                logger.DebugFormat("DetachDocument archive:{0} Chain:{1} Enum:{2}", archive, chain, Enum);
                List<Document> documentsToProcess = new List<Document>();

                Document document = DocumentService.GetDocument(chain, ArchiveService.GetArchiveByName(archive).IdArchive);

                if (Enum >= 0)
                {
                    BindingList<Document> documents = DocumentService.GetChainDocuments(document.IdDocument);
                    //TODO se ho la catena ma la catena non ha nessun documento vado a cercare nel vecchio?
                    if (documents == null)
                        throw new Exception("Documento non trovato");

                    documents = new BindingList<Document>(documents.OrderBy(x => x.ChainOrder).ToList());

                    if (Enum >= documents.Count)
                        throw new Exception("Enum non comprese nell'intervallo dei documenti.");

                    documentsToProcess.Add(documents[Enum]);
                }
                else
                    documentsToProcess.Add(document);

                foreach (var item in documentsToProcess)
                {
                    RemoveFullTextDataForChain(item.IdDocument);
                    DocumentService.DetachDocument(item);                    
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
            return 0;
        }

        /// <summary>
        /// Permette di eseguire il detach di un documento specifico
        /// </summary>
        /// <param name="document">Documento di cui eseguire detach</param>
        /// <returns>Documento aggiornato</returns>
        public Document DocumentDetach(Document document)
        {
            try
            {
                logger.InfoFormat("DetachDocument -> IdDocument: {0}", document.IdDocument);
                //TODO: implementare validation
                Document dbDocument = DocumentService.GetDocument(document.IdDocument);
                if(dbDocument == null)
                {
                    throw new DocumentNotFound_Exception(string.Format("Document with IdDocument {0} not found", document.IdDocument));
                }

                //Si tratta di una chain, eseguo il detach di tutti i figli
                if(dbDocument.DocumentParent == null || (dbDocument.DocumentParentVersion != null && dbDocument.DocumentParentVersion.IdDocument == dbDocument.DocumentParent.IdDocument))
                {
                    Guid idChain = dbDocument.DocumentParentVersion == null ? dbDocument.IdDocument : dbDocument.DocumentParentVersion.IdDocument;
                    BindingList<Document> chainChildren = DocumentService.GetDocumentChildren(idChain);
                    RemoveFullTextDataForChain(idChain);
                    DocumentService.DetachDocuments(chainChildren);                    
                }
                else
                {
                    if (dbDocument.Storage != null && dbDocument.Storage.EnableFulText)
                    {
                        RemoveFullTextDataForDocument(dbDocument.IdDocument);
                    }
                    DocumentService.DetachDocument(dbDocument);                    
                }                
                return dbDocument;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("DetachDocument -> Error document with IdDocument {0}", document.IdDocument), ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<Document> GetDocumentLinks(Guid IdDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentLinks IdDocument:{0}", IdDocument);

                var res = DocumentService.GetDocumentLink(IdDocument);

                logger.DebugFormat("GetDocumentLinks - ritorno al chiamante -  return " + res.Count());

                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public bool DeleteDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            try
            {
                logger.DebugFormat("DeleteDocumentLink IdDocument:{0} IdDocumentLink:{1}", IdDocument, IdDocumentLink);

                var ret = DocumentService.DeleteDocumentLink(IdDocument, IdDocumentLink);

                logger.DebugFormat("DeleteDocumentLink - ritorno al chiamante");
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public DocumentAttach AddDocumentAttach(Guid idDocument, DocumentAttach attach)
        {
            //TODO Inserire nella tabella Attach            
            //Archive è quello del documento
            //Storage è quello del documento
            //Storage Area è quello del documento
            //Chiamare ServiceDocuementStorage AddDocumentAttach
            try
            {
                logger.DebugFormat("AddDocumentAttach IdDocument: {0} Nome Allegato: {1} Lunghezza File: {2} bytes", idDocument, (attach != null) ? attach.Name : "N/A", (attach != null && attach.Content != null && attach.Content.Blob != null) ? attach.Content.Blob.Length.ToString() : "0");

                if (attach.Document == null)
                    attach.Document = new Document(idDocument);

                attach.IdDocument = idDocument;

                var documentAttach = DocumentService.AddDocumentAttach(attach);

                if (!documentAttach.Document.Archive.TransitoEnabled)
                {
                    documentAttach.Content = attach.Content;
                    /*
                     * Si collega al servizio WCF dello storage.
                     * Verrà usato questo servizio perchè l'allegato è ovviamente da cacciare su storage.
                     */
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {
                        documentAttach.IdDocument = (clientChannel as IServiceDocumentStorage).AddAttachToDocument(documentAttach);
                    }
                }

                return documentAttach;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        public BindingList<DocumentAttach> GetDocumentAttachs(Guid IdDocument)
        {
            try
            {
                return DocumentService.GetDocumentAttachesFromDocument(IdDocument);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public DocumentContent GetDocumentAttachContent(Guid IdDocumentAttach)
        {
            try
            {
                logger.DebugFormat("GetDocumentAttachContent IdDocumentAttach:{0}", IdDocumentAttach);
                DocumentContent content = null;
                DocumentAttach document = DocumentService.GetDocumentAttach(IdDocumentAttach);
                //Set dell'id document per riutilizzare i metodi fi get document.
                //Ale: EEEEEH???
                //document.IdDocument = document.IdDocumentAttach;

                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                //                
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    content = (clientChannel as IServiceDocumentStorage).GetDocumentAttach(document).Content;
                    content.Description = document.Name;
                }
                //                               
                return content;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public void SetVisibleDocumentAttach(Guid idDocumentAttach, bool visible)
        {
            try
            {
                logger.DebugFormat("SetVisibleDocumentAttach SetVisibleDocument:{0} isVisible:{1}", idDocumentAttach, visible);
                DocumentService.DeleteDocumentAttach(idDocumentAttach, visible);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public bool ConfirmDocumentAttach(Guid IdDocumentAttach)
        {
            try
            {
                logger.DebugFormat("ConfirmDocumentAttach IdDocumentAttach:{0}", IdDocumentAttach);
                DocumentAttach document = DocumentService.GetDocumentAttach(IdDocumentAttach);

                if (document == null)
                    throw new DocumentNotFound_Exception("ConfirmDocumentAttach : no attachments with ID " + IdDocumentAttach);

                document.IsConfirmed = true;
                DocumentService.UpdateDocumentAttach(document);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<DocumentAttribute> GetAttributesDefinition(string archiveName)
        {
            try
            {
                logger.DebugFormat("GetAttributesDefinition {0}", archiveName);
                var archive = ArchiveService.GetArchiveByName(archiveName);
                if (archive == null)
                    throw new Archive_Exception();
                return AttributeService.GetAttributesFromArchive(archive.IdArchive);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public bool CheckIntegrity(Document document, bool? forceSign)
        {
            try
            {
                bool result = false;
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    result = (clientChannel as IServiceDocumentStorage).CheckIntegrity(document);
                    if (!result && (forceSign.HasValue && forceSign.Value))
                        (clientChannel as IServiceDocumentStorage).RestoreAttribute(document.IdDocument);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<DocumentStorage> GetStorages()
        {
            return StorageService.GetStorages();
        }

        public BindingList<DocumentStorageArea> GetStorageAreas(DocumentStorage storage)
        {
            return StorageService.GetStorageAreaFromStorage(storage.IdStorage);
        }

        public Guid GetDocumentId(string archiveName, int idBiblos)
        {
            try
            {
                logger.DebugFormat("GetDocumentId archiveName:{0} idBiblos:{1}", archiveName, idBiblos);
                var archive = ArchiveService.GetArchiveByName(archiveName);
                if (archive == null)
                    throw new Archive_Exception();
                var document = DocumentService.GetDocument(idBiblos, archive.IdArchive);
                if (document == null)
                    throw new DocumentNotFound_Exception("Impossibile recuperare un documento con IdBiblos:" + idBiblos + " nell'archivio:" + archiveName);
                return document.IdDocument;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document GetDocumentInServer(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentInServers idDocument:{0}", idDocument);
                return DocumentService.GetDocumentWithServerDetails(idDocument);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #region  Statistiche archivio

        public ArchiveStatistics GetArchiveStatistics(Guid idArchive)
        {
            try
            {
                logger.DebugFormat("GetArchiveStatistics idArchive:{0}", idArchive);

                var retval = ArchiveService.GetArchiveStatistics(idArchive);

                logger.Debug("GetArchiveStatistics - END");

                return retval;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion


        public Document AddDocumentToChainPDFCrypted(Document document, Guid? idParent, DocumentContentFormat inputFormat)
        {
            try
            {
                logger.DebugFormat("AddDocumentToChainPDFCrypted idDocument:{0}", document.IdDocument);
                if (document.Name.Substring(document.Name.Length - 3) != "pdf")
                    new BiblosDS.Library.Common.Exceptions.DocumentDateNotValid_Exception("Documento non in formato PDF.");
                if (document.Archive == null)
                    new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archivio non definito.");
                var archive = ArchiveService.GetArchive(document.Archive.IdArchive);
                if (archive == null)
                    new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archivio non definito.");
                var archiveCertificate = ArchiveService.GetArchiveCertificate(document.Archive.IdArchive);
                //                
                PdfReader reader = new PdfReader(document.Content.Blob);
                var outCriptedPdfStream = new MemoryStream();
                PdfStamper stamper = new PdfStamper(reader, outCriptedPdfStream);


                Pkcs12Store store = new Pkcs12StoreBuilder().Build();
                store.Load(new MemoryStream(archiveCertificate.CertificateBlob), archiveCertificate.Pin.ToCharArray());
                X509CertificateEntry thisEntry = store.GetCertificate(archiveCertificate.UserName + " Certificate");

                stamper.SetEncryption(new X509Certificate[] { thisEntry.Certificate }, new int[] { PdfWriter.ALLOW_PRINTING }, PdfWriter.ENCRYPTION_AES_128);

                reader.Close();
                stamper.Close();
                document.Content.Blob = outCriptedPdfStream.ToArray();
                document.IdArchiveCertificate = archiveCertificate.IdArchiveCertificate;
                return AddDocumentToChain(document, idParent, inputFormat);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #region [ Refactoring ]

        /// <summary>
        /// Recupera l'ultima versione del documento.
        /// </summary>
        /// <param name="idDocument">Id del documento da recuperare.</param>
        /// <returns></returns>
        public Document GetDocumentLatestVersion(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentLatestVersion - idDocument: {0}", idDocument);
                return DocumentService.GetDocumentLatestVersion(idDocument);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Recupera tutti i documenti all'ultima versione presenti nella catena.
        /// </summary>
        /// <param name="idParent">Id della catena di cui recuperare i documenti.</param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentChildren(Guid idParent)
        {
            try
            {
                logger.DebugFormat("GetDocumentChildren - idParent: {0}", idParent);
                return DocumentService.GetDocumentChildren(idParent);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion


        #region DocumentUnit Methods

        /// <summary>
        /// Ricerca DocumentUnit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>DocumentUnit</returns>
        public DocumentUnit UdsGetDocumentUnit(Guid idDocumentUnit)
        {
            try
            {
                logger.DebugFormat("UdsGetDocumentUnit idDocumentUnit:{0}", idDocumentUnit);
                var res = DocumentService.UdsGetDocumentUnit(idDocumentUnit);
                logger.DebugFormat("UdsGetDocumentUnit - END");
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca DocumentUnit - Ritorno ReadOnly e Preservate
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>DocumentUnit</returns>
        public DocumentUnitExt UdsGetDocumentUnitExt(Guid idDocumentUnit)
        {
            try
            {
                logger.DebugFormat("UdsGetDocumentUnitExt idDocumentUnit:{0}", idDocumentUnit);
                var res = DocumentService.UdsGetDocumentUnitExt(idDocumentUnit);
                logger.DebugFormat("UdsGetDocumentUnitExt - END");
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca documentUnit per fascicolo
        /// </summary>
        /// <param name="uriFascicle">Fascicolo</param>
        /// <returns>Elenco delle document unit per fascicolo</returns>
        public BindingList<DocumentUnit> UdsGetDocumentUnitsByFascicle(string uriFascicle)
        {
            try
            {
                logger.DebugFormat("UdsGetDocumentUnitsByFascicle uriFascicle:{0}", uriFascicle);
                var res = DocumentService.UdsGetDocumentUnitsByFascicle(uriFascicle);
                logger.DebugFormat("UdsGetDocumentUnitsByFascicle - ritorno al chiamante -  return " + res.Count());
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca catena Guid dei documenti legati alla document unit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>Catena ID documenti</returns>
        public BindingList<DocumentUnitChain> UdsGetUnitChain(Guid idDocumentUnit)
        {
            try
            {
                logger.DebugFormat("UdsGetUnitChain idDocumentUnit:{0}", idDocumentUnit);
                var res = DocumentService.UdsGetUnitChain(idDocumentUnit);
                logger.DebugFormat("UdsGetUnitChain - ritorno al chiamante -  return " + res.Count());
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca i documenti per DocumentUnit
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>Ritorna lista dei documenti</returns>
        public BindingList<Document> UdsGetUnitDocuments(Guid idDocumentUnit)
        {
            try
            {
                logger.DebugFormat("UdsGetUnitDocuments idDocumentUnit:{0}", idDocumentUnit);
                var res = DocumentService.UdsGetUnitDocuments(idDocumentUnit);
                logger.DebugFormat("UdsGetUnitDocuments - ritorno al chiamante -  return " + res.Count());
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca i documenti per fascicolo
        /// </summary>
        /// <param name="uriFascicle">Fascicolo</param>
        /// <returns>Ritorna lista dei documenti</returns>
        public BindingList<Document> UdsGetUnitDocumentsByFascicle(string uriFascicle)
        {
            try
            {
                logger.DebugFormat("UdsGetUnitDocumentsByFascicle uriFascicle:{0}", uriFascicle);
                var res = DocumentService.UdsGetUnitDocumentsByFascicle(uriFascicle);
                logger.DebugFormat("UdsGetUnitDocumentsByFascicle - ritorno al chiamante -  return " + res.Count());
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Apre a modifiche una DocumentUnit e le relative Aggregate a meno che una queste non sia già stata Preservata
        /// </summary>
        /// <param name="IdDocumentUnit">Chiave DocumentUnit</param>
        /// <returns>DocumentUnit</returns>
        public DocumentUnit UdsOpenDocumentUnit(Guid idDocumentUnit)
        {
            try
            {
                logger.Debug("UdsOpenDocumentUnit");
                return DocumentService.UdsOpenDocumentUnit(idDocumentUnit);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiunge una nuova DocumentUnit
        /// </summary>
        /// <param name="unit">DocumentUnit da aggiungere</param>
        /// <returns>DocumentUnit inserita</returns>
        public DocumentUnit UdsAddDocumentUnit(DocumentUnit unit)
        {
            try
            {
                logger.Debug("UdsAddDocumentUnit");
                return DocumentService.UdsAddDocumentUnit(unit);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiorna una DocumentUnit esistente
        /// </summary>
        /// <param name="unit">DocumentUnit da aggiornare</param>
        /// <returns>Ritorna document unit aggiornata</returns>
        public DocumentUnit UdsUpdateDocumentUnit(DocumentUnit unit)
        {
            try
            {
                logger.Debug("UdsUpdateDocumentUnit");
                return DocumentService.UdsUpdateDocumentUnit(unit);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Collega Documenti ad una DocumentUnit esistente
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        /// <param name="documents"></param>
        /// <param name="checkUnitExist">Verifica l'esistenza della document unit prima di collegare i documenti (default true)</param>
        /// <returns>N.di documenti collegati</returns>
        public int UdsDocumentUnitAddDocuments(Guid idDocumentUnit, DocumentUnitChain[] documents)
        {
            try
            {
                logger.Debug("UdsDocumentUnitAddDocuments");
                return DocumentService.UdsDocumentUnitAddDocuments(idDocumentUnit, documents);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiunge una nuova DocumentUnit e collega i documenti collegati passati
        /// </summary>
        /// <param name="unit">DocumentUnit da inserire</param>
        /// <param name="documents">Elenco dei riferimenti ai documenti da collegare</param>
        /// <returns>DocumentUnit creata</returns>  
        public DocumentUnit UdsAddDocumentUnitWithDocuments(DocumentUnit unit, DocumentUnitChain[] documents)
        {
            try
            {
                logger.Debug("UdsAddDocumentUnitWithDocuments");
                return DocumentService.UdsAddDocumentUnitWithDocuments(unit, documents);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Elimina una document unit. Rimuove anche i riferimenti dei documenti alla DocumentUnit dalla tabella DocumentUnitChain
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        /// <returns>Ritorna n. di record complessivamente eliminati</returns>
        public int UdsDeleteDocumentUnit(Guid idDocumentUnit)
        {
            try
            {
                logger.Debug("UdsDeleteDocumentUnit");
                return DocumentService.UdsDeleteDocumentUnit(idDocumentUnit);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Elimina tutti i riferimenti ai documenti della document unit passata
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        /// <returns>Ritorna il n. di riferimenti eliminati</returns>
        public int UdsDeleteDocumentUnitChain(Guid idDocumentUnit)
        {
            try
            {
                logger.Debug("UdsDeleteDocumentUnitChain");
                return DocumentService.UdsDeleteDocumentUnitChain(idDocumentUnit);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion

        #region DocumentUnitAggregate Methods


        /// <summary>
        /// Ricerca Aggregato (Fasciolo)
        /// </summary>
        /// <param name="IdAggregate"></param>
        /// <returns>DocumentUnitAggregate (Fascicolo)</returns>
        public DocumentUnitAggregate UdsGetDocumentUnitAggregate(Guid idAggregate)
        {
            try
            {
                logger.DebugFormat("UdsGetDocumentUnitAggregate idAggregate:{0}", idAggregate);
                var res = DocumentService.UdsGetDocumentUnitAggregate(idAggregate);
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Ricerca DocumentUnitAggregate - Ritorna anche se ReadOnly e Preserved
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        /// <returns>Ritorna DocumentUnitAggregate individuato</returns>
        public DocumentUnitAggregateExt UdsGetDocumentUnitAggregateExt(Guid idAggregate)
        {
            try
            {
                logger.DebugFormat("UdsGetDocumentUnitAggregateExt idAggregate:{0}", idAggregate);
                var res = DocumentService.UdsGetDocumentUnitAggregateExt(idAggregate);
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        public BindingList<DocumentUnit> UdsGetAggregateChain(Guid idAggregate)
        {
            try
            {
                logger.DebugFormat("UdsGetAggregateChain idAggregate:{0}", idAggregate);
                var res = DocumentService.UdsGetAggregateChain(idAggregate);
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Apre una DocumentUnitAggregate a meno che una queste non sia già stata Preservata e tutte le relative DocumentUnit 
        /// </summary>
        /// <param name="idDocumentUnit"></param>
        public DocumentUnitAggregate UdsOpenDocumentUnitAggregate(Guid idAggregate)
        {
            try
            {
                logger.DebugFormat("UdsOpenDocumentUnitAggregate idAggregate:{0}", idAggregate);
                var res = DocumentService.UdsOpenDocumentUnitAggregate(idAggregate);
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiunge una nuova DocumentUnitAggregate
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns>Ritorna l'aggregato aggiunto</returns>
        public DocumentUnitAggregate UdsAddDocumentUnitAggregate(DocumentUnitAggregate aggregate)
        {
            try
            {
                logger.Debug("UdsAddDocumentUnitAggregate");
                return DocumentService.UdsAddDocumentUnitAggregate(aggregate);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiorna una DocumentUnitAggregate esistente
        /// </summary>
        /// <param name="aggregate">Aggregato da aggiornare</param>
        /// <returns>Ritorna aggregato aggiornato</returns>
        public DocumentUnitAggregate UdsUpdateDocumentUnitAggregate(DocumentUnitAggregate aggregate)
        {
            try
            {
                logger.Debug("UdsUpdateDocumentUnitAggregate");
                return DocumentService.UdsUpdateDocumentUnitAggregate(aggregate);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Aggiunge DocumentUnit all'aggregato (fascicolo) selezionato
        /// </summary>
        /// <param name="idAggregate">Chiave aggregato</param>
        /// <param name="units">elenco delle document unit (ID's)</param>
        /// <param name="checkAggregateExist">Verifica esistenza aggregato</param>
        /// <returns>N. di document unit aggregate</returns>
        public int UdsDocumentAggregateAddUnits(Guid idAggregate, Guid[] unitsId)
        {
            try
            {
                logger.Debug("UdsDocumentAggregateAddUnits");
                return DocumentService.UdsDocumentAggregateAddUnits(idAggregate, unitsId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Elimina aggregato (fascicolo) - Elimina anche tutti i riferimenti alle DocumentiUnit aggregate.
        /// </summary>
        /// <param name="idAggregate">Chiave aggregato</param>
        /// <returns>N. di rercord complessivamente eliminati</returns>
        public int UdsDeleteDocumentUnitAggregate(Guid idAggregate)
        {
            try
            {
                logger.Debug("UdsDeleteDocumentUnitAggregate");
                return DocumentService.UdsDeleteDocumentUnitAggregate(idAggregate);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }


        /// <summary>
        /// Elimina tutti i riferimenti di DocumentUnit all'aggregato passato 
        /// </summary>
        /// <param name="idAggregate"></param>
        /// <returns>N. di riferimenti eliminati</returns>
        public int UdsDeleteDocumentUnitAggregateChain(Guid idAggregate)
        {
            try
            {
                logger.Debug("UdsDeleteDocumentUnitAggregateChain");
                return DocumentService.UdsDeleteDocumentUnitAggregateChain(idAggregate);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        #endregion

        /// <summary>
        /// Rimuove l'indicizzazione fulltext per tutti i documenti della catena specificata.
        /// </summary>
        /// <param name="idChain"></param>
        public void RemoveFullTextDataForChain(Guid idChain)
        {
            try
            {
                logger.DebugFormat("RemoveFullTextDataForChain idChain:{0}", idChain);
                BindingList<Document> chainDocuments = DocumentService.GetDocumentChildren(idChain);
                if (chainDocuments.Count == 0)
                {
                    logger.WarnFormat("RemoveFullTextDataForChain idChain:{0} is empty, no action required.", idChain);
                    return;
                }

                using (IClientChannel clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    foreach (Document document in chainDocuments)
                    {
                        if (document.Storage != null && document.Storage.EnableFulText)
                        {
                            (clientChannel as IServiceDocumentStorage).DeleteFullTextDocumentData(document);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Rimuove l'indicizzazione fulltext per uno specifico documento.
        /// </summary>
        /// <param name="idDocument"></param>
        public void RemoveFullTextDataForDocument(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("RemoveFullTextDataForDocument idDocument:{0}", idDocument);
                Document document = DocumentService.GetDocumentLatestVersion(idDocument);
                if (document == null)
                {
                    throw new DocumentNotFound_Exception(string.Concat("RemoveFullTextDataForDocument document with id ", idDocument, " not found"));
                }

                using (IClientChannel clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).DeleteFullTextDocumentData(document);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Genera le informazioni necessarie al popolamento dell'indice fulltext per tutti i documenti di una catena specifica.
        /// </summary>
        /// <param name="idChain"></param>
        public void AlignFullTextDataForChain(Guid idChain)
        {
            try
            {
                logger.DebugFormat("AlignFullTextDataForChain idChain:{0}", idChain);
                BindingList<Document> chainDocuments = DocumentService.GetDocumentChildren(idChain);
                if (chainDocuments.Count == 0)
                {
                    logger.WarnFormat("AlignFullTextDataForChain idChain:{0} is empty, no action required.", idChain);
                    return;
                }

                using (IClientChannel clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    foreach (Document document in chainDocuments)
                    {
                        (clientChannel as IServiceDocumentStorage).DeleteFullTextDocumentData(document);
                        (clientChannel as IServiceDocumentStorage).WriteFullTextDocumentData(document);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Genera le informazioni necessarie al popolamento dell'indice fulltext per un documento specifico.
        /// </summary>
        /// <param name="idDocument"></param>
        public void AlignFullTextDataForDocument(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("AlignFullTextDataForDocument idDocument:{0}", idDocument);
                Document document = DocumentService.GetDocumentLatestVersion(idDocument);
                if (document == null)
                {
                    throw new DocumentNotFound_Exception(string.Concat("AlignFullTextDataForDocument document with id ", idDocument, " not found"));
                }

                using (IClientChannel clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).DeleteFullTextDocumentData(document);
                    (clientChannel as IServiceDocumentStorage).WriteFullTextDocumentData(document);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public Document GetDocumentInfoIgnoreState(Guid idDocument)
        {
            try
            {
                logger.DebugFormat("GetDocumentInfoIgnoreState idDocument:{0}", idDocument);
                Document document = DocumentService.GetDocumentIgnoreState(idDocument);
                if (document == null)
                {
                    throw new DocumentNotFound_Exception(string.Concat("GetHistoricalDocumentInfo document with id ", idDocument, " not found"));
                }
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public BindingList<Document> GetDocumentChildrenIgnoreState(Guid idParent)
        {
            try
            {
                logger.DebugFormat("GetDocumentChildrenIgnoreState - idParent: {0}", idParent);
                return DocumentService.GetDocumentChildrenIgnoreState(idParent);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }

        public bool HasActiveDocuments(Guid idParent)
        {
            try
            {
                logger.DebugFormat("HasActiveDocuments - idParent: {0}", idParent);
                return DocumentService.HasDocumentChildren(idParent);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException<BiblosDsException>(new BiblosDsException(ex), new FaultReason(ex.Message));
            }
        }
    }

#if WCF_Documents
}
#endif



