using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Workflows
{
    public class WorkflowRepositoriesController : BaseODataController<WorkflowRepository, IWorkflowRepositoryService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public WorkflowRepositoriesController(IWorkflowRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Method ]

        [HttpGet]
        public IHttpActionResult GetAuthorizedActiveWorkflowRepositories(ODataQueryOptions<WorkflowRepository> options, int environment, bool anyEnv, 
            bool documentRequired, bool showOnlyNoInstanceWorkflows, bool showOnlyHasIsFascicleClosedRequired, bool documentUnitRequired)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<WorkflowRepository> workflowRepositories = _unitOfWork.Repository<WorkflowRepository>().GetAuthorizedActiveWorkflowRepositories(Username, Domain, environment, anyEnv, 
                    documentRequired, showOnlyNoInstanceWorkflows, showOnlyHasIsFascicleClosedRequired, documentUnitRequired);
                IQueryable<WorkflowRepository> results = options.ApplyTo(workflowRepositories.AsQueryable()) as IQueryable<WorkflowRepository>;
                return Ok(results);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult HasAuthorizedWorkflowRepositories(ODataQueryOptions<WorkflowRepository> options, int environment, bool anyEnv)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                bool result = _unitOfWork.Repository<WorkflowRepository>().HasAuthorizedWorkflowRepositories(Username, Domain, environment, anyEnv);
                return Ok(result);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetByWorkflowActivityId(Guid workflowActivityId)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<WorkflowRepository> result = _unitOfWork.Repository<WorkflowRepository>().GetByWorkflowActivityId(workflowActivityId);
                return Ok(result);
            }, _logger, LogCategories);
        }

        #endregion
    }
}
