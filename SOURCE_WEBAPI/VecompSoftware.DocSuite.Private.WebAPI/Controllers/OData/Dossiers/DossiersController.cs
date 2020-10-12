using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;
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
        private readonly IMetadataFilterFactory _metadataFilterFactory;

        #endregion

        #region [ Constructor ]

        public DossiersController(IDossierService service, IDataUnitOfWork unitOfWork, ILogger logger, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, IMetadataFilterFactory metadataFilterFactory)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfWork;
            _metadataFilterFactory = metadataFilterFactory;
        }

        #endregion

        #region [ Methods ]

        [HttpPost]
        public IHttpActionResult GetAuthorizedDossiers(ODataQueryOptions<Dossier> options, ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                DossierFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as DossierFinderModel;
                IDictionary<string, string> metadataValues = finder.MetadataValues.ToDictionary(d => d.KeyName, d => _metadataFilterFactory.CreateMetadataFilter(d).ToFilter());
                ICollection<DossierTableValuedModel> dossierResults = _unitOfWork.Repository<Dossier>().GetAuthorized(Username, Domain, finder.Skip, finder.Top, finder.Year, finder.Number, finder.Subject, finder.IdContainer, finder.StartDateFrom,
                    finder.StartDateTo, finder.EndDateFrom, finder.EndDateTo, finder.Note, finder.IdMetadataRepository, finder.MetadataValue, metadataValues, finder.IdCategory, finder.DossierType, finder.Status);
                ICollection<DossierModel> dossierModels = _mapperUnitOfwork.Repository<IDomainMapper<DossierTableValuedModel, DossierModel>>().MapCollection(dossierResults);

                return Ok(dossierModels);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountAuthorizedDossiers(ODataQueryOptions<Dossier> options, ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                DossierFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as DossierFinderModel;
                IDictionary<string, string> metadataValues = finder.MetadataValues.ToDictionary(d => d.KeyName, d => _metadataFilterFactory.CreateMetadataFilter(d).ToFilter());
                int dossiersCount = _unitOfWork.Repository<Dossier>().CountAuthorized(Username, Domain, finder.Year, finder.Number, 
                    finder.Subject, finder.IdContainer, finder.StartDateFrom, finder.StartDateTo, finder.EndDateFrom, finder.EndDateTo, finder.Note, 
                    finder.IdMetadataRepository, finder.MetadataValue, metadataValues, finder.IdCategory, finder.DossierType, finder.Status);
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

        [HttpGet]
        public IHttpActionResult AllFasciclesAreClosed(Guid idDossier)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Dossier>().AllFasciclesAreClosed(idDossier);
                return Ok(result);
            }, _logger, LogCategories);
        }

        #endregion
    }
}