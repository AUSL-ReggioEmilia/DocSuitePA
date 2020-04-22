using System;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Workflows
{
    public class WorkflowActivitiesController : BaseODataController<WorkflowActivity, IWorkflowActivityService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public WorkflowActivitiesController(IWorkflowActivityService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult IsWorkflowActivityHandler(Guid workflowActivityId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool isHandler = _unitOfWork.Repository<WorkflowActivity>().IsWorkflowActivityHandler(Username, Domain, workflowActivityId);
                return Ok(isHandler);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasHandler(Guid workflowActivityId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool hasHandler = _unitOfWork.Repository<WorkflowActivity>().HasHandler(workflowActivityId);
                return Ok(hasHandler);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult IsAuthorized(Guid workflowActivityId, string username, string domain)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool isAuthorized = _unitOfWork.Repository<WorkflowActivity>().IsAuthorized(username, domain, workflowActivityId);
                return Ok(isAuthorized);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetActiveActivitiesByReferenceIdAndEnvironment(Guid referenceId, DSWEnvironmentType type)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<WorkflowActivity> workflowActivity = _unitOfWork.Repository<WorkflowActivity>().GetActiveActivitiesByReferenceIdAndEnvironment(referenceId, type);
                return Ok(workflowActivity);

            }, _logger, LogCategories);
        }
        #endregion
    }
}