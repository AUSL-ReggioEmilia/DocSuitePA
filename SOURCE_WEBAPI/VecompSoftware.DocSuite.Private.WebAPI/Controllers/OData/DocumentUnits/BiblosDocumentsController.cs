using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentUnits
{
    [EnableQuery]
    public class BiblosDocumentsController : ODataController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(BiblosDocumentsController));
                }
                return _logCategories;
            }
        }
        #endregion

        public BiblosDocumentsController(IDataUnitOfWork unitOfWork, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
        {
            _unitOfWork = unitOfWork;
            _documentService = documentService;
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public async Task<IHttpActionResult> GetBiblosDocuments(Guid uniqueId, Guid? workflowArchiveChainId)
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
                        documents.Add(new DocumentModel
                        {
                            DocumentId = item.IdDocument,
                            FileName = item.Name,
                            ChainType = (DocSuiteWeb.Model.Entities.DocumentUnits.ChainType) documentUnitChain.ChainType,
                            ChainId = item.IdChain.Value,
                            ArchiveSection = documentUnitChain.DocumentLabel
                        });
                    }
                }
                if (workflowArchiveChainId.HasValue)
                {
                    foreach (ModelDocument.Document item in await _documentService.GetDocumentsFromChainAsync(workflowArchiveChainId.Value))
                    {
                        documents.Add(new DocumentModel
                        {
                            ChainType = (DocSuiteWeb.Model.Entities.DocumentUnits.ChainType)ChainType.Miscellanea,
                            DocumentId = item.IdDocument,
                            FileName = item.Name,
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
                        documents.Add(new DocumentModel
                        {
                            DocumentId = item.IdDocument,
                            FileName = item.Name,
                            ChainType = (item.CreatedDate.Value - lastDate).TotalMilliseconds > tollerance ? (DocSuiteWeb.Model.Entities.DocumentUnits.ChainType)ChainType.Miscellanea : (DocSuiteWeb.Model.Entities.DocumentUnits.ChainType)ChainType.MainChain,
                            ChainId = item.IdChain.Value,
                            ArchiveSection = "Attività"
                        });
                        lastDate = item.CreatedDate.Value;
                    }
                }
                return Ok(documents.AsQueryable());
            }, _logger, LogCategories);
        }
    }
}