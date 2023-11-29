using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tasks;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TaskHeaderController : BaseWebApiController<TaskHeader, ITaskHeaderService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TaskHeaderController(ITaskHeaderService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}