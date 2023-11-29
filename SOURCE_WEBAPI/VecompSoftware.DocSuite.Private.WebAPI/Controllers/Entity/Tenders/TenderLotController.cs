using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenders;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TenderLotController : BaseWebApiController<TenderLot, ITenderLotService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TenderLotController(ITenderLotService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}