using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper.Model.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSRepositoriesController : BaseODataController<UDSRepository, IUDSRepositoryService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IUDSRepositoryTableValuedModelMapper _mapperTableValued;
        #endregion

        #region [ Constructor ]

        public UDSRepositoriesController(IUDSRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IUDSRepositoryTableValuedModelMapper mapperTableValued)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _mapperTableValued = mapperTableValued;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetViewableRepositoriesByTypology(ODataQueryOptions<UDSRepository> options, Guid? idUDSTypology, bool pecAnnexedEnabled)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<UDSRepositoryTableValuedModel> results = _unitOfWork.Repository<UDSRepository>().GetViewableRepositoriesByTypology(idUDSTypology, pecAnnexedEnabled);
                return Ok(_mapperTableValued.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetInsertableRepositoriesByTypology(ODataQueryOptions<UDSRepository> options, string username, string domain, Guid? idUDSTypology, bool pecAnnexedEnabled)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<UDSRepositoryTableValuedModel> results = _unitOfWork.Repository<UDSRepository>().GetInsertableRepositoriesByTypology(username, domain, idUDSTypology, pecAnnexedEnabled);
                return Ok(_mapperTableValued.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableCQRSPublishedUDSRepositories(ODataQueryOptions<UDSRepository> options, Guid? idUDSTypology, string name, string alias, short? idContainer)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IEnumerable<UDSRepositoryTableValuedModel> results = _unitOfWork.Repository<UDSRepository>().GetAvailableCQRSPublishedUDSRepositories(idUDSTypology, name, alias, idContainer);
                return Ok(_mapperTableValued.MapCollection(results));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetTenantUDSRepositories(ODataQueryOptions<UDSRepository> options, string tenantName, string udsName)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<UDSRepository> results = _unitOfWork.Repository<UDSRepository>().GetTenantUDSRepositories(tenantName, udsName, true);
                return Ok(results);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
