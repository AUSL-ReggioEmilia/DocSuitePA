using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Processes;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Processes
{
    public class ProcessFascicleTemplatesController : BaseODataController<ProcessFascicleTemplate, IProcessFascicleTemplateService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public ProcessFascicleTemplatesController(IProcessFascicleTemplateService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security) 
            : base(service, unitOfWork, logger, security)
        {
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}