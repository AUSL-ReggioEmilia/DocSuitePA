using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tasks;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TaskDetailController : BaseWebApiController<TaskDetail, ITaskDetailService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TaskDetailController(ITaskDetailService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}