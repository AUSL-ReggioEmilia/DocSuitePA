using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Finder.Workflows;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Workflows
{

    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class WorkflowsController : ODataController
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static IEnumerable<LogCategory> _logCategories = null;
        private readonly Guid _instanceId;
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

        public WorkflowsController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _instanceId = Guid.NewGuid();
        }

        #endregion

        #region [ Methods ]

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult MyInstances(string workflowName)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<WorkflowInstance> workflowInstances = _unitOfWork.Repository<WorkflowInstance>().GetByWorkflowName(workflowName).ToList();
                ICollection<WorkflowStatusModel> results = _mapper.Map<ICollection<WorkflowInstance>, ICollection<WorkflowStatusModel>>(workflowInstances);

                return Ok(results.AsQueryable());
            }, _logger, LogCategories);
        }


        #endregion
    }
}
