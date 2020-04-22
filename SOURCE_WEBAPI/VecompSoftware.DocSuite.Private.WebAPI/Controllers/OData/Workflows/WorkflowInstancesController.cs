using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Workflows;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Workflows
{
    public class WorkflowInstancesController : BaseODataController<WorkflowInstance, IWorkflowInstanceService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public WorkflowInstancesController(IWorkflowInstanceService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion
    }
}
