using System;
using System.ComponentModel;
using System.ServiceModel;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Services;
using System.ServiceModel.Activation;
using BiblosDS.Library.Common.Objects.Response;
using VecompSoftware.ServiceContract.BiblosDS.Documents;
using BiblosDS.Library.Common.Objects.UtilityService;
using BiblosDS.Library.Common.Utility;

[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Transit : ITransit
{
    static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Transit));

    public DocumentResponse GetTransitListDocumentsPaged(string archiveName, int skip, int take)
    {
        var ret = new DocumentResponse { TotalRecords = 0, Documents = new BindingList<Document>(), };
        if (!string.IsNullOrEmpty(archiveName))
        {
            ret = DocumentService.GetDocumentInTransitoByArchive(archiveName, take, skip);
            //if (retval.Error == null)
            //    return retval.Documents;
        }
        return ret;
    }

    public bool StoreTransitArchiveDocuments(string archiveName)
    {
        bool bReturn = true;        
        try
        {
            DocumentArchive archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archive not found");           

            BindingList<Document> documents = DocumentService.GetDocumentInTransito(archive.IdArchive, 0); // esegue la query nel db
            Document document;
            foreach (Document item in documents)
            {
                try
                {
                    document = DocumentService.GetDocument(item.IdDocument);
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {                                               
                        (clientChannel as IServiceDocumentStorage).AddDocument(document);                        
                    }                         
                }
                catch (Exception e)
                {
                    bReturn = false;

                    Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                            "Document.ProcessCheckInTransitoDocument",
                            e.ToString(),
                            LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                            LoggingLevel.BiblosDS_Warning);                   
                }
            } // end foreach       

            return bReturn;
        }
        catch (Exception ex)
        {
            Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                "Document.ProcessCheckInTransitoDocument",
                ex.ToString(),
                LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                LoggingLevel.BiblosDS_Warning);

            return false;
        }       
    }

    public bool StoreTransitDocuments()
    {
        logger.Debug("StoreTransitDocuments -> Init");
        bool bReturn = true;       
        try
        {
            BindingList<Document> documents = DocumentService.GetDocumentInTransito(0); // esegue la query nel db
            Document document;
            foreach (Document item in documents)
            {
                try
                {
                    document = DocumentService.GetDocument(item.IdDocument);
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {                        
                        (clientChannel as IServiceDocumentStorage).AddDocument(document);
                    }                         
                }
                catch (Exception e)
                {
                    bReturn = false;

                    Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                            "Document.ProcessCheckInTransitoDocument",
                            e.ToString(),
                            LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                            LoggingLevel.BiblosDS_Warning);                   
                }
            } // end foreach

            logger.InfoFormat("StoreTransitDocuments -> End processed {0} documents that are in 'Transito'", documents.Count);          

            return bReturn;
        }
        catch (Exception ex)
        {
            Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                "Document.ProcessCheckInTransitoDocument",
                ex.ToString(),
                LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                LoggingLevel.BiblosDS_Warning);

            return false;
        }        
    }

    public bool StoreTransitDocument(Guid idDocument)
    {
        bool bReturn = true;        
        try
        {
            Document document;

            try
            {
                document = DocumentService.GetDocument(idDocument);
                using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                {
                    (clientChannel as IServiceDocumentStorage).AddDocument(document);
                }                   
            }
            catch (Exception e)
            {
                bReturn = false;

                Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                        "Document.ProcessCheckInTransitoDocument",
                        e.ToString(),
                        LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                        LoggingLevel.BiblosDS_Warning);
              
            }

            logger.InfoFormat("Document.ProcessCheckInTransitoDocument processed document: {0}", idDocument);
            
            return bReturn;
        }
        catch (Exception ex)
        {
            Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                "Document.ProcessCheckInTransitoDocument",
                ex.ToString(),
                LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                LoggingLevel.BiblosDS_Warning);

            return false;
        }       
    }


    public bool StoreTransitArchiveDocumentAttaches(string archiveName)
    {
        bool bReturn = true;   
        try
        {
            DocumentArchive archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archive not found");
            

            BindingList<DocumentAttach> documents = DocumentService.GetDocumentAttachesInTransito(archive.IdArchive, 0); // esegue la query nel db
            DocumentAttach document;
            foreach (var item in documents)
            {
                try
                {
                    document = DocumentService.GetDocumentAttach(item.IdDocumentAttach);
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
                    {
                        (clientChannel as IServiceDocumentStorage).AddAttachToDocument(document);
                    }                      
                }
                catch (Exception e)
                {
                    bReturn = false;

                    Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                            "Document.ProcessCheckInTransitoDocumentAttach",
                            e.ToString(),
                            LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                            LoggingLevel.BiblosDS_Warning);
                   
                }
            } // end foreach       

            return bReturn;
        }
        catch (Exception ex)
        {
            Logging.WriteLogEvent(LoggingSource.BiblosDS_WS,
                "Document.ProcessCheckInTransitoDocument",
                ex.ToString(),
                LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                LoggingLevel.BiblosDS_Warning);

            return false;
        }       
    }


    public DocumentResponse GetTransitServerListDocumentsPaged(string archiveName, string serverName, int skip, int take)
    {
        throw new NotImplementedException();
    }

    public bool StoreTransitServerDocument(Guid idDocument, string serverName)
    {
        throw new NotImplementedException();
    }
}
