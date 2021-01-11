using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;
using VecompSoftware.DocSuiteWeb.Model.Validations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Validation;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Fascicles
{
    public class FasciclesController : BaseODataController<Fascicle, IFascicleService>
    {
        #region [ Fields ]

        private readonly IFascicleModelMapper _mapper;
        private readonly IFascicleTableValuedModelMapper _fascicleTableValuedModelMapper;
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        private readonly IMetadataFilterFactory _metadataFilterFactory;
        private readonly IParameterEnvService _parameterEnvService; 

        #endregion

        #region [ Constructor ]

        public FasciclesController(IFascicleService service, IDataUnitOfWork unitOfWork,
            ILogger logger, IFascicleModelMapper mapper, IMapperUnitOfWork mapperUnitOfwork, 
            IFascicleTableValuedModelMapper fascicleTableValuedModelMapper, ISecurity security, 
            IMetadataFilterFactory metadataFilterFactory, IParameterEnvService parameterEnvService)
            : base(service, unitOfWork, logger, security)
        {
            _mapper = mapper;
            _fascicleTableValuedModelMapper = fascicleTableValuedModelMapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfwork = mapperUnitOfwork;
            _metadataFilterFactory = metadataFilterFactory;
            _parameterEnvService = parameterEnvService;
        }

        #endregion

        #region [ Methods ]        

        [HttpGet]
        public IHttpActionResult AvailableFascicles(ODataQueryOptions<FascicleModel> options, Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<FascicleTableValuedModel> availableFascicles = _unitOfWork.Repository<Fascicle>().GetAvailables(Username, Domain, uniqueId);
                ICollection<FascicleModel> mappedModels = _fascicleTableValuedModelMapper.MapCollection(availableFascicles);
                //IQueryable<FascicleModel> results = options.ApplyTo(mappedModels.AsQueryable()) as IQueryable<FascicleModel>;
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult PeriodicFascicles(ODataQueryOptions<FascicleModel> options, Guid uniqueId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                DocumentUnit documentUnit = _unitOfWork.Repository<DocumentUnit>().Find(uniqueId);
                if (documentUnit == null)
                {
                    throw new DSWValidationException("PeriodicFascicles validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel() { Key = string.Concat("DocumentUnit - ", uniqueId), Message = "Nessuna unità documentaria trovata con l'ID passato" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }

                ICollection<FascicleTableValuedModel> availableFascicles = _unitOfWork.Repository<Fascicle>().GetPeriodicFromDocumentUnit(documentUnit);
                ICollection<FascicleModel> mappedModels = _fascicleTableValuedModelMapper.MapCollection(availableFascicles);
                return Ok(mappedModels);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult DocumentUnitAssociated(ODataQueryOptions<Fascicle> options, Guid uniqueId)
        {

            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                DocumentUnit documentUnit = _unitOfWork.Repository<DocumentUnit>().GetById(uniqueId).SingleOrDefault();
                if (documentUnit == null)
                {
                    throw new DSWValidationException("DocumentUnitAssociated validation error",
                        new List<ValidationMessageModel>() { new ValidationMessageModel() { Key = "Protocol", Message = "Nessun documneti trovato con l'ID passato" } },
                        null, DSWExceptionCode.VA_RulesetValidation);
                }
                IQueryable<Fascicle> associatedFascicles = _unitOfWork.Repository<Fascicle>().GetAssociated(documentUnit);
                return Ok(_mapper.MapCollection(associatedFascicles));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult NotLinkedFascicles(ODataQueryOptions<Fascicle> options, Guid idFascicle, int idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Fascicle> notLinkedFascicles;
                if (idCategory == 0)
                {
                    notLinkedFascicles = _unitOfWork.Repository<Fascicle>().GetNotLinked(idFascicle);
                }
                else
                {
                    notLinkedFascicles = _unitOfWork.Repository<Fascicle>().GetNotLinked(idFascicle, idCategory);
                }
                //IQueryable<Fascicle> results = options.ApplyTo(notLinkedFascicles) as IQueryable<Fascicle>;
                return Ok(notLinkedFascicles);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasViewableDocument(Guid idFascicle, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasDocumentVisibilityRights(username, domain, idFascicle);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetFasciclesByCategory(ODataQueryOptions<FascicleModel> options, short idCategory, string name, bool? hasProcess)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
             {

                 ICollection<FascicleTableValuedModel> fasciclesModel = _unitOfWork.Repository<Fascicle>().GetByRight(Username, Domain, idCategory, name, hasProcess);
                 ICollection<FascicleModel> fascicles = _mapperUnitOfwork.Repository<IDomainMapper<FascicleTableValuedModel, FascicleModel>>().MapCollection(fasciclesModel);
                 return Ok(fascicles.OrderByDescending(x => x.RegistrationDate));
             }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasFascicolatedDocumentUnits(Guid idFascicle)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasFascicolatedDocumentUnits(idFascicle);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasInsertRight(DocSuiteWeb.Entity.Fascicles.FascicleType fascicleType)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasInsertRight(Username, Domain, fascicleType);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasViewableRight(Guid idFascicle)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasViewableRight(Username, Domain, idFascicle);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasManageableRight(Guid idFascicle)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasManageableRight(Username, Domain, idFascicle);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasProcedureDistributionInsertRight(short idCategory)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().HasProcedureDistributionInsertRight(Username, Domain, idCategory);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult AuthorizedFascicles(ODataQueryOptions<Fascicle> options, ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                FascicleFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as FascicleFinderModel;

                DateTimeOffset? thresholdDate = CreateThresholdDate(finder.ViewOnlyClosable);
                IDictionary<string, string> metadataValues = finder.MetadataValues.ToDictionary(d => d.KeyName, d => _metadataFilterFactory.CreateMetadataFilter(d).ToFilter());
                ICollection<FascicleTableValuedModel> fasciclesModel = _unitOfWork.Repository<Fascicle>().GetAuthorized(Username, Domain, finder.Skip, finder.Top, finder.Year, 
                    finder.StartDateFrom, finder.StartDateTo, finder.EndDateFrom, finder.EndDateTo, finder.FascicleStatus, finder.Manager, finder.Name, finder.Subject, 
                    finder.ViewConfidential, finder.ViewAccessible, finder.Note, finder.Rack, finder.IdMetadataRepository, finder.MetadataValue, metadataValues,
                    finder.Classifications, finder.IncludeChildClassifications, finder.Roles, finder.Container, finder.ApplySecurity, _parameterEnvService.ForceDescendingOrderElements,
                    finder.ViewOnlyClosable, thresholdDate, finder.Title, finder.IsManager, finder.IsSecretary);
                ICollection<FascicleModel> fascicles = _mapperUnitOfwork.Repository<IDomainMapper<FascicleTableValuedModel, FascicleModel>>().MapCollection(fasciclesModel);
                return Ok(fascicles);
            }, _logger, LogCategories);
        }

        [HttpPost]
        public IHttpActionResult CountAuthorizedFascicles(ODataQueryOptions<Fascicle> options, ODataActionParameters parameter)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                FascicleFinderModel finder = parameter[ODataConfig.ODATA_FINDER_PARAMETER] as FascicleFinderModel;

                DateTimeOffset? thresholdDate = CreateThresholdDate(finder.ViewOnlyClosable);
                IDictionary<string, string> metadataValues = finder.MetadataValues.ToDictionary(d => d.KeyName, d => _metadataFilterFactory.CreateMetadataFilter(d).ToFilter());
                int countFascicles = _unitOfWork.Repository<Fascicle>().CountAuthorized(Username, Domain, finder.Year, finder.StartDateFrom, finder.StartDateTo,
                    finder.EndDateFrom, finder.EndDateTo, finder.FascicleStatus, finder.Manager, finder.Name, finder.Subject, finder.ViewConfidential, finder.ViewAccessible, finder.Note,
                    finder.Rack, finder.IdMetadataRepository, finder.MetadataValue, metadataValues, finder.Classifications, finder.IncludeChildClassifications, finder.Roles, finder.Container, 
                    finder.ApplySecurity, finder.ViewOnlyClosable, thresholdDate, finder.Title, finder.IsManager, finder.IsSecretary);
                return Ok(countFascicles);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult IsManager(Guid idFascicle)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<Fascicle>().IsManager(Username, Domain, idFascicle);
                return Ok(result);
            }, _logger, LogCategories);
        }

        private DateTimeOffset? CreateThresholdDate(bool? viewOnlyClosable)
        {
            DateTimeOffset? thresholdDate = null;

            if (viewOnlyClosable.HasValue && viewOnlyClosable.Value)
            {
                int fascicleAutoCloseThreshlodDays = _parameterEnvService.FascicleAutoCloseThresholdDays;
                thresholdDate = DateTimeOffset.UtcNow.AddDays(-fascicleAutoCloseThreshlodDays);
            }

            return thresholdDate;
        }


        [HttpGet]
        public IHttpActionResult AuthorizedFasciclesFromDocumentUnit(ODataQueryOptions<Fascicle> options, Guid uniqueIdDocumentUnit)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<FascicleTableValuedModel> fascicleTableValuedModels = _unitOfWork.Repository<Fascicle>().AuthorizedFasciclesFromDocumentUnit(Username, Domain, uniqueIdDocumentUnit);
                return Ok(_fascicleTableValuedModelMapper.MapCollection(fascicleTableValuedModels));
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAuthorizedFasciclesFromDocumentUnit(ODataQueryOptions<Fascicle> options, Guid uniqueIdDocumentUnit)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int countFascicles = _unitOfWork.Repository<Fascicle>().CountAuthorizedFasciclesFromDocumentUnit(Username, Domain, uniqueIdDocumentUnit);
                return Ok(countFascicles);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
