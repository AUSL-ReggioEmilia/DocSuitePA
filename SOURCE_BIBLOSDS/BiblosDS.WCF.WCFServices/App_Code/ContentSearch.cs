using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.Activation;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Utility;
using VecompSoftware.ServiceContract.BiblosDS.Documents;

[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ContentSearch : IContentSearch
{
    log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ContentSearch));

    #region [ IContentSearch Members ]

    public bool IsAlive()
    {
        return true;
    }

    public DocumentResponse SearchQueryPaged(BindingList<DocumentCondition> AttributeConditions,
        BindingList<DocumentCondition> ContentConditions,
        BindingList<DocumentCondition> InferConditions,
        int? skip,
        int? take)
    {
        try
        {
            logger.DebugFormat("SearchQuery AttributeConditions:{0}", AttributeConditions == null ? 0 : AttributeConditions.Count);
            List<Document> documents = new List<Document>();
            int documentsInArchiveCount = 0;
            Guid? idArchive = null;
            if (AttributeConditions != null && AttributeConditions.Count > 0)
                documents.AddRange(DocumentService.SearchDocuments(AttributeConditions, out documentsInArchiveCount, skip, take, idArchive));
            var items = new BindingList<Document>(documents);
            logger.DebugFormat("SearchQuery Retuns:{0}", items.Count);
            return new DocumentResponse { Documents = items, TotalRecords = documentsInArchiveCount };
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            return new DocumentResponse { HasErros = true, Error = new ResponseError(ex) };
        }
    }

    public BindingList<Document> SearchQuery(BindingList<DocumentCondition> AttributeConditions,
                                             BindingList<DocumentCondition> ContentConditions,
                                             BindingList<DocumentCondition> InferConditions)
    {
        try
        {
            logger.DebugFormat("SearchQuery AttributeConditions:{0}", AttributeConditions == null ? 0 : AttributeConditions.Count);
            List<Document> documents = new List<Document>();
            int documentsInArchiveCount = 0;
            if (AttributeConditions != null && AttributeConditions.Count > 0)
                documents.AddRange(DocumentService.SearchDocuments(AttributeConditions, out documentsInArchiveCount));
            var items = new BindingList<Document>(documents);
            logger.DebugFormat("SearchQuery Retuns:{0}", items.Count);
            return items;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public BindingList<Document> SearchQueryLatestVersion(BindingList<DocumentCondition> AttributeConditions,
                                             BindingList<DocumentCondition> ContentConditions,
                                             BindingList<DocumentCondition> InferConditions)
    {
        try
        {
            logger.DebugFormat("SearchQuery AttributeConditions:{0}", AttributeConditions == null ? 0 : AttributeConditions.Count);
            List<Document> documents = new List<Document>();
            int documentsInArchiveCount = 0;
            if (AttributeConditions != null && AttributeConditions.Count > 0)
                documents.AddRange(DocumentService.SearchDocuments(AttributeConditions, out documentsInArchiveCount, latestVersion: true));
            var items = new BindingList<Document>(documents);
            logger.DebugFormat("SearchQuery Retuns:{0}", items.Count);
            return items;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    public DocumentResponse SearchQueryContext(string archiveName, BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions, int? skip, int? take)
    {
        try
        {
            logger.DebugFormat("SearchQueryContext AttributeConditions:{0}", attributeConditions != null ? attributeConditions.Count : 0);
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new Archive_Exception(string.Concat("Archive: ", archiveName, " not found."));

            List<Document> documents = new List<Document>();
            int documentsInArchiveCount = 0;
            if (attributeConditions != null && attributeConditions.Count > 0)
                documents.AddRange(DocumentService.SearchDocuments(attributeConditions, out documentsInArchiveCount, skip, take, archive.IdArchive));
            BindingList<Document> items = new BindingList<Document>(documents);
            logger.DebugFormat("SearchQueryContext Returns:{0}", items.Count);
            return new DocumentResponse { Documents = items, TotalRecords = documentsInArchiveCount };
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            return new DocumentResponse { HasErros = true, Error = new ResponseError(ex) };
        }
    }

    public DocumentResponse SearchQueryContextLatestVersion(string archiveName, BindingList<DocumentCondition> attributeConditions, BindingList<DocumentCondition> contentConditions, BindingList<DocumentCondition> inferConditions, int? skip, int? take)
    {
        try
        {
            logger.DebugFormat("SearchQueryContextLatestVersion AttributeConditions:{0}", attributeConditions != null ? attributeConditions.Count : 0);
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new Archive_Exception(string.Concat("Archive: ", archiveName, " not found."));

            List<Document> documents = new List<Document>();
            int documentsInArchiveCount = 0;
            if (attributeConditions != null && attributeConditions.Count > 0)
                documents.AddRange(DocumentService.SearchDocumentsExt(archive.IdArchive, attributeConditions, out documentsInArchiveCount, skip, take));
            BindingList<Document> items = new BindingList<Document>(documents);
            logger.DebugFormat("SearchQueryContextLatestVersion Returns:{0}", items.Count);
            return new DocumentResponse { Documents = items, TotalRecords = documentsInArchiveCount };
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            return new DocumentResponse { HasErros = true, Error = new ResponseError(ex) };
        }
    }

    public BindingList<Document> GetAllDocumentChains(string archiveName, int skip, int take, out int docunentsInArchiveCount)
    {
        docunentsInArchiveCount = -1;
        try
        {
            logger.DebugFormat("GetAllDocumentChains archiveName:{0}, skip:{1}, take:{2}", archiveName, skip, take);
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archive: " + archiveName + " not found.");
            return DocumentService.GetAllDocumentChains(archive, skip, take, out docunentsInArchiveCount);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
        finally
        {
            logger.DebugFormat("GetAllDocumentChains archiveName:{0}, RETURNS: {1}", archiveName, docunentsInArchiveCount);
        }
    }

    /// <summary>
    /// Recupera dall'archivio i documenti aventi l'attributo del valore specificato.
    /// </summary>
    /// <param name="archiveName">Nome dell'archivio.</param>
    /// <param name="attributeName">Nome dell'attributo.</param>
    /// <param name="attributeValue">Valore esatto dell'attributo (case insensitive).</param>
    /// <returns></returns>
    public BindingList<Document> GetDocumentsByAttributeValue(string archiveName, string attributeName, string attributeValue)
    {
        return getDocumentsByAttributeValue(archiveName, attributeName, attributeValue, false, null, null);
    }
    /// <summary>
    /// Recupera dall'archivio i documenti aventi l'attributo del valore specificato.
    /// </summary>
    /// <param name="archiveName">Nome dell'archivio.</param>
    /// <param name="attributeName">Nome dell'attributo.</param>
    /// <param name="attributeValue">Valore esatto dell'attributo (case insensitive).</param>
    /// <param name="skip">Indice di inizio pagina.</param>
    /// <param name="take">Estensione della pagina.</param>
    /// <returns></returns>
    public BindingList<Document> GetPagingDocumentsByAttributeValue(string archiveName, string attributeName, string attributeValue, int skip, int take)
    {
        return getDocumentsByAttributeValue(archiveName, attributeName, attributeValue, false, skip, take);
    }
    /// <summary>
    /// Recupera dall'archivio i documenti aventi l'attributo contenente nel valore la stringa specificata.
    /// </summary>
    /// <param name="archiveName">Nome dell'archivio.</param>
    /// <param name="attributeName">Nome dell'attributo.</param>
    /// <param name="attributeValue">Stringa che deve essere contenuta nel valore dell'attributo (case insensitive).</param>
    /// <returns></returns>
    public BindingList<Document> GetDocumentsByAttributeValueContains(string archiveName, string attributeName, string attributeValue)
    {
        return getDocumentsByAttributeValue(archiveName, attributeName, attributeValue, true, null, null);
    }
    /// <summary>
    /// Recupera dall'archivio una pagina dei documenti aventi l'attributo contenente nel valore la stringa specificata.
    /// </summary>
    /// <param name="archiveName">Nome dell'archivio.</param>
    /// <param name="attributeName">Nome dell'attributo.</param>
    /// <param name="attributeValue">Stringa che deve essere contenuta nel valore dell'attributo (case insensitive).</param>
    /// <param name="skip">Indice di inizio della pagina.</param>
    /// <param name="take">Estensione della pagina.</param>
    /// <returns></returns>
    public BindingList<Document> GetPagingDocumentsByAttributeValueContains(string archiveName, string attributeName, string attributeValue, int skip, int take)
    {
        return getDocumentsByAttributeValue(archiveName, attributeName, attributeValue, true, skip, take);
    }

    private DocumentAttribute getAttributeByArchiveAndName(DocumentArchive archive, string attributeName)
    {
        // Recupero l'attributo a partire dal suo nome.
        BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(archive.IdArchive);
        foreach (DocumentAttribute item in attributes)
            if (item.Name.Equals(attributeName, StringComparison.InvariantCultureIgnoreCase))
                return item;

        throw new AttributeNotFoundException("Attributo \"{0}\" non configurato per l'archivio \"{1}\".",
            attributeName, archive.Name);
    }
    private BindingList<Document> getDocumentsByAttributeValue(DocumentArchive archive, DocumentAttributeValue attributeValue, bool? stringContains, int? skip, int? take)
    {
        try
        {
            logger.InfoFormat("getDocumentsByAttributeValue archive.IdArchive:{0} archive.Name:{1} attributeValue.Attribute.Name:{2} attributeValue.Value:{3}",
                archive.IdArchive, archive.Name, attributeValue.Attribute.Name, attributeValue.Value);

            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(archive.IdArchive);
            // Recupero la lista di documenti.
            BindingList<Document> documents = DocumentService.GetDocumentsByAttributeValue(archive, attributeValue, stringContains, skip, take);
            if (documents == null || documents.Count == 0)
                throw new DocumentNotFound_Exception("Nessun documento trovato con attributo \"{0}\" e valore \"{1}\".",
                    attributeValue.Attribute.Name, attributeValue.Value);

            logger.InfoFormat("getDocumentsByAttributeValue documenti trovati: {0}.", documents.Count);
            return documents;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }
    private BindingList<Document> getDocumentsByAttributeValue(string archiveName, string attributeName, string attributeValue, bool? stringContains, int? skip, int? take)
    {
        try
        {
            // Recupero l'archivio.
            DocumentArchive archive = ArchiveService.GetArchiveByName(archiveName);
            // Recupero l'attributo a partire dal suo nome.
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(archive.IdArchive);
            DocumentAttribute attribute = getAttributeByArchiveAndName(archive, attributeName);
            DocumentAttributeValue value = new DocumentAttributeValue { Attribute = attribute, Value = attributeValue };

            return getDocumentsByAttributeValue(archive, value, stringContains, skip, take);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
    }

    #endregion

    public BindingList<Document> GetAllDocuments(string archiveName, bool visible, int skip, int take, out int docunentsInArchiveCount)
    {
        docunentsInArchiveCount = -1;
        try
        {
            logger.DebugFormat("GetAllDocuments archiveName:{0}, skip:{1}, take:{2}, visible:{3}", archiveName, skip, take, visible);
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archive: " + archiveName + " not found.");
            return DocumentService.GetAllDocuments(archive, visible, skip, take, out docunentsInArchiveCount);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
        finally
        {
            logger.DebugFormat("GetAllDocuments archiveName:{0}, RETUNT: {1}", archiveName, docunentsInArchiveCount);
        }
    }


    public BindingList<Document> GetAllDocumentsWithThumbnail(string archiveName, bool visible, int skip, int take, out int docunentsInArchiveCount)
    {
        docunentsInArchiveCount = -1;
        try
        {
            logger.DebugFormat("GetAllDocuments archiveName:{0}, skip:{1}, take:{2}, visible:{3}", archiveName, skip, take, visible);
            var archive = ArchiveService.GetArchiveByName(archiveName);
            if (archive == null)
                throw new BiblosDS.Library.Common.Exceptions.Archive_Exception("Archive: " + archiveName + " not found.");
            var result = DocumentService.GetAllDocuments(archive, visible, skip, take, out docunentsInArchiveCount);

            using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName))
            {
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.IdThumbnail))
                    {
                        item.ThumbnailContent = (clientChannel as IServiceDocumentStorage).GetDocumentConformAttach(item, item.IdThumbnail).Content;
                    }
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex);
            throw;
        }
        finally
        {
            logger.DebugFormat("GetAllDocuments archiveName:{0}, RETUNT: {1}", archiveName, docunentsInArchiveCount);
        }
    }



}