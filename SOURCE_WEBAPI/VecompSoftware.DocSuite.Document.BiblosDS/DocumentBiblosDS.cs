using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.BiblosDS.DocumentService;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Document.BiblosDS
{
    [LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
    public class DocumentBiblosDS : IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument>
    {
        #region [ Fields ]

        private readonly Guid _instanceId;
        private readonly ILogger _logger;
        private readonly DocumentsClient _documentsClient;
        private ICollection<Archive> _archives = null;
        private bool _disposed;
        private static IEnumerable<LogCategory> _logCategories = null;
        private static readonly ConcurrentDictionary<string, KeyValuePair<Archive, List<DocumentService.Attribute>>> _cache_ArchiveAttributes = new ConcurrentDictionary<string, KeyValuePair<Archive, List<DocumentService.Attribute>>>();
        private const string _biblos_attribute_filename = "filename";
        private const string _biblos_attribute_signature = "signature";
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DocumentBiblosDS));
                }
                return _logCategories;
            }
        }

        protected ICollection<Archive> Archives
        {
            get
            {
                if (_archives == null)
                {
                    _archives = _documentsClient.GetArchives();
                }
                return _archives;
            }
        }
        #endregion

        #region [ Constructor ]
        public DocumentBiblosDS(ILogger logger)
        {
            _instanceId = Guid.NewGuid();
            _logger = logger;
            _documentsClient = new DocumentsClient();
        }

        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only

            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

        #endregion Dispose

        #region [ Methods ]
        public async Task<ICollection<ModelDocument.Document>> GetDocumentsFromChainAsync(Guid idChain)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                List<DocumentService.Document> documents = await _documentsClient.GetDocumentChildrenAsync(idChain);
                List<ModelDocument.Document> results = new List<ModelDocument.Document>(documents.Count);
                results.AddRange(documents.Select(doc => new ModelDocument.Document()
                {
                    IdChain = idChain,
                    IdDocument = doc.IdDocument,
                    Name = doc.Name,
                    Size = doc.Size,
                    Version = doc.Version,
                    CreatedDate = doc.DateCreated
                }));
                return results;
            }, _logger, LogCategories);
        }

        public async Task<IEnumerable<ModelDocument.Document>> GetDocumentLatestVersionFromChainAsync(Guid idChain)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                List<DocumentService.Document> documents = await _documentsClient.GetDocumentChildrenAsync(idChain);
                if (documents.Count == 0)
                {
                    return null;
                }

                return documents.Where(d => d.IsLatestVersion == true).Select(x => new ModelDocument.Document()
                {
                    IdChain = idChain,
                    IdDocument = x.IdDocument,
                    Name = x.Name,
                    Size = x.Size,
                    Version = x.Version,
                    CreatedDate = x.DateCreated
                });
            }, _logger, LogCategories) ?? new List<ModelDocument.Document>();
        }

        public async Task<byte[]> GetDocumentContentAsync(Guid idDocument)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Content document = await _documentsClient.GetDocumentContentByIdAsync(idDocument);
                return document.Blob;
            }, _logger, LogCategories);
        }

        public async Task<ModelDocument.ArchiveDocument> InsertDocumentAsync(ModelDocument.ArchiveDocument documentModel)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                DocumentService.Document document = new DocumentService.Document();
                string archiveName = documentModel.Archive;
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive = await GetArchive(archiveName);

                document.Archive = archive.Key;
                document.Name = documentModel.Name;
                document = await _documentsClient.InsertDocumentChainAsync(document);
                _logger.WriteInfo(new LogMessage($"Document chain {document.IdDocument} has been successfully created in archive {archive.Key.Name}"), LogCategories);
                documentModel.IdLegacyChain = document.IdBiblos.Value;
                documentModel.IdChain = document.IdDocument;
                document = await InsertDocument(documentModel, archive);
                documentModel.IdDocument = document.IdDocument;
                documentModel.Size = document.Size;
                documentModel.Version = document.Version;
                return documentModel;
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ModelDocument.ArchiveDocument>> InsertDocumentsAsync(ICollection<ModelDocument.ArchiveDocument> documents, Guid? idChain = null)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                DocumentService.Document documentChain;
                DocumentService.Document document = null;
                string archiveName = documents.First().Archive;
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive = await GetArchive(archiveName);
                if (!idChain.HasValue)
                {
                    documentChain = new DocumentService.Document
                    {
                        Archive = archive.Key
                    };
                    documentChain = await _documentsClient.InsertDocumentChainAsync(documentChain);
                    _logger.WriteInfo(new LogMessage($"Document chain {documentChain.IdDocument} has been successfully created in archive {archive.Key.Name}"), LogCategories);
                }
                else
                {
                    documentChain = await _documentsClient.GetDocumentInfoByIdAsync(idChain.Value);
                }
                foreach (ModelDocument.ArchiveDocument documentModel in documents)
                {
                    documentModel.IdLegacyChain = documentChain.IdBiblos.Value;
                    documentModel.IdChain = documentChain.IdDocument;
                    document = await InsertDocument(documentModel, archive);
                    documentModel.IdDocument = document.IdDocument;
                    documentModel.Size = document.Size;
                    documentModel.Version = document.Version;
                }
                return documents;
            }, _logger, LogCategories);
        }

        public async Task<bool> HasActiveDocumentsAsync(Guid idChain)
        {
            bool hasActiveDocument = await _documentsClient.HasActiveDocumentsAsync(idChain);
            return hasActiveDocument;
        }


        public bool HasActiveDocuments(Guid idChain)
        {
            bool hasActiveDocument =  _documentsClient.HasActiveDocuments(idChain);
            return hasActiveDocument;
        }


        private async Task<DocumentService.Document> InsertDocument(ModelDocument.ArchiveDocument documentModel, KeyValuePair<Archive, List<DocumentService.Attribute>> archive)
        {
            DocumentService.Document document = new DocumentService.Document
            {
                Content = new Content() { Blob = documentModel.ContentStream },
                Name = documentModel.Name,
                Archive = Archives.Single(f => f.Name.Equals(documentModel.Archive, StringComparison.InvariantCultureIgnoreCase)),
                AttributeValues = new List<AttributeValue>()
            };
            document.AttributeValues.Add(new AttributeValue()
            {
                Value = documentModel.Name,
                Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase))
            });
            document.AttributeValues.Add(new AttributeValue()
            {
                Value = documentModel.Name,
                Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase))
            });
            document = await _documentsClient.AddDocumentToChainAsync(document, documentModel.IdChain, ContentFormat.Binary);
            _logger.WriteInfo(new LogMessage($"Document {document.IdDocument} has been successfully created in archive {archive.Key.Name}"), LogCategories);
            return document;
        }

        private async Task<KeyValuePair<Archive, List<DocumentService.Attribute>>> GetArchive(string archiveName)
        {
            KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
            if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
            {
                Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                if (biblosArchive == null)
                {
                    throw new DSWException(string.Concat("Document archive '", archiveName, "' not found"), null, DSWExceptionCode.DM_Parameters);
                }
                List<DocumentService.Attribute> attributes = await _documentsClient.GetAttributesDefinitionAsync(biblosArchive.Name);
                if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new DSWException(string.Concat("Archive '", archiveName, "' doesn't has attibute ", _biblos_attribute_filename, " definition"), null, DSWExceptionCode.DM_Parameters);
                }
                if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new DSWException(string.Concat("Archive '", archiveName, "' doesn't has attibute ", _biblos_attribute_signature, " definition"), null, DSWExceptionCode.DM_Parameters);
                }
                archive = new KeyValuePair<Archive, List<DocumentService.Attribute>>(biblosArchive, attributes);
                _cache_ArchiveAttributes.TryAdd(archiveName, archive);
            }

            return archive;
        }

        public async Task<Guid> GetDocumentIdAsync(string archiveName, int documentId)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Guid idChain = await _documentsClient.GetDocumentIdAsync(archiveName, documentId);
                return idChain;
            }, _logger, LogCategories);
        }

        #endregion
    }
}
