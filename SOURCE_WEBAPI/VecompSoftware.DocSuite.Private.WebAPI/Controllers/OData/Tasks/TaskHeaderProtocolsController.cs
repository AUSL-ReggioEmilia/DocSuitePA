using Microsoft.AspNet.OData;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tasks;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Tasks
{
    [EnableQuery]
    public class TaskHeaderProtocolsController : BaseODataController<TaskHeaderProtocol, ITaskHeaderProtocolService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public TaskHeaderProtocolsController(ILogger logger, IDataUnitOfWork unitOfWork, ISecurity security, ITaskHeaderProtocolService service)
            :base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
