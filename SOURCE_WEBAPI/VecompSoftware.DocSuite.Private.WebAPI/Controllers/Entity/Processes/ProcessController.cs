using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Processes
{
    public class ProcessController : BaseWebApiController<Process, IProcessService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public ProcessController(IProcessService service, IDataUnitOfWork unitOfWork, ILogger logger) 
            : base(service, unitOfWork, logger)
        {

        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}