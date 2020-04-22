using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Service.Entity.Commons;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.UDS
{
    public class PrivacyLevelController : BaseWebApiController<PrivacyLevel, IPrivacyLevelService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public PrivacyLevelController(IPrivacyLevelService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
