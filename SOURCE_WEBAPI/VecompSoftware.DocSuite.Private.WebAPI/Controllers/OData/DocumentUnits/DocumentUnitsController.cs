using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

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

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitsController(IDSWDataContext dswDataContext, ILogger logger, ISecurity security,
            IDocumentUnitTableValuedModelMapper mapperTableValue, ICurrentIdentity currentIdentity,
            IDocumentUnitService service, IDataUnitOfWork unitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _dswDataContext = dswDataContext;
            _mapperTableValue = mapperTableValue;
            _security = security;
            _unitOfWork = unitOfWork;
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
        public IHttpActionResult AuthorizedDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, int skip, int top, Guid idFascicle, int? year, string number, string documentUnitName, int? categoryId, int? containerId, string subject, bool includeChildClassification)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetAuthorized(skip, top, idFascicle, Username, Domain, year, number, documentUnitName, categoryId, containerId, subject, includeChildClassification);
                ICollection<DocumentUnitModel> mappedModels = _mapperTableValue.MapCollection(documentUnits);
                //IQueryable<DocumentUnitModel> results = options.ApplyTo(mappedModels.AsQueryable()) as IQueryable<DocumentUnitModel>;
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAuthorizedDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, Guid idFascicle, int? year, string number, string documentUnitName, int? categoryId, int? containerId, string subject, bool includeChildClassification)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                int count = _unitOfWork.Repository<DocumentUnit>().CountAuthorized(idFascicle, Username, Domain, year, number, documentUnitName, categoryId, containerId, subject, includeChildClassification);
                return Ok(count);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult FascicleDocumentUnits(ODataQueryOptions<DocumentUnitModel> options, Guid idFascicle, Guid? idFascicleFolder)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<DocumentUnitTableValuedModel> documentUnits = _unitOfWork.Repository<DocumentUnit>().GetFascicleDocumentUnitsPrivate(new Fascicle(idFascicle), idFascicleFolder);
                ICollection<DocumentUnitModel> mappedModels = _mapperTableValue.MapCollection(documentUnits);
                //IQueryable<DocumentUnitModel> results = options.ApplyTo(mappedModels.AsQueryable()) as IQueryable<DocumentUnitModel>;
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        //TODO: Rivedere la funzione FascicolableDocumentUnits dopo la creazione dell'entità DocumentUnit
        public IHttpActionResult GetFascicolableDocumentUnits(string username, string domain, string dateFrom, string dateTo, bool includeThreshold,
            string threshold, bool excludeLinked = false)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<DocumentUnitTableValuedModel> results = _unitOfWork.Repository<DocumentUnit>().GetFascicolableDocumentUnits(
                    username, domain,
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    includeThreshold, threshold, excludeLinked);
                return Ok(_mapperTableValue.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAuthorizedDocuments(string username, string domain, string dateFrom, string dateTo, bool isSecurityEnabled)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<DocumentUnitTableValuedModel> results = _unitOfWork.Repository<DocumentUnit>().GetAllowedDocumentUnits(username, domain,
                    DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                    DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture));
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

        #endregion
    }
}
