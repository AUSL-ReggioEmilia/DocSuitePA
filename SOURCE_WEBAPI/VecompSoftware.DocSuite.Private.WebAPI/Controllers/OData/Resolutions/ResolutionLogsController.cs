using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Resolutions
{
    public class ResolutionLogsController : BaseODataController<ResolutionLog, IResolutionLogService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ResolutionLogsController(IResolutionLogService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}