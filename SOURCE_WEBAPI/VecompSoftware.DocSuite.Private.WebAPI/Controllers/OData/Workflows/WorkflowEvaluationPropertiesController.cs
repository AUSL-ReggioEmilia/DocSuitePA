﻿using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Workflows
{
    public class WorkflowEvaluationPropertiesController : BaseODataController<WorkflowEvaluationProperty, IWorkflowEvaluationPropertyService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public WorkflowEvaluationPropertiesController(IWorkflowEvaluationPropertyService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }
        #endregion
    }
}