using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using PrivateCommonModels = VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Fascicles
{
    [EnableQuery]
    public class FasciclesController : BaseODataController<FascicleModel, Fascicle>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Constructor ]

        public FasciclesController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
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
        public IHttpActionResult GetFascicleSummary(Guid uniqueId)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                Fascicle fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(uniqueId);
                if (fascicle == null)
                {
                    throw new ArgumentNullException("Fascicle not found");
                }

                FascicleModel model = _mapper.Map<Fascicle, FascicleModel>(fascicle);
                ICollection<PrivateCommonModels.ContactTableValuedModel> contacts = _unitOfWork.Repository<Fascicle>().GetContacts(fascicle);
                List<FascicleContactModel> contactModels = _mapper.Map<ICollection<PrivateCommonModels.ContactTableValuedModel>, ICollection<FascicleContactModel>>(contacts).ToList();
                model.Contacts = contactModels;

                FascicleFolder internetFolder = _unitOfWork.Repository<FascicleFolder>().GetInternetFolderByFascicle(uniqueId).SingleOrDefault();
                if (internetFolder != null)
                {
                    ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetFascicleDocumentUnitsPrivate(fascicle, internetFolder.UniqueId);
                    ICollection<GenericDocumentUnitModel> genericDocumentUnits = _mapper.Map<ICollection<DocumentUnitTableValuedModel>, ICollection<GenericDocumentUnitModel>>(documentUnits).ToList();
                    model.DocumentUnits = genericDocumentUnits;
                }

                return Ok(new List<FascicleModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetFascicleSummaryByTitle(string title)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                Fascicle fascicle = _unitOfWork.Repository<Fascicle>().GetByTitle(title);
                if (fascicle == null)
                {
                    throw new ArgumentNullException("Fascicle not found");
                }

                FascicleModel model = _mapper.Map<Fascicle, FascicleModel>(fascicle);
                ICollection<PrivateCommonModels.ContactTableValuedModel> contacts = _unitOfWork.Repository<Fascicle>().GetContacts(fascicle);
                List<FascicleContactModel> contactModels = _mapper.Map<ICollection<PrivateCommonModels.ContactTableValuedModel>, ICollection<FascicleContactModel>>(contacts).ToList();
                model.Contacts = contactModels;

                return Ok(new List<FascicleModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetFascicleDocumentUnits(Guid uniqueId, string filter)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                Fascicle fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(uniqueId);
                if (fascicle == null)
                {
                    throw new ArgumentNullException("Fascicle not found");
                }

                FascicleModel model = _mapper.Map<Fascicle, FascicleModel>(fascicle);

                ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetFascicleDocumentUnitsPublic(fascicle, filter);
                ICollection<GenericDocumentUnitModel> genericDocumentUnits = _mapper.Map<ICollection<DocumentUnitTableValuedModel>, ICollection<GenericDocumentUnitModel>>(documentUnits).ToList();
                model.DocumentUnits = genericDocumentUnits;

                return Ok(new List<FascicleModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public async Task<IHttpActionResult> GetFascicleFlatDocuments(Guid uniqueId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                FascicleFolder internetFolder = _unitOfWork.Repository<FascicleFolder>().GetInternetFolderByFascicle(uniqueId).SingleOrDefault();
                if (internetFolder == null)
                {
                    return Ok(new List<FascicleModel>() { }.AsQueryable());
                }

                ICollection<FascicleDocument> miscellaneas = _unitOfWork.Repository<FascicleDocument>().GetByIdFascicleFolder(internetFolder.UniqueId, true).ToList();
                ICollection<FascicleDocumentUnit> documentUnits = _unitOfWork.Repository<FascicleDocumentUnit>().GetByIdFascicleFolder(internetFolder.UniqueId, true).ToList();
                ICollection<DocumentUnitChain> documentUnitChains = documentUnits.SelectMany(s => _unitOfWork.Repository<DocumentUnitChain>().GetByDocumentUnit(s.DocumentUnit, true)).ToList();
                ICollection<Guid> chains = miscellaneas.Select(s => s.IdArchiveChain).Union(documentUnitChains.Select(s => s.IdArchiveChain)).ToList();

                FascicleModel model = _mapper.Map<Fascicle, FascicleModel>(internetFolder.Fascicle);
                IList<DocumentModel> documents = new List<DocumentModel>();
                ICollection<ModelDocument.Document> biblosDocuments;
                foreach (Guid archiveChain in chains)
                {
                    biblosDocuments = await _documentService.GetDocumentsFromChainAsync(archiveChain);
                    foreach (ModelDocument.Document biblosDocument in biblosDocuments)
                    {
                        documents.Add(new DocumentModel(biblosDocument.IdDocument, biblosDocument.Name)
                        {
                            DocumentType = DocumentType.Miscellanea,
                            ChainId = biblosDocument.IdChain.Value
                        });
                    }
                }

                model.Documents = documents;

                return Ok(new List<FascicleModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetFasciclesByMetadataIdentifier(string name, string identifier)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<Fascicle> fascicles = _unitOfWork.Repository<Fascicle>().GetByMetadataIdentifier(name, identifier).ToList();                
                ICollection<FascicleModel> models = _mapper.Map<ICollection<Fascicle>, ICollection<FascicleModel>>(fascicles);

                return Ok(models.AsQueryable());
            }, _logger, LogCategories);
        }
        #endregion
    }
}
