using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Workflows
{
    public class WorkflowRoleMappingController : BaseWebApiController<WorkflowRoleMapping, IWorkflowRoleMappingService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public WorkflowRoleMappingController(IWorkflowRoleMappingService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}