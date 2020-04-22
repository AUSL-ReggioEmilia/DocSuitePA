using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Commons
{
    [EnableQuery]
    public class DocumentUnitsController : BaseODataController<DocumentModel, DocumentUnit>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Constructor ]

        public DocumentUnitsController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
            : base(unitOfWork, logger, mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _documentService = documentService;
        }

        #endregion

        #region [ Methods ]

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public async Task<IHttpActionResult> GetDocuments(Guid uniqueId, Guid? workflowArchiveChainId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                DocumentUnit documentUnit = _unitOfWork.Repository<DocumentUnit>().GetById(uniqueId).SingleOrDefault();
                if (documentUnit == null)
                {
                    throw new ArgumentNullException("Document unit not found");
                }
                IList<DocumentModel> documents = new List<DocumentModel>();
                foreach (DocumentUnitChain documentUnitChain in documentUnit.DocumentUnitChains)
                {
                    foreach (ModelDocument.Document item in await _documentService.GetDocumentsFromChainAsync(documentUnitChain.IdArchiveChain))
                    {
                        documents.Add(new DocumentModel(item.IdDocument, item.Name)
                        {
                            DocumentType = documentUnitChain.ChainType == ChainType.MainChain ? DocumentType.Main : documentUnitChain.ChainType == ChainType.AttachmentsChain ? DocumentType.Attachment : DocumentType.Annexed,
                            ChainId = item.IdChain.Value,
                            ArchiveSection = documentUnitChain.DocumentLabel
                        });
                    }
                }
                if (workflowArchiveChainId.HasValue)
                {
                    foreach (ModelDocument.Document item in await _documentService.GetDocumentsFromChainAsync(workflowArchiveChainId.Value))
                    {
                        documents.Add(new DocumentModel(item.IdDocument, item.Name)
                        {
                            DocumentType = DocumentType.Miscellanea,
                            ChainId = workflowArchiveChainId.Value,
                            ArchiveSection = "Miscellanea"
                        });
                    }
                }
                return Ok(documents.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public async Task<IHttpActionResult> GetDocumentsByArchiveChain(Guid idArchiveChain)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                IList<DocumentModel> documents = new List<DocumentModel>();
                ICollection<ModelDocument.Document> results = await _documentService.GetDocumentsFromChainAsync(idArchiveChain);
                if (results.Any())
                {
                    int tollerance = 10000;
                    DateTime lastDate = results.First().CreatedDate.Value;
                    foreach (ModelDocument.Document item in results)
                    {
                        documents.Add(new DocumentModel(item.IdDocument, item.Name)
                        {
                            DocumentType = (item.CreatedDate.Value - lastDate).TotalMilliseconds > tollerance ? DocumentType.Miscellanea : DocumentType.Main,
                            ChainId = item.IdChain.Value,
                            ArchiveSection = "Attività"
                        });
                        lastDate = item.CreatedDate.Value;
                    }
                }
                return Ok(documents.AsQueryable());
            }, _logger, LogCategories);
        }

        #endregion
    }
}
