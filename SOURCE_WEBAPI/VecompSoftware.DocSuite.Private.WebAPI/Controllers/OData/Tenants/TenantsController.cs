using Microsoft.AspNet.OData;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenants;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Tenants
{
    [EnableQuery]
    public class TenantsController : BaseODataController<Tenant, ITenantService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ITenantTableValuedModelMapper _tenantTableValuedModelMapper;
        #endregion

        #region [ Constructor ]

        public TenantsController(ITenantService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security, IMapperUnitOfWork mapperUnitOfwork, ITenantTableValuedModelMapper tenantTableValuedModelMapper)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tenantTableValuedModelMapper = tenantTableValuedModelMapper;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public IHttpActionResult GetUserTenants()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<TenantTableValuedModel> result =
                    _unitOfWork.Repository<Tenant>().GetUserTenants(Username, Domain);

                ICollection<Tenant> mappedModels = _tenantTableValuedModelMapper.MapCollection(result);
                return Ok(mappedModels);

            }, _logger, LogCategories);
        }

        #endregion
    }
}
