using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Security.Cryptography;
using System.IO;

using BiblosDS.Library.IStorage;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.WCF.Storage.ServiceReferenceDigitalSigned;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.UtilityService;
using BiblosDS.Library.Common.Utility;
using VecompSoftware.ServiceContract.BiblosDS.Documents;

namespace BiblosDS.WCF.Storage
{
    public class ServiceDocumentStorage : IServiceDocumentStorage
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServiceDocumentStorage));  
        #region IServiceDocumentStorage Members

        public Guid AddDocument(BiblosDS.Library.Common.Objects.Document Document)
        {            
            string localPath = string.Empty;
            Guid idCorrelation = Guid.NewGuid();
            DocumentContent docContent = null;
            DocumentTransito transito = null;
            try
            {
                logger.DebugFormat("AddDocument to Storage: {0}", Document.IdDocument);
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddDocument", "Init add document", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, Document);

                if (Document.Content != null)
                    docContent = new DocumentContent { Blob = Document.Content.Blob, Description = Document.Content.Description };

                Document = DocumentService.GetDocument(Document.IdDocument);
                if (Document == null)
                {
                    logger.WarnFormat("No Document Found with IdDocument: {0}", Document.IdDocument);
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                }   

                //Change the state on the transiction path table(Lock)               
                if (Document.Archive.TransitoEnabled)
                {
                    transito = DocumentService.CheckOutTransitoDocument(Document.IdDocument);
                    if (transito == null)
                        throw new BiblosDS.Library.Common.Exceptions.FileNotFound_Exception("Document not found in Transito:"+ Document.IdDocument);
                }
                Document.AttributeValues = AttributeService.GetAttributeValues(Document.IdDocument);

                //BiblosDS Proxy
                var servers = ServerService.GetServers();

                var currentServer = servers.FirstOrDefault(x => x.ServerName == MachineService.GetServerName());                

                //Retrive the document Storage
                if (Document.Storage == null || currentServer != null)
                {
                    Document.Storage = DocumentService.GetStorage(Document.Archive, currentServer, Document.AttributeValues);
                    //if there is no configured storage throw exception
                    if (Document.Storage == null)
                        throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception();
                }
                                
                //Retrive the document StorageArea    
                Document.StorageArea = DocumentService.GetStorageArea(Document, Document.AttributeValues);
                if (Document.StorageArea == null)
                    throw new BiblosDS.Library.Common.Exceptions.StorageAreaConfiguration_Exception();
                
                //If not is loaded the StorageType Try to load.
                if (Document.Storage.StorageType == null)
                    Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);

                if (Document.Archive.EnableSecurity)
                    Document.Permissions = DocumentService.GetDocumentPermissions(Document.IdDocument);
                logger.DebugFormat("AddDocument to Storage: {0} - Storage Area: {1} - Storage Type = {2}", Document.Storage.Name, Document.StorageArea.Name, Document.Storage.StorageType);
                //Find the store & put the document on the archive
                var storageFromLoader = StorageAssemblyLoader.GetStorage(Document.Storage.StorageType.IdStorageType);                
                //if (Document.Archive.ThumbnailEmabled || Document.Archive.PdfConversionEmabled)
                //{
                //    logger.Debug("Write thumbnail and add document");
                //    new ThumbnailStorage().CreateThumbnailAndAddFromStorage(Document, storageFromLoader);
                //}                
                logger.Debug("Adding document");

                byte[] content = null;
                if (Document.Archive.TransitoEnabled)
                {
                    localPath = DocumentService.GetTransitoLocalPath(transito);

                    var localFilePath = Path.Combine(localPath,
                                                       Document.IdDocument.ToString() +
                                                       Path.GetExtension(Document.Name));

                    content = File.ReadAllBytes(localFilePath);
                }
                else if(Document.Content == null)
                {                    
                    content = (docContent == null) ? new byte[0] : docContent.Blob; 
                }                

                ServerRole currentServerRole = ServerRole.Undefined;
                if (currentServer != null)
                    currentServerRole = currentServer.ServerRole;

                Document.Content = new DocumentContent(content);

                var task = Task.Factory.StartNew( () =>
                    {
                        if (Document.Archive.ThumbnailEmabled || Document.Archive.PdfConversionEmabled)
                        {
                            try
                            {                               

                                BiblosDSConv.BiblosDSConv conv = new BiblosDSConv.BiblosDSConv();
                                var pdfDoc = conv.ToRaster(new BiblosDSConv.stDoc { Blob = Convert.ToBase64String(content), FileExtension = Document.Name });
                                if (Document.Archive.PdfConversionEmabled)
                                {
                                    string pdfName = Guid.NewGuid().ToString() + ".pdf";
                                    storageFromLoader.AddConformAttach(Document, new DocumentContent(Convert.FromBase64String(pdfDoc.Blob)), pdfName);
                                    DocumentService.SetDocumentPdf(Document.IdDocument, pdfName);
                                }
                                if (Document.Archive.ThumbnailEmabled)
                                {
                                    string thumbnailName = Guid.NewGuid().ToString() + ".png";
                                    var thumbnailDoc = conv.PdfToPngThumbnail(pdfDoc);
                                    storageFromLoader.AddConformAttach(Document, new DocumentContent(Convert.FromBase64String(thumbnailDoc.Blob)), thumbnailName);
                                    DocumentService.SetDocumentThumbnail(Document.IdDocument, thumbnailName);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);                                
                            }                           
                        }                        
                    });

                var task1 = Task.Factory.StartNew(() =>
                    {        
                        if (currentServerRole != ServerRole.Proxy)
                            storageFromLoader.AddDocument(Document, Document.AttributeValues);
                        if (currentServerRole != ServerRole.Undefined)
                            DocumentService.SaveDocumentToMaster(Document, currentServer, DocumentStatus.InStorage);

                    });

                //Se il server è configurato come full-proxy il file viene caricato nel master
                var task2 = Task.Factory.StartNew(() =>
                {
                    if (currentServerRole == ServerRole.FullProxy)
                    {
                        var masterServer = servers.FirstOrDefault(x => x.ServerRole == ServerRole.Master);
                        if (masterServer != null)
                        {                            
                            using (var clientChannel = WCFUtility.GetClientConfigChannel<IDocuments>(ServerService.WCF_Document_HostName, masterServer.ServerName))
                            {
                                var retVal = (clientChannel as IDocuments).AddDocumentToMaster(Document);
                            }
                        }
                    }
                });

                Task.WaitAll(task, task1, task2);

                if (!task.IsCompleted || !task1.IsCompleted || !task2.IsCompleted)
                    throw new Exception("Not completed all task exception.");
                

                // if (Document.Name.EndsWith(".p7m", StringComparison.InvariantCultureIgnoreCase)
                //    || Document.Name.EndsWith(".m7m", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    DocumentCertificate certificate = new DocumentCertificate();
                //    using (ServiceDigitalSignClient cllient = new ServiceDigitalSignClient("ServiceDigitalSign"))
                //    {
                //        cllient.GetAllExpireDates(out certificate, Document.Name, Document.Content);
                //    }
                //    if (certificate != null)
                //        Document.DateExpire = certificate.DateExpiration;
                //}
                logger.Debug("GetAttributesHash");
                //Calculate the document Sign Header
                Document.SignHeader = AttributeService.GetAttributesHash(Document.AttributeValues);
                logger.Debug("GetAttributesHash 1");
                //Calculate the document Hash                
                Document.DocumentHash = DocumentService.GetDocumentHash(Document);
                logger.Debug("GetDocumentHash - ok");
                //Sign the document...
                //TODO Chiamare il metodo di Benoni: Chiedere a Piccoli se OK (chiamata al servizio da quì)
                //Il salvataggio degli attributi passati proporrei di farlo in fase di inserimento e quì andare a chiamare i metodi
                //Di firma e sign: può essere giusto.
                if (Document.Archive.FullSignEnabled)
                {
                    Document.Certificate = DocumentService.GetCertificateDefault();
                    if (Document.Certificate == null)
                        throw new BiblosDS.Library.Common.Exceptions.CertificateNotFound_Exception();
                    logger.DebugFormat("GetCertificateDefault - {0}", Document.Certificate.Name);
                    Document.FullSign = UtilityService.GetStringFromBob(
                        DocumentService.Sign(
                            Document.IdDocument,
                            Document.DocumentParent.IdDocument,
                            Document.Content,
                            Path.GetExtension(Document.Name),
                            DateTime.Now.ToString("yyyyMMdd"),
                            Document.AttributeValues,
                            Document.Certificate));
                    logger.Debug("Signed - ok");
                }else
                    logger.Debug("Signed - DISABLED");
                //Set the status to "In Storage"
                Document.Status = new Status((short)DocumentStatus.InStorage);
                
                if(Document.Archive.TransitoEnabled)
                    transito.TarnsitoStatus = DocumentTarnsitoStatus.EndProcessing;
                logger.Debug("SaveAtomic - Init");
                //Update the document value.
                if (Document.Archive.TransitoEnabled)
                {
                    DocumentService.SaveAtomic(null, new List<BiblosDSObject> { Document, transito });
                }
                else
                {
                    DocumentService.SaveAtomic(null, new List<BiblosDSObject> { Document });
                }
                logger.Debug("SaveAtomic - End");
                // update Transito
                //DocumentService.CheckInTransitoDocument(Document.IdDocument);
                if (Document.Archive.TransitoEnabled)
                {
                    if (transito.Document == null)
                        transito.Document = Document;
                    FileService.DeleteFileToTransitoLocalPath(transito);
                }
                logger.Debug("DeleteFileToTransitoLocalPath - End");
                //TODO Fine (Da finire a definire cos'altro deve fare)                
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddDocument", "End add document", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, Document);
                return Document.IdDocument;
            }
            catch (StorageIsProcessingFile_Exception ex)
            {
                logger.DebugFormat("AddDocument:{0} - {1}", Document.IdDocument, ex.Message);
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddDocument", "End StorageIsProcessingFile_Exception", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error, idCorrelation, Document);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);                
                try
                {
                    //TODO UNDO THE OPERATION (gestire il caso di fallimento cambio stato)
                    if (transito != null)
                        DocumentService.UndoCheckOutTransitoDocument(transito);
                }
                catch (Exception exundo)
                {
                    Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "AddDocument", exundo, LoggingOperationType.BiblosDS_InsertDocument, Document);                    
                }                            
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "AddDocument", ex, LoggingOperationType.BiblosDS_InsertDocument, Document);
                throw new FaultException(ex.Message);  
            }                                                       
        }
       

        public Document GetDocumentConformAttach(Document doc, string fileName)
        {
            logger.DebugFormat("GetDocumentAttach: {1} - {0}", doc.IdDocument, doc.IdBiblos);
            Document document = null;
            Guid idCorrelation = Guid.NewGuid();
            try
            {
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", "Init GetDocument", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, doc);
                //Load the document with the feature to retrive the storage & storage area of the docuemnt
                document = DocumentService.GetDocument(doc.IdDocument);
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();

                if (document.Storage == null)
                    throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception("Storage non configurato o Documento in transito.");
                if (document.Storage.StorageType == null)
                    document.Storage = StorageService.GetStorage(document.Storage.IdStorage);                             
                IStorage loader = StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType);
                //Verifica degli attributi del documento richiesto.
               
                document.Content = new DocumentContent(loader.GetAttach(document, fileName));                
             

                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", "End GetDocument", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, document);
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocumentAttach", ex, LoggingOperationType.BiblosDS_GetDocument, document);
                throw new FaultException(ex.Message);
            }            
        }

        public Document GetDocument(Document Document)
        {
            logger.DebugFormat("GetDocument: {1} - {0}", Document.IdDocument, Document.IdBiblos);            
            Guid idCorrelation = Guid.NewGuid();
            Document document = null;
            try
            {
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", "Init GetDocument", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, Document);
                ServerRole serverRole = ServerRole.Undefined;
                //BiblosDS Proxy
                var currentServer = ServerService.GetCurrentServer();
                List<DocumentServer> documentInServer = null;
                if (currentServer != null)
                {
                    documentInServer = ServerService.GetDocumentInServer(Document.IdDocument);
                    serverRole = currentServer.ServerRole;
                    if (serverRole == ServerRole.Proxy || !ServerService.CheckDocumentInServer(currentServer, documentInServer))
                    {
                        //Retrive document from master
                        var requestServer = ServerService.GetMasterServer();
                        if (!ServerService.CheckDocumentInServer(requestServer, documentInServer))
                            requestServer = documentInServer.First().Server;
                        logger.DebugFormat("Server is a proxy, Redirect to: {1} - IdDocument:{0}", Document.IdDocument, requestServer.ServerName);
                        if (requestServer != null)
                        {
                            using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName, requestServer.ServerName))
                            {
                                document = (clientChannel as IServiceDocumentStorage).GetDocument(Document);
                            }
                            return document;
                        }
                        else
                            throw new BiblosDS.Library.Common.Exceptions.ServerNotDefined_Exception("Nessun server master definito. Configurazione non valida per il server proxy: " + MachineService.GetServerName() + ".");
                    }
                    else
                    {
                        document = DocumentService.GetDocument(Document.IdDocument);
                        var documentDetail = ServerService.GetDocumentInServer(currentServer, document.IdDocument);
                        document.Status = documentDetail.Status;
                        document.Storage = documentDetail.Storage;
                        document.StorageArea = documentDetail.StorageArea;
                        logger.DebugFormat("Server role: {0}, Status:{1}, Storage:{2}, StorageArea:{3}", currentServer.ServerName, document.Status.Description, document.Storage == null ? "" : document.Storage.Name, document.StorageArea == null ? "" : document.StorageArea.Name);
                    }
                }
                else
                {
                    //Load the document with the feature to retrive the storage & storage area of the docuemnt                
                    document = DocumentService.GetDocument(Document.IdDocument);
                }
                if (document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();               
                try
                {
                    switch ((DocumentStatus)document.Status.IdStatus)
                    {
                        case DocumentStatus.Undefined:
                        case DocumentStatus.InTransito:
                            document.Content = GetDocumentInTransito(document);
                            break;
                        case DocumentStatus.InStorage:
                            document.Content = GetDocumentInStorage(document);
                            break;
                        case DocumentStatus.InCache:
                            document.Content = GetDocumentInCache(document);
                            break;
                        case DocumentStatus.ProfileOnly:
                            if (document.DocumentLink != null)
                                document = GetDocument(document.DocumentLink);                            
                            break;
                        case DocumentStatus.RemovedFromStorage:
                            document.Content = new DocumentContent(new byte[] { });
                            break;
                        case DocumentStatus.MovedToPreservation:
                            document.Content = GetDocumentInPreservation(document);
                            break;
                        default:
                            throw new Exception("Impossibile elaborare lo stato del documento.");
                    }
                }
                catch (FileNotFoundException ex)
                {
                    logger.Error(ex);
                    Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocument(" +Document.IdDocument.ToString()+")", ex, LoggingOperationType.BiblosDS_GetDocument, document);
                    if (ConfigurationManager.AppSettings["DefaultFileOnError"] != null && File.Exists(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()))
                    {
                        logger.Info("Return default file..");
                        document.Content = new DocumentContent(File.ReadAllBytes(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", ex, LoggingOperationType.BiblosDS_GetDocument, document);
                    if (ConfigurationManager.AppSettings["DefaultFileOnError"] != null && File.Exists(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()))
                    {
                        logger.Info("Return default file..");
                        document.Content = new DocumentContent(File.ReadAllBytes(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()));
                    }
                    else
                    {
                        throw;
                    }
                }
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", "End GetDocument", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, document);
                return document;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", ex, LoggingOperationType.BiblosDS_GetDocument, document);
                throw new FaultException(ex.Message);
            }
        }

        public DocumentAttach GetDocumentAttach(DocumentAttach attach)
        {
            logger.DebugFormat("GetDocumentAttach: {1} - {0}", attach.IdDocumentAttach, attach.IdDocument);
            DocumentAttach documentAttach = null;            
            try
            {                
                //Load the document with the feature to retrive the storage & storage area of the docuemnt
                documentAttach = DocumentService.GetDocumentAttach(attach.IdDocumentAttach);
                //if (documentAttach == null)
                //    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                try
                {
                    switch ((DocumentStatus)documentAttach.Status.IdStatus)
                    {
                        case DocumentStatus.Undefined:
                        case DocumentStatus.InTransito:
                            documentAttach.Content = GetDocumentAttachInTransito(documentAttach);
                            break;
                        case DocumentStatus.InStorage:
                            attach.Storage = attach.Document.Storage;
                            attach.StorageArea = attach.Document.StorageArea;
                            attach.Archive = attach.Document.Archive;
                            attach.IdDocument = documentAttach.IdDocumentAttach;
                            documentAttach.Content = GetDocumentAttachInStorage(attach);
                            break;
                        case DocumentStatus.MovedToPreservation:
                            documentAttach.Content = GetDocumentInPreservation(documentAttach);
                            break;
                        default:
                            throw new Exception("Impossibile elaborare lo stato del documento.");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", ex, LoggingOperationType.BiblosDS_GetDocument, attach);
                    if (ConfigurationManager.AppSettings["DefaultFileOnError"] != null && File.Exists(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()))
                    {
                        logger.Info("Return default file..");
                        attach.Content = new DocumentContent(File.ReadAllBytes(ConfigurationManager.AppSettings["DefaultFileOnError"].ToString()));
                    }
                    else
                    {
                        throw;
                    }
                }
                return documentAttach;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "GetDocument", ex, LoggingOperationType.BiblosDS_GetDocument, attach);
                throw new FaultException(ex.Message);
            }
        }

        public void InitializeStorage(DocumentStorage storage)
        {
            try
            {
                if (storage == null)
                {
                    throw new ArgumentNullException("storage", "Parameter storage is null");
                }

                IStorage storageLoader = StorageAssemblyLoader.GetStorage(storage.StorageType.IdStorageType);
                if (storageLoader == null)
                {
                    throw new Generic_Exception(string.Concat("Nessuna tipologia di Storage trovata per lo StorageType ", storage.StorageType.IdStorageType));
                }

                storageLoader.InitializeStorage(storage);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new FaultException(ex.Message);
            }
        }

        #region GetDocument From
        /// <summary>
        /// Retrive document from Chache
        /// </summary>
        /// <param name="Document"></param>
        /// <returns>
        ///     <see cref="DocumentContent"/> with the file
        /// </returns>
        private DocumentContent GetDocumentInCache(Document Document)
        {
            if (string.IsNullOrEmpty(Document.Archive.PathCache))
                Document.Archive = ArchiveService.GetArchive(Document.Archive.IdArchive);         
            return new DocumentContent(File.ReadAllBytes(Path.Combine(Document.Archive.PathCache, Document.IdDocument.ToString() + Path.GetExtension(Document.Name))));
        }

        /// <summary>
        /// Retrive document from Transito
        /// </summary>
        /// <param name="Document"></param>
        /// <returns>
        ///     <see cref="DocumentContent"/> with the file
        /// </returns>
        private DocumentContent GetDocumentInTransito(Document Document)
        {
            var transito = DocumentService.GetTransito(Document.IdDocument);               
            if (transito == null)
                throw new BiblosDS.Library.Common.Exceptions.FileNotFound_Exception("Documento configurato in transito ma non presente: "+ Document.IdDocument);
            string transitoPath = transito.LocalPath;
            return new DocumentContent(File.ReadAllBytes(Path.Combine(transitoPath, Document.IdDocument.ToString() + Path.GetExtension(Document.Name))));
        }

        private DocumentContent GetDocumentAttachInTransito(DocumentAttach attach)
        {
            string transitoPath = DocumentService.GetAttachTransitLocalPath(attach.IdDocumentAttach);
            return new DocumentContent(File.ReadAllBytes(Path.Combine(transitoPath, attach.Document.IdDocument + "_" + attach.IdDocumentAttach + Path.GetExtension(attach.Name))));
        }

        /// <summary>
        /// Retrive document from Storage
        /// </summary>
        /// <param name="Document"></param>
        /// <returns>
        ///     <see cref="DocumentContent"/> with the file
        /// </returns>        
        private DocumentContent GetDocumentInStorage(Document Document)
        {
            if (Document.Storage == null)
                throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception("Storage non configurato o Documento in transito.");
            if (Document.AttributeValues == null || Document.AttributeValues.Count <= 0)
                Document.AttributeValues = AttributeService.GetAttributeValues(Document.IdDocument);
            else
                Document.AttributeValues = Document.AttributeValues;
            if (Document.Storage.StorageType == null)
                Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);
            //Gianni:27-01-2010 se ho un documento linkato, recupero il content del documento link                
            IStorage loader = StorageAssemblyLoader.GetStorage(Document.Storage.StorageType.IdStorageType);
            //Verifica degli attributi del documento richiesto.
            if (loader.VerifyAttribute(Document, Document.AttributeValues))
            {
                var tmpDoc = Document.DocumentLink != null ? DocumentService.GetDocument(Document.DocumentLink.IdDocument) : Document;
                var content = loader.GetDocument(tmpDoc);
                Document.Content = new DocumentContent(content, tmpDoc.Name);                
            }
            else
            {
                throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati.");
            }

            return Document.Content;
        }

        private DocumentContent GetDocumentAttachInStorage(DocumentAttach attach)
        {
            if (attach.Document == null || attach.Document.Storage == null)
                throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception("Storage non configurato o Documento in transito.");
            if (attach.Document.Storage.StorageType == null)
                attach.Document.Storage = StorageService.GetStorage(attach.Document.Storage.IdStorage);            
            IStorage loader = StorageAssemblyLoader.GetStorage(attach.Document.Storage.StorageType.IdStorageType);            
            attach.IdDocument = attach.IdDocumentAttach;
            attach.Content = new DocumentContent(loader.GetDocument(attach));           
            return attach.Content;
        } 

        private DocumentContent GetDocumentInPreservation(Document document)
        {
            string preservationPath = DocumentService.GetPreservationDocumentPath(document);
            if (string.IsNullOrEmpty(preservationPath))
            {
                throw new FileNotFound_Exception($"Documento configurato in conservazione non presente: {document.IdDocument}");
            }
            return new DocumentContent(File.ReadAllBytes(preservationPath));
        }

        #endregion

        #endregion

        #region Private Method

        /// <summary>
        /// Retrive the storage interface
        /// associated with the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private IStorage GetStorageIterface(Document document)
        {
            return null;
        }
        #endregion

        #region IServiceDocumentStorage Members

        public bool CheckIntegrity(Document Document)
        {
            try
            {
                if (Document != null) Document = DocumentService.GetDocument(Document.IdDocument); else throw new DocumentNotFound_Exception();
                if (Document.Storage == null) throw new StorageNotFound_Exception("Storage non configurato o Documento in transito.");
                if (Document.AttributeValues == null || Document.AttributeValues.Count <= 0) Document.AttributeValues = AttributeService.GetAttributeValues(Document.IdDocument);
                else Document.AttributeValues = Document.AttributeValues;
                if (Document.Storage.StorageType == null) Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);
                IStorage loader = StorageAssemblyLoader.GetStorage(Document.Storage.StorageType.IdStorageType);
                //Verifica degli attributi del documento richiesto.
                return loader.VerifyAttribute(Document, Document.AttributeValues);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "CheckIntegrity", ex, LoggingOperationType.BiblosDS_General, Document);
                throw;
            }           
        }

        public bool IsAlive()
        {            
            return true;
        }

        #endregion

        #region Delete

        public void DeleteDocument(Guid IdDocument)
        {
            Document document = null;
            try
            {
                document = DocumentService.GetById(IdDocument);
                if (document == null)
                {
                    logger.WarnFormat("Documento con Id {0} non trovato", IdDocument);
                    return;
                }

                StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType).DeleteDocument(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "DeleteDocument", ex, LoggingOperationType.BiblosDS_General, document);
                throw;
            }           
        }       

        public void RestoreAttribute(Guid IdDocument)
        {
            Document document = null;
            try
            {
                //Load the document with the feature to retrive the storage & storage area of the docuemnt
                document = DocumentService.GetDocument(IdDocument);
                document.AttributeValues = AttributeService.GetAttributeValues(document.IdDocument);
                StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType).RestoreAttribute(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "RestoreAttribute", ex, LoggingOperationType.BiblosDS_General, document);
                throw;
            }           
        }

        public void WriteAttribute(Document Document)
        {
            try
            {
                if (Document.Storage == null)
                    throw new StorageNotFound_Exception();
                if (Document.Storage.StorageType == null)
                    Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);
                StorageAssemblyLoader.GetStorage(Document.Storage.StorageType.IdStorageType).WriteAttributes(Document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "WriteAttribute", ex, LoggingOperationType.BiblosDS_General, Document);
                throw;
            }           
        }

        /// <summary>
        /// Elimina il contenuto degli indici fulltext di uno specifico documento, rimuovendo i documenti salvati nelle specifiche filetable.
        /// </summary>
        /// <param name="document">Documento da gestire</param>
        public void DeleteFullTextDocumentData(Document document)
        {
            try
            {
                if (document == null)
                    throw new ArgumentNullException("Parameter document is null");

                document = DocumentService.GetDocument(document.IdDocument);
                if (document.Storage == null)
                    throw new StorageNotFound_Exception(string.Format("Nessuno storage definito per il documento con Id {0}", document.IdDocument));
                if (document.Storage.StorageType == null)
                    document.Storage = StorageService.GetStorage(document.Storage.IdStorage);
                StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType).DeleteFullTextDocuments(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "DeleteFullTextDocumentData", ex, LoggingOperationType.BiblosDS_General, document);
                throw;
            }
        }

        /// <summary>
        /// Dato un documento esistente, vengono salvati i documenti necessari all'indicizzazione fulltext nelle specifiche filetable.
        /// </summary>
        /// <param name="document">Documento da gestire</param>
        public void WriteFullTextDocumentData(Document document)
        {
            try
            {
                if (document == null)
                    throw new ArgumentNullException("Parameter document is null");

                document = DocumentService.GetDocument(document.IdDocument);
                if (document.Storage == null)
                    throw new StorageNotFound_Exception(string.Format("Nessuno storage definito per il documento con Id {0}", document.IdDocument));
                if (document.Storage.StorageType == null)
                    document.Storage = StorageService.GetStorage(document.Storage.IdStorage);
                StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType).WriteFullTextDocuments(document);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "WriteFullTextDocumentData", ex, LoggingOperationType.BiblosDS_General, document);
                throw;
            }
        }
        
        #endregion        
          
        //Idem alla AddDocument ma sulla tabella attach
        public Guid AddAttachToDocument(DocumentAttach attach)
        {
            logger.DebugFormat("AddAttachToDocument in Storage: Id Attach {0} IdDocument {1}", attach.IdDocumentAttach, (attach.Document != null) ? attach.Document.IdDocument.ToString() : "N/A");
            string localPath = string.Empty;
            Guid idCorrelation = Guid.NewGuid();
            DocumentContent docContent = null;
            DocumentAttachTransito transito = null;
            try
            {
                if (attach.IdDocument == Guid.Empty)
                    throw new DocumentNotFound_Exception();

                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddAttachToDocument", "Init add document attach", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, attach.Document);

                var document = DocumentService.GetDocument(attach.IdDocument);

                if (document == null)
                {
                    logger.WarnFormat("No Document Found with IdDocument: {0}", document.IdDocument);
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                }

                if (document.Status == null || document.Status.IdStatus != (short)DocumentStatus.InStorage)
                {
                    throw new DocumentNotReadyForAttach_Exception();
                }
                //Retrive the document Storage
                if (document.Storage == null)
                {
                    throw new DocumentNotReadyForAttach_Exception();
                }
                //Retrive the document StorageArea    
                //document.StorageArea = StorageService.GetStorageArea(document.StorageArea.IdStorageArea);
                if (document.StorageArea == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotReadyForAttach_Exception();

                if (document.Storage.StorageType == null)
                {
                    //document.Storage = StorageService.GetStorage(document.Storage.IdStorage);
                    //if (document.Storage == null || document.Storage.StorageType == null)
                        throw new StorageConfiguration_Exception();
                }

                if (attach.Content != null)
                    docContent = new DocumentContent { Blob = attach.Content.Blob, Description = attach.Content.Description };
                else
                    throw new FileNotFound_Exception("Content of attach not specified. IdDocument:" + attach.IdDocument);
                                              
                //Change the state on the transiction path table(Lock)                
                if (document.Archive.TransitoEnabled)
                    transito = DocumentService.CheckOutTransitoAttach(attach.IdDocumentAttach);

                logger.DebugFormat("AddAttachToDocument in Storage: {0} - Storage Area: {1} - Storage Type = {2}", document.Storage.Name, document.StorageArea.Name, document.Storage.StorageType);
                //Find the store & put the document on the archive
                var storageFromLoader = StorageAssemblyLoader.GetStorage(document.Storage.StorageType.IdStorageType);
                logger.Debug("Adding attachment");

                byte[] content = null;
                if (document.Archive.TransitoEnabled)
                {
                    localPath = transito.LocalPath; 

                    var localFilePath = Path.Combine(localPath,
                                                       attach.IdDocument + "_" + attach.IdDocumentAttach.ToString() +
                                                       Path.GetExtension(document.Name));

                    content = File.ReadAllBytes(localFilePath);
                }
                else if (document.Content == null)
                {
                    content = (docContent == null) ? new byte[0] : docContent.Blob;
                }

                document.Content = new DocumentContent(content);

                attach.Storage = document.Storage;
                attach.StorageArea = document.StorageArea;
                attach.Archive = document.Archive;
                attach.IdBiblos = document.IdBiblos;
                attach.DocumentParent = document.DocumentParent;

                storageFromLoader.AddDocumentAttach(attach);

                attach.IdDocument = attach.Document.IdDocument;

                //Set the status to "In Storage"
                attach.Status = new Status((short)DocumentStatus.InStorage);

                if (document.Archive.TransitoEnabled)
                    transito.TarnsitoStatus = DocumentTarnsitoStatus.EndProcessing;

                logger.Debug("SaveAtomic - Init");
                //Update the document value.
                if (document.Archive.TransitoEnabled)
                {
                    DocumentService.SaveAtomic(null, new List<BiblosDSObject> { attach, transito });
                }
                else
                {
                    DocumentService.SaveAtomic(null, new List<BiblosDSObject> { attach });
                }
                logger.Debug("SaveAtomic - End");

                if (document.Archive.TransitoEnabled)
                    FileService.DeleteFileToTransitoAttachLocalPath(transito);

                logger.Debug("DeleteFileToTransitoLocalPath - End");
                //TODO Fine (Da finire a definire cos'altro deve fare)                
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddAttachToDocument", "End add document", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, idCorrelation, attach.Document);
                return attach.IdDocument;
            }
            catch (StorageIsProcessingFile_Exception ex)
            {
                /*OCCHIO*/
                logger.DebugFormat("AddAttachToDocument:{0} - {1}", attach.IdDocument, ex.Message);
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WCF_Storage, "AddAttachToDocument", "End StorageIsProcessingFile_Exception", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error, idCorrelation, attach.Document);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                try
                {
                    //TODO UNDO THE OPERATION (gestire il caso di fallimento cambio stato)
                    DocumentService.UndoCheckOutTransitoAttach(attach.IdDocumentAttach);
                }
                catch (Exception exundo)
                {
                    Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "AddAttachToDocument", exundo, LoggingOperationType.BiblosDS_InsertDocument, attach.Document);
                }
                Logging.WriteError(LoggingSource.BiblosDS_WCF_Storage, "AddAttachToDocument", ex, LoggingOperationType.BiblosDS_InsertDocument, attach.Document);
                throw new FaultException(ex.Message);
            }      
        }
    }
}
