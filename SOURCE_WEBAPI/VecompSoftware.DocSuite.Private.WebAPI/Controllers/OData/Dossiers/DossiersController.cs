using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Dossiers
{
    public class DossiersController : BaseODataController<Dossier, IDossierService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;

        #endregion

        #region [ Constructor ]

        public DossiersController(IDossierService service, IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetAuthorizedDossiers(ODataQueryOptions<Dossier> options, int skip, int top, short? year, int? number, string subject, string note, short? idContainer, string startDateFrom, string startDateTo, string endDateFrom, string endDateTo, Guid? idMetadataRepository, string metadataValue)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
           {
               ICollection<DossierTableValuedModel> dossierResults = _unitOfWork.Repository<Dossier>().GetAuthorized(Username, Domain, skip, top, year, number, subject, idContainer, startDateFrom, startDateTo, endDateFrom, endDateTo, note, idMetadataRepository, metadataValue);
               ICollection<DossierModel> dossierModels = _mapperUnitOfwork.Repository<IDomainMapper<DossierTableValuedModel, DossierModel>>().MapCollection(dossierResults);

               return Ok(dossierModels);
           }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAuthorizedDossiers(ODataQueryOptions<Dossier> options, short? year, int? number, string subject, short? idContainer, Guid? idMetadataRepository, string metadataValue)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int dossiersCount = _unitOfWork.Repository<Dossier>().CountAuthorized(Username, Domain, year, number, subject, idContainer, idMetadataRepository, metadataValue);
                return Ok(dossiersCount);
            }, _logger, LogCategories);
        }



        [HttpGet]
        public IHttpActionResult GetCompleteDossier(ODataQueryOptions<Dossier> options, Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Dossier> dossierResult = _unitOfWork.Repository<Dossier>().GetCompleteDossier(uniqueId);
                IQueryable<DossierModel> dossierModel = _mapperUnitOfwork.Repository<IDomainMapper<Dossier, DossierModel>>().MapCollection(dossierResult).AsQueryable();

                return Ok(dossierModel);
            }, _logger, LogCategories);
        }


        public IHttpActionResult GetByUniqueId(ODataQueryOptions<Dossier> options, Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Dossier> dossierResult = _unitOfWork.Repository<Dossier>().GetByUniqueId(uniqueId);
                IQueryable<DossierModel> dossierModel = _mapperUnitOfwork.Repository<IDomainMapper<Dossier, DossierModel>>().MapCollection(dossierResult).AsQueryable();

                return Ok(dossierModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetDossierContacts(ODataQueryOptions<Dossier> options, Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ContactTableValuedModel> contacts = _unitOfWork.Repository<Dossier>().GetDossierContacts(uniqueId);
                return Ok(contacts);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetDossierRoles(ODataQueryOptions<DossierRole> options, Guid idDossier)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<DossierRole> dossierRoles = _unitOfWork.Repository<DossierRole>().GetAuthorizedDossierRoles(idDossier);
                IQueryable<DossierRoleModel> dossierRoleModel = _mapperUnitOfwork.Repository<IDomainMapper<DossierRole, DossierRoleModel>>().MapCollection(dossierRoles).AsQueryable();
                return Ok(dossierRoleModel);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult IsViewableDossier(Guid idDossier)
        {
            return DocSuite.WebAPI.Common.Helpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {

                bool result = _unitOfWork.Repository<Dossier>().HasDossierViewable(idDossier, Username, Domain);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult IsManageableDossier(Guid idDossier)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Dossier>().HasDossierManageable(idDossier, Username, Domain);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasInsertRight()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Dossier>().HasDossierInsertRight(Username, Domain);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasModifyRight(Guid idDossier)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Dossier>().HasDossierModifyRight(Username, Domain, idDossier);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasRootNode(Guid idDossier)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Dossier>().HasRootNode(idDossier);
                return Ok(result);
            }, _logger, LogCategories);
        }
        #endregion
    }
}