using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.BiblosDS.DocumentService;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.BiblosDS
{
    [LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
    public class DocumentClient : IDocumentClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly DocumentsClient _document;
        private readonly SearchService.ContentSearchClient _contentSearch;
        private readonly SignService.ServiceDigitalSignClient _digitalSign;
        private static readonly ConcurrentDictionary<string, KeyValuePair<Archive, List<DocumentService.Attribute>>> _cache_ArchiveAttributes = new ConcurrentDictionary<string, KeyValuePair<Archive, List<DocumentService.Attribute>>>();
        private const string _biblos_attribute_filename = "filename";
        private const string _biblos_attribute_signature = "signature";
        private const string _biblos_checkout_userId = "IntegrationService";
        private ICollection<Archive> _archives = null;
        #endregion

        #region [ Properties ]
        public string SIGNATURE_ATTRIBUTE_NAME 
        {
            get
            {
                return "Signature";
            }
        }

        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DocumentClient));
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
                    _archives = _document.GetArchives();
                }
                return _archives;
            }
        }
        #endregion

        #region [ Constructor ]
        public DocumentClient(ILogger logger)
        {
            _logger = logger;
            _document = new DocumentsClient();
            _contentSearch = new SearchService.ContentSearchClient();
            _digitalSign = new SignService.ServiceDigitalSignClient();
        }
        #endregion

        #region [ Methods ]
        private List<AttributeValue> GetChainAttributes(List<DocumentService.Attribute> attributes, IDictionary<string, object> metadatas)
        {
            List<AttributeValue> attributeValues = new List<AttributeValue>();
            AttributeValue attributeValue = null;
            foreach (KeyValuePair<string, object> metadata in metadatas)
            {
                _logger.WriteDebug(new LogMessage(string.Concat("filling metadata ", metadata.Key)), LogCategories);
                attributeValue = CreateBiblosDSAttribute((x) => x.SingleOrDefault(f => f.Name.Equals(metadata.Key, StringComparison.InvariantCultureIgnoreCase) && f.AttributeGroup.GroupType == AttributeGroupType.Chain),
                        attributes, metadata.Key, metadata.Value);
                if (attributeValue != null)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("set metadata ", metadata.Key, " -> ", metadata.Value)), LogCategories);
                    attributeValues.Add(attributeValue);
                }
            }

            return attributeValues;
        }

        private List<AttributeValue> GetDocumentAttributes(List<DocumentService.Attribute> attributes, IDictionary<string, object> metadatas)
        {
            List<AttributeValue> attributeValues = new List<AttributeValue>();
            AttributeValue attributeValue = null;
            foreach (KeyValuePair<string, object> metadata in metadatas)
            {
                _logger.WriteDebug(new LogMessage(string.Concat("filling metadata ", metadata.Key)), LogCategories);
                attributeValue = CreateBiblosDSAttribute((x) => x.SingleOrDefault(f => f.Name.Equals(metadata.Key, StringComparison.InvariantCultureIgnoreCase) && f.AttributeGroup.GroupType != AttributeGroupType.Chain),
                        attributes, metadata.Key, metadata.Value);
                if (attributeValue != null)
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("set metadata ", metadata.Key, " -> ", metadata.Value)), LogCategories);
                    attributeValues.Add(attributeValue);
                }
            }

            return attributeValues;
        }

        private AttributeValue CreateBiblosDSAttribute(Func<List<DocumentService.Attribute>, DocumentService.Attribute> lambda,
            List<DocumentService.Attribute> attributes, string attributeName, object value)
        {
            DocumentService.Attribute attribute = null;
            try
            {
                attribute = lambda(attributes);
            }
            catch (Exception)
            {
                throw new ArgumentNullException(string.Concat("AttributeName '", attributeName, "' is not defined in selected archive"));
            }
            AttributeValue r_attributeValue = null;
            if (attribute != null)
            {
                if (attribute.IsRequired && value == null)
                {
                    throw new ArgumentNullException(string.Concat("Attribute '", attribute.Name, "' in archive is set to be required but value is not defined"));
                }
                if (value is DateTimeOffset)
                {
                    DateTimeOffset item = (DateTimeOffset)value;
                    value = item.DateTime;
                }
                r_attributeValue = new AttributeValue()
                {
                    Attribute = attribute,
                    Value = value
                };
            }
            return r_attributeValue;
        }

        public async Task<ArchiveDocument> InsertDocumentAsync(ArchiveDocument documentModel)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                string archiveName = documentModel.Archive.ToLower();
                Document document = new Document();
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
                if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
                {
                    Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                    if (biblosArchive == null)
                    {
                        throw new DSWException(string.Concat("Document archive '", documentModel.Archive, "' not found"), null, DSWExceptionCode.DM_Parameters);
                    }
                    List<DocumentService.Attribute> attributes = await _document.GetAttributesDefinitionAsync(biblosArchive.Name);
                    if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new DSWException(string.Concat("Archive '", documentModel.Archive, "' doesn't has attibute ", _biblos_attribute_filename, " definition"), null, DSWExceptionCode.DM_Parameters);
                    }
                    if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new DSWException(string.Concat("Archive '", documentModel.Archive, "' doesn't has attibute ", _biblos_attribute_signature, " definition"), null, DSWExceptionCode.DM_Parameters);
                    }
                    archive = new KeyValuePair<Archive, List<DocumentService.Attribute>>(biblosArchive, attributes);
                    _cache_ArchiveAttributes.TryAdd(archiveName, archive);
                }

                document.Archive = archive.Key;
                document.Name = documentModel.Name;
                document.AttributeValues = GetChainAttributes(archive.Value, documentModel.Metadata);
                document = await _document.InsertDocumentChainAsync(document);
                _logger.WriteInfo(new LogMessage(string.Concat("Document chain ", document.IdDocument, " has been created in archive ", archive.Key.Name)), LogCategories);
                documentModel.IdLegacyChain = document.IdBiblos.Value;
                documentModel.IdChain = document.IdDocument;
                document = new Document
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

                KeyValuePair<string, object> signatureValue = documentModel.Metadata.SingleOrDefault(f => f.Key.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase));
                document.AttributeValues.Add(new AttributeValue()
                {
                    Value = signatureValue.Value ?? documentModel.Name,
                    Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase))
                });
                document = await _document.AddDocumentToChainAsync(document, documentModel.IdChain, ContentFormat.Binary);
                _logger.WriteInfo(new LogMessage(string.Concat("Document ", document.IdDocument, " has been created in archive ", archive.Key.Name)), LogCategories);
                documentModel.IdDocument = document.IdDocument;
                documentModel.Size = document.Size;
                documentModel.Version = document.Version;
                return documentModel;
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ArchiveDocument>> InsertDocumentsAsync(ICollection<ArchiveDocument> documentModels)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                string archiveName = documentModels.First().Archive.ToLower();
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
                if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
                {
                    Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                    if (biblosArchive == null)
                    {
                        throw new DSWException(string.Concat("Document archive '", archiveName, "' not found"), null, DSWExceptionCode.DM_Parameters);
                    }
                    List<DocumentService.Attribute> attributes = await _document.GetAttributesDefinitionAsync(biblosArchive.Name);
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

                Document chainDocument = new Document();
                Document document;
                chainDocument.Archive = archive.Key;
                //document.Name = documentModel.Name;
                chainDocument = await _document.InsertDocumentChainAsync(chainDocument);
                _logger.WriteInfo(new LogMessage(string.Concat("Document chain ", chainDocument.IdDocument, " has been created in archive ", archive.Key.Name)), LogCategories);
                foreach (ArchiveDocument archiveDocument in documentModels)
                {
                    archiveDocument.IdLegacyChain = chainDocument.IdBiblos.Value;
                    archiveDocument.IdChain = chainDocument.IdDocument;
                    document = new Document
                    {
                        Content = new Content() { Blob = archiveDocument.ContentStream },
                        Name = archiveDocument.Name,
                        Archive = Archives.Single(f => f.Name.Equals(archiveDocument.Archive, StringComparison.InvariantCultureIgnoreCase)),
                        AttributeValues = new List<AttributeValue>()
                    };
                    document.AttributeValues.Add(new AttributeValue()
                    {
                        Value = archiveDocument.Name,
                        Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase))
                    });

                    KeyValuePair<string, object> signatureValue = archiveDocument.Metadata.SingleOrDefault(f => f.Key.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase));
                    document.AttributeValues.Add(new AttributeValue()
                    {
                        Value = signatureValue.Value ?? archiveDocument.Name,
                        Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase))
                    });
                    document = await _document.AddDocumentToChainAsync(document, archiveDocument.IdChain, ContentFormat.Binary);
                    _logger.WriteInfo(new LogMessage(string.Concat("Document ", document.IdDocument, " has been created in archive ", archive.Key.Name)), LogCategories);
                    archiveDocument.IdDocument = document.IdDocument;
                    archiveDocument.Size = document.Size;
                    archiveDocument.Version = document.Version;
                }
                return documentModels;
            }, _logger, LogCategories);
        }

        public async Task<byte[]> GetDocumentStreamAsync(Guid idDocument)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Content content = await _document.GetDocumentContentByIdAsync(idDocument);
                return content.Blob;
            }, _logger, LogCategories);
        }

        public async Task DetachDocumentAsync(Guid documentId)
        {
            await DocumentHelper.TryCatchWithLogger(async () =>
            {
                _logger.WriteInfo(new LogMessage(string.Concat("DetachDocumentAsync -> Detaching document ", documentId)), LogCategories);
                Document document = await _document.GetDocumentInfoByIdAsync(documentId);
                if (document == null)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("DetachDocumentAsync -> Document ", documentId, " not found")), LogCategories);
                }
                await _document.DocumentDetachAsync(document);
                _logger.WriteInfo(new LogMessage(string.Concat("DetachDocumentAsync -> Document ", documentId, " detached")), LogCategories);
            }, _logger, LogCategories);
        }

        public async Task<Guid> UpdateChainAsync(string archiveName, Guid chainId, IDictionary<string, object> metadatas)
        {
            try
            {
                Document document = new Document();
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
                if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
                {
                    Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                    if (biblosArchive == null)
                    {
                        throw new DSWException(string.Concat("Document archive '", archiveName, "' not found"), null, DSWExceptionCode.DM_Parameters);
                    }
                    List<DocumentService.Attribute> attributes = await _document.GetAttributesDefinitionAsync(biblosArchive.Name);
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

                Document checkedout = await _document.DocumentCheckOutAsync(chainId, true, _biblos_checkout_userId);
                checkedout.AttributeValues = GetChainAttributes(archive.Value, metadatas);

                Guid savedId = await _document.DocumentCheckInAsync(checkedout, _biblos_checkout_userId);
                _logger.WriteInfo(new LogMessage(string.Concat("Document chain ", chainId, " update correctly")), LogCategories);
                await _document.ConfirmDocumentAsync(checkedout.IdDocument);
                return savedId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                await _document.UndoCheckOutDocumentAsync(chainId, _biblos_checkout_userId);
                throw new DSWException(string.Concat("Document BiblosDS layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public async Task<Guid> UpdateDocumentAsync(string archiveName, Guid documentId, IDictionary<string, object> metadatas)
        {
            try
            {
                Document document = new Document();
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
                if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
                {
                    Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                    if (biblosArchive == null)
                    {
                        throw new DSWException(string.Concat("Document archive '", archiveName, "' not found"), null, DSWExceptionCode.DM_Parameters);
                    }
                    List<DocumentService.Attribute> attributes = await _document.GetAttributesDefinitionAsync(biblosArchive.Name);
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

                Document checkedout = await _document.DocumentCheckOutAsync(documentId, true, _biblos_checkout_userId);
                checkedout.AttributeValues = GetDocumentAttributes(archive.Value, metadatas);

                Guid savedId = await _document.DocumentCheckInAsync(checkedout, _biblos_checkout_userId);
                _logger.WriteInfo(new LogMessage(string.Concat("Document ", documentId, " update correctly")), LogCategories);
                await _document.ConfirmDocumentAsync(checkedout.IdDocument);
                return savedId;
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                await _document.UndoCheckOutDocumentAsync(documentId, _biblos_checkout_userId);
                throw new DSWException(string.Concat("Document BiblosDS layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public async Task<ICollection<ArchiveDocument>> FindDocumentsAsync(ICollection<FinderCondition> conditions)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                IList<SearchService.Condition> biblosConditions = new List<SearchService.Condition>();
                foreach (FinderCondition condition in conditions)
                {
                    SearchService.Condition biblosCondition = new SearchService.Condition()
                    {
                        Name = condition.Name,
                        Value = condition.Value
                    };

                    switch (condition.Operator)
                    {
                        case SearchConditionOperator.Contains:
                            biblosCondition.Operator = SearchService.FilterOperator.Contains;
                            break;
                        case SearchConditionOperator.IsEqualTo:
                            biblosCondition.Operator = SearchService.FilterOperator.IsEqualTo;
                            break;
                        case SearchConditionOperator.IsNullOrEmpty:
                            biblosCondition.Operator = SearchService.FilterOperator.IsNullOrEmpty;
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("Operator [{0}] not implemented.", condition.Operator));
                    }
                    biblosConditions.Add(biblosCondition);
                }
                SearchService.Document[] documents = await _contentSearch.SearchQueryLatestVersionAsync(biblosConditions.ToArray(), null, null);
                ICollection<ArchiveDocument> wrappedDocuments = new List<ArchiveDocument>();
                foreach (SearchService.Document document in documents)
                {
                    ArchiveDocument wrappedDocument = new ArchiveDocument()
                    {
                        Archive = document.Archive.Name,
                        IdChain = document.DocumentParent.IdDocument,
                        IdDocument = document.IdDocument,
                        Name = document.Name,
                        Size = document.Size,
                        Version = document.Version,
                        Metadata = document.AttributeValues.ToDictionary(d => d.Attribute.Name, d => d.Value)
                    };
                    wrappedDocuments.Add(wrappedDocument);
                }
                return wrappedDocuments;
            }, _logger, LogCategories);
        }

        public async Task<ArchiveDocument> AddDocumentToChainAsync(Guid idChain, ArchiveDocument documentModel)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                string archiveName = documentModel.Archive.ToLower();
                Document document = new Document();
                KeyValuePair<Archive, List<DocumentService.Attribute>> archive;
                if (!_cache_ArchiveAttributes.ContainsKey(archiveName) || !_cache_ArchiveAttributes.TryGetValue(archiveName, out archive))
                {
                    Archive biblosArchive = Archives.SingleOrDefault(f => f.Name.Equals(archiveName, StringComparison.InvariantCultureIgnoreCase));
                    if (biblosArchive == null)
                    {
                        throw new DSWException(string.Concat("Document archive '", documentModel.Archive, "' not found"), null, DSWExceptionCode.DM_Parameters);
                    }
                    List<DocumentService.Attribute> attributes = await _document.GetAttributesDefinitionAsync(biblosArchive.Name);
                    if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_filename, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new DSWException(string.Concat("Archive '", documentModel.Archive, "' doesn't has attibute ", _biblos_attribute_filename, " definition"), null, DSWExceptionCode.DM_Parameters);
                    }
                    if (!attributes.Any(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new DSWException(string.Concat("Archive '", documentModel.Archive, "' doesn't has attibute ", _biblos_attribute_signature, " definition"), null, DSWExceptionCode.DM_Parameters);
                    }
                    archive = new KeyValuePair<Archive, List<DocumentService.Attribute>>(biblosArchive, attributes);
                    _cache_ArchiveAttributes.TryAdd(archiveName, archive);
                }

                document = new Document
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

                KeyValuePair<string, object> signatureValue = documentModel.Metadata.SingleOrDefault(f => f.Key.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase));
                document.AttributeValues.Add(new AttributeValue()
                {
                    Value = signatureValue.Value == null ? documentModel.Name : signatureValue.Value,
                    Attribute = archive.Value.Single(f => f.Name.Equals(_biblos_attribute_signature, StringComparison.InvariantCultureIgnoreCase))
                });
                document = await _document.AddDocumentToChainAsync(document, idChain, ContentFormat.Binary);
                _logger.WriteInfo(new LogMessage(string.Concat("Document ", document.IdDocument, " has been created in archive ", archive.Key.Name)), LogCategories);
                documentModel.IdDocument = document.IdDocument;
                documentModel.Size = document.Size;
                documentModel.Version = document.Version;
                documentModel.IdChain = document.DocumentParent.IdDocument;
                return documentModel;
            }, _logger, LogCategories);
        }

        public async Task AddDocumentToDocumentUnitAsync(ArchiveDocument document, Guid idDocumentUnit)
        {
            await DocumentHelper.TryCatchWithLogger(async () =>
            {
                List<DocumentUnitChain> documentUnitChains = new List<DocumentUnitChain>()
                {
                    new DocumentUnitChain()
                    {
                        IdParentBiblos = document.IdDocument,
                        IdDocumentUnit = idDocumentUnit,
                        Name = document.Name
                    }
                };
                await _document.UdsDocumentUnitAddDocumentsAsync(idDocumentUnit, documentUnitChains);
            }, _logger, LogCategories);
        }

        public async Task<bool> IsSignedAsync(byte[] source)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                SignService.Content content = new SignService.Content() { Blob = source };
                bool isSigned = await _digitalSign.IsBlobSignedAsync(content);
                return isSigned;
            }, _logger, LogCategories);
        }

        public async Task<ArchiveDocument> GetInfoDocumentAsync(Guid documentId)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Document document = await _document.GetDocumentInfoByIdAsync(documentId);
                return new ArchiveDocument()
                {
                    Archive = document.Archive.Name,
                    IdChain = document.DocumentParent?.IdDocument,
                    IdDocument = document.IdDocument,
                    Name = document.Name,
                    Size = document.Size,
                    Version = document.Version,
                    Metadata = document.AttributeValues.ToDictionary(d => d.Attribute.Name, d => d.Value)
                };
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ArchiveDocument>> GetChildrenAsync(Guid parentId)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                ICollection<ArchiveDocument> documents = new List<ArchiveDocument>();
                ICollection<Document> childrenDocuments = await _document.GetDocumentChildrenAsync(parentId);
                foreach (Document childDocument in childrenDocuments)
                {
                    documents.Add(new ArchiveDocument()
                    {
                        Archive = childDocument.Archive.Name,
                        IdChain = childDocument.DocumentParent.IdDocument,
                        IdDocument = childDocument.IdDocument,
                        Name = childDocument.Name,
                        Size = childDocument.Size,
                        Version = childDocument.Version,
                        Metadata = childDocument.AttributeValues.ToDictionary(d => d.Attribute.Name, d => d.Value)
                    });
                }
                return documents;
            }, _logger, LogCategories);
        }

        public async Task RemoveFullTextDataAsync(Guid chainId)
        {
            await DocumentHelper.TryCatchWithLogger(async () =>
            {
                await _document.RemoveFullTextDataForChainAsync(chainId);
            }, _logger, LogCategories);
        }

        public async Task AlignFullTextDataAsync(Guid chainId)
        {
            await DocumentHelper.TryCatchWithLogger(async () =>
            {
                await _document.AlignFullTextDataForChainAsync(chainId);
            }, _logger, LogCategories);
        }


        public async Task<List<Document>> GetDocumentChildrenAsync(Guid idChain)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                List<Document> document = await _document.GetDocumentChildrenAsync(idChain);
                return document;
            }, _logger, LogCategories);
        }

        public async Task<Content> GetDocumentContentByIdAsync(Guid idDocument)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Content documentContent = await _document.GetDocumentContentByIdAsync(idDocument);
                return documentContent;
            }, _logger, LogCategories);
        }

        public async Task<Document> AddDocumentToChainAsync(Document document)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                Document savedDocument = await _document.AddDocumentToChainAsync(document, null, ContentFormat.Binary);
                return savedDocument;
            }, _logger, LogCategories);
        }

        public async Task<IList<Archive>> GetArchivesAsync()
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                IList<Archive> results = await _document.GetArchivesAsync();
                return results;
            }, _logger, LogCategories);
        }

        public async Task<IList<DocumentService.Attribute>> GetAttributesDefinitionAsync(string archiveName)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                IList<DocumentService.Attribute> results = await _document.GetAttributesDefinitionAsync(archiveName);
                return results;
            }, _logger, LogCategories);
        }
        public async Task<IList<DocumentSignInfo>> GetDocumentSignInfoAsync(Guid idDocument)
        {
            return await DocumentHelper.TryCatchWithLogger(async () =>
            {
                IList<DocumentSignInfo> results = await _document.GetDocumentSignInfoAsync(idDocument);
                return results;
            }, _logger, LogCategories);
        }

        #endregion
    }
}
