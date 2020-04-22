using Microsoft.AspNet.OData.Query;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Commons
{
    public class RoleUserController : BaseWebApiController<RoleUser, IRoleUserService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Contructor ]
        public RoleUserController(IRoleUserService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetInvalidatedRoleUser(ODataQueryOptions<RoleUser> options)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<RoleUser> result = _unitOfWork.Repository<RoleUser>().GetInvalidatedRoleUser();
                return Ok(result);
            }, _logger, LogCategories);
        }
        #endregion
    }
}