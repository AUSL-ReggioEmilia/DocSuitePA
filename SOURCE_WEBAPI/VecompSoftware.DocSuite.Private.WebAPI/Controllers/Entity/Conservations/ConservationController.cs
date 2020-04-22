using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Conservations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Conservations
{
    public class ConservationController : BaseWebApiController<Conservation, IConservationService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public ConservationController(IConservationService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
