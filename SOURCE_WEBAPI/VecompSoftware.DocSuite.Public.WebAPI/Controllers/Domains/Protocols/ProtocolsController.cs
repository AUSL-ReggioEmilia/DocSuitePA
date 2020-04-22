using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Public.Core.Models.Domains;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;
using VecompSoftware.DocSuite.Public.Core.Models.Helpers.ExtensionMethods;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using PrivateCommonModels = VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Protocols
{
    [EnableQuery]
    [Authorize]
    public class ProtocolsController : BaseODataController<ProtocolModel, Protocol>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        #endregion

        #region [ Constructor ]

        public ProtocolsController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
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
        [DSWAuthorize(DSWAuthorizeType.OData, DSWAuthorizeClaimType.Protocol, new string[] { DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Year, DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Number })]
        public IHttpActionResult GetProtocolSummary(short year, int number)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                Protocol protocol = _unitOfWork.Repository<Protocol>().GetByCompositeKey(year, number).SingleOrDefault();
                if (protocol == null)
                {
                    throw new ArgumentNullException("Protocol not found");
                }
                ProtocolModel model = _mapper.Map<Protocol, ProtocolModel>(protocol);

                ICollection<PrivateCommonModels.RoleTableValuedModel> sectors = _unitOfWork.Repository<ProtocolRole>().Get(year, number);
                List<ProtocolSectorModel> sectorModels = _mapper.Map<ICollection<PrivateCommonModels.RoleTableValuedModel>, ICollection<ProtocolSectorModel>>(sectors).ToList();
                model.Sectors = sectorModels.BuildHierarchical();

                ICollection<PrivateCommonModels.ContactTableValuedModel> contacts = _unitOfWork.Repository<ProtocolContact>().Get(year, number);
                List<ProtocolContactModel> contactModels = _mapper.Map<ICollection<PrivateCommonModels.ContactTableValuedModel>, ICollection<ProtocolContactModel>>(contacts).ToList();
                model.Contacts = contactModels.BuildHierarchical();

                return Ok(new List<ProtocolModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        [DSWAuthorize(DSWAuthorizeType.OData, DSWAuthorizeClaimType.Protocol, new string[] { DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_UniqueId })]
        public async Task<IHttpActionResult> GetProtocolDocuments(Guid uniqueId)
        {
            return await ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                DocumentUnit documentUnit = _unitOfWork.Repository<DocumentUnit>().GetByNumbering(uniqueId, (int)DSWEnvironmentType.Protocol).SingleOrDefault();
                if (documentUnit == null)
                {
                    throw new ArgumentNullException("Protocol not found");
                }
                ProtocolModel model = _mapper.Map<Protocol, ProtocolModel>(new Protocol()
                {
                    UniqueId = uniqueId,
                    Year = documentUnit.Year,
                    Number = documentUnit.Number,
                    Object = documentUnit.Subject,
                    Category = new Category(),
                    Container = new Container()
                });

                foreach (DocumentUnitChain documentUnitChain in documentUnit.DocumentUnitChains)
                {
                    foreach (ModelDocument.Document item in await _documentService.GetDocumentsFromChainAsync(documentUnitChain.IdArchiveChain))
                    {
                        model.Documents.Add(new DocumentModel(item.IdDocument, item.Name)
                        {
                            DocumentType = documentUnitChain.ChainType == ChainType.MainChain ? DocumentType.Main : documentUnitChain.ChainType == ChainType.AttachmentsChain ? DocumentType.Attachment : DocumentType.Annexed,
                            ChainId = item.IdChain.Value
                        });
                    }
                }
                return Ok(new List<ProtocolModel>() { model }.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        [DSWAuthorize(DSWAuthorizeType.OData, DSWAuthorizeClaimType.Protocol, new string[] { DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Year, DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Number })]
        public IHttpActionResult GetProtocolOutgoingPECCount(short year, int number)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int pecCount = _unitOfWork.Repository<PECMail>().CountOutgoing(year, number);
                return Ok(pecCount);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        [DSWAuthorize(DSWAuthorizeType.OData, DSWAuthorizeClaimType.Protocol, new string[] { DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Year, DSWAuthorizationServerProvider.CLAIM_ExternalViewer_OAuth2_Number })]
        public IHttpActionResult GetProtocolIngoingPECCount(short year, int number)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int pecCount = _unitOfWork.Repository<PECMail>().CountIncoming(year, number);
                return Ok(pecCount);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ByContacts(string searchCode, DateTimeOffset? dateFrom, DateTimeOffset? dateTo)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<Protocol> finderResults = _unitOfWork.Repository<Protocol>().GetByContacts(searchCode, dateFrom, dateTo);
                ICollection<DocumentUnitReferenceModel> results = _mapper.Map<ICollection<Protocol>, ICollection<DocumentUnitReferenceModel>>(finderResults.ToList()).ToList();
                return Ok(results);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
