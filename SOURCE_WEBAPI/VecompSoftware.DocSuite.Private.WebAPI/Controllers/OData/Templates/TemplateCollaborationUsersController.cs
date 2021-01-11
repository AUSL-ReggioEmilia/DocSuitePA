using Microsoft.AspNet.OData.Query;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Templates
{
    public class TemplateCollaborationUsersController : BaseODataController<TemplateCollaborationUser, ITemplateCollaborationUserService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public TemplateCollaborationUsersController(ITemplateCollaborationUserService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        public IHttpActionResult GetAllInvalidTemplateUsers(ODataQueryOptions<TemplateCollaborationUser> options)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                IQueryable<TemplateCollaborationUser> validusers = _unitOfWork.Repository<TemplateCollaborationUser>().GetInvalidatingTemplateCollaborationUsers();
                return Ok(validusers);
            }, _logger, LogCategories);
        }
        #endregion

    }
}