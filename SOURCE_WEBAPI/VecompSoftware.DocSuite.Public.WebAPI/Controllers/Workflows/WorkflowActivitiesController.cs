using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Workflows
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class WorkflowActivitiesController : ODataController
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly Guid _instanceId;
        private readonly DomainUserModel _currentUser;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowsController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        public WorkflowActivitiesController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper, Security.ISecurity security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _instanceId = Guid.NewGuid();
            _currentUser = security.GetCurrentUser();
        }

        #endregion

        #region [ Methods ]

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult MyActivities([FromODataUri]WorkflowActivityFinderModel finder)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<WorkflowActivity> workflows = _unitOfWork.Repository<WorkflowActivity>().GetAuthorized(finder, _currentUser.Account).ToList();
                ICollection<WorkflowActivityModel> result = _mapper.Map<ICollection<WorkflowActivity>, ICollection<WorkflowActivityModel>>(workflows);
                return Ok(result.AsQueryable());
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All, HandleNullPropagation = HandleNullPropagationOption.Default)]
        [HttpGet]
        public IHttpActionResult CurrentWorkflowActivityFromDocumentUnit([FromODataUri]Guid idDocumentUnit)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                WorkflowActivity workflowActivity = _unitOfWork.Repository<WorkflowActivity>().GetWorkflowActivityByDoumentUnitId(idDocumentUnit, _currentUser.Account);
                IList<WorkflowActivityModel> result = new List<WorkflowActivityModel>();
                if (workflowActivity != null)
                {
                    IList<WorkflowActivity> workflowActivities = workflowActivities = new List<WorkflowActivity> { workflowActivity };
                    result = _mapper.Map<IList<WorkflowActivity>, IList<WorkflowActivityModel>>(workflowActivities);
                }
                return Ok(result);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All, HandleNullPropagation = HandleNullPropagationOption.Default)]
        [HttpGet]
        public IHttpActionResult GetLastWorkflowActivityFromDocumentUnit([FromODataUri]Guid idDocumentUnit)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                WorkflowActivity workflowActivity = _unitOfWork.Repository<WorkflowActivity>().GetLastWorkflowActivityByDoumentUnitId(idDocumentUnit, _currentUser.Account);
                IList<WorkflowActivityModel> result = new List<WorkflowActivityModel>();
                if (workflowActivity != null)
                {
                    IList<WorkflowActivity> workflowActivities = workflowActivities = new List<WorkflowActivity> { workflowActivity };
                    result = _mapper.Map<IList<WorkflowActivity>, IList<WorkflowActivityModel>>(workflowActivities);
                }
                return Ok(result);
            }, _logger, LogCategories);
        }

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult CountUserAuthorizedWorkflowActivities([FromODataUri]WorkflowActivityFinderModel finder)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                long count = _unitOfWork.Repository<WorkflowActivity>().CountUserAuthorized(_currentUser.Account, finder);
                return Ok(count);
            }, _logger, LogCategories);
        }

        #endregion
    }
}