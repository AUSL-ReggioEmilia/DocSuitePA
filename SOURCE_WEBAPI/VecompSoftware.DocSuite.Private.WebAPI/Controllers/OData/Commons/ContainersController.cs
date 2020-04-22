using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Commons
{
    public class ContainersController : BaseODataController<Container, IContainerService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IContainerService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Constructor ]

        public ContainersController(IContainerService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _service = service;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
        }


        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult HasProtocolInsertRight(string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool hasRight = _unitOfWork.Repository<Container>().CountProtocolInsertRight(username, domain) > 0;
                return Ok(hasRight);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetDossierInsertAuthorizedContainers()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Container> containerResults = _unitOfWork.Repository<Container>().GetDossierInsertContainers(Username, Domain);
                return Ok(containerResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAnyDossierAuthorizedContainers()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Container> containerResults = _unitOfWork.Repository<Container>().GetAnyDossierContainers();
                return Ok(containerResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetInsertAuthorizedContainers()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<Container> containerResults = _unitOfWork.Repository<Container>().GetInsertAuthorizedContainers(Username, Domain);
                return Ok(containerResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetFascicleInsertAuthorizedContainers(short? idCategory, FascicleType? fascicleType)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ContainerModel> containers = _unitOfWork.Repository<Container>().GetFascicleInsertContainers(Username, Domain, idCategory, fascicleType);
                return Ok(containers);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
