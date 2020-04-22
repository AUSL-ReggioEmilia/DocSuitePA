using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Workflows
{
    public class WorkflowActivityController : BaseWebApiController<WorkflowActivity, IWorkflowActivityService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public WorkflowActivityController(IWorkflowActivityService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}