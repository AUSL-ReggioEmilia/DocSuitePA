using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Processes
{
    #region [ Fields ]

    #endregion

    #region [ Constructor ]

    public class ProcessFascicleWorkflowRepositoryController : BaseWebApiController<ProcessFascicleWorkflowRepository, IProcessFascicleWorkflowRepositoryService>
    {
        public ProcessFascicleWorkflowRepositoryController(IProcessFascicleWorkflowRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger) 
            : base(service, unitOfWork, logger)
        {
        }
    }

    #endregion

    #region [ Methods ]

    #endregion
}