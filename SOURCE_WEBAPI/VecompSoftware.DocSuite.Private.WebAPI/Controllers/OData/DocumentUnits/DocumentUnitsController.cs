using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document;
using VecompSoftware.DocSuite.Document.BiblosDS;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using ModelDocuments = VecompSoftware.DocSuiteWeb.Model.Documents;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentUnits
{
    public class DocumentUnitsController : BaseODataController<DocumentUnit, IDocumentUnitService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDSWDataContext _dswDataContext;
        private readonly IDocumentUnitTableValuedModelMapper _mapperTableValue;
        private readonly ISecurity _security;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDocumentContext<ModelDocuments.Document, ModelDocuments.ArchiveDocument> _documentClient;
        private readonly IDecryptedParameterEnvService _parameterEnvService;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitsController(IDSWDataContext dswDataContext, ILogger logger, ISecurity security,
            IDocumentUnitTableValuedModelMapper mapperTableValue, ICurrentIdentity currentIdentity,
            IDocumentUnitService service, IDataUnitOfWork unitOfWork,
            IDocumentContext<ModelDocuments.Document, ModelDocuments.ArchiveDocument> documentClient, IDecryptedParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _dswDataContext = dswDataContext;
            _mapperTableValue = mapperTableValue;
            _security = security;
            _unitOfWork = unitOfWork;
            _documentClient = documentClient;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="year"></param>
        /// <param name="number"></param>
        /// <param name="username"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult AuthorizedDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, [FromODataUri]DocumentUnitFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetAuthorized(finder.Skip.Value, finder.Top.Value, 
                    finder.IdFascicle.Value, Username, Domain, finder.Year, finder.Number, finder.DocumentUnitName, finder.IdCategory, 
                    finder.IdContainer, finder.Subject, finder.IncludeChildClassification.Value, finder.IdTenantAOO.Value);
                ICollection<DocumentUnitModel> mappedModels = _mapperTableValue.MapCollection(documentUnits);
                //IQueryable<DocumentUnitModel> results = options.ApplyTo(mappedModels.AsQueryable()) as IQueryable<DocumentUnitModel>;
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAuthorizedDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, [FromODataUri]DocumentUnitFinderModel finder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int count = _unitOfWork.Repository<DocumentUnit>().CountAuthorized(finder.IdFascicle.Value, Username, Domain, 
                    finder.Year, finder.Number, finder.DocumentUnitName, finder.IdCategory, finder.IdContainer, 
                    finder.Subject, finder.IncludeChildClassification.Value, finder.IdTenantAOO.Value);
                return Ok(count);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult FascicleDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, [FromODataUri]Guid idFascicle, [FromODataUri]Guid? idFascicleFolder, [FromODataUri]Guid idTenantAOO)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                Guid? tmpidTenantAOO = idTenantAOO;
                if (_parameterEnvService.MultiAOOFascicleEnabled)
                {
                    tmpidTenantAOO = null;
                }
                ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetFascicleDocumentUnitsPrivate(new Fascicle(idFascicle), idFascicleFolder, tmpidTenantAOO);
                ICollection<DocumentUnitModel> mappedModels = _mapperTableValue.MapCollection(documentUnits);
                //IQueryable<DocumentUnitModel> results = options.ApplyTo(mappedModels.AsQueryable()) as IQueryable<DocumentUnitModel>;
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetFascicolableDocumentUnits(string dateFrom, string dateTo, bool includeThreshold,
            string threshold, Guid idTenantAOO, bool excludeLinked = false)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<DocumentUnitTableValuedModel> results = _unitOfWork.Repository<DocumentUnit>().GetFascicolableDocumentUnits(Username, Domain,
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    includeThreshold, threshold, idTenantAOO, excludeLinked);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAuthorizedDocuments(string dateFrom, string dateTo, Guid idTenantAOO)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<DocumentUnitTableValuedModel> results = _unitOfWork.Repository<DocumentUnit>().GetAllowedDocumentUnits(Username, Domain,
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    idTenantAOO);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasViewableDocument(Guid idDocumentUnit, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<DocumentUnit>().HasVisibilityRight(username, domain, idDocumentUnit);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public async Task<IHttpActionResult> FullTextSearchDocumentUnits(string filter, Guid idTenant)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
            {
                IList<string> archiveNames = _unitOfWork.Repository<Container>().GetAnyProtocolContainers(idTenant)
                    .Select(s => s.ProtLocation.ProtocolArchive)
                    .Distinct().ToList();
                ICollection<Guid> chains = await _documentClient.FullTextFindDocumentsAsync(archiveNames, filter);
                ICollection<DocumentUnitModel> results = new List<DocumentUnitModel>();
                if (chains.Count > 0)
                {
                    Tenant tenant = _unitOfWork.Repository<Tenant>().GetIncludeTenantAPP(idTenant, true);
                    ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetDocumentUnitsByChains(Username, Domain,
                        chains, tenant.TenantAOO.UniqueId);
                    results = _mapperTableValue.MapCollection(documentUnits);
                }
                return Ok(results);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
