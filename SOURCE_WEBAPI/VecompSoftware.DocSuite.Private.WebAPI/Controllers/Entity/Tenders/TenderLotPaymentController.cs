using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenders;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TenderLotPaymentController : BaseWebApiController<TenderLotPayment, ITenderLotPaymentService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TenderLotPaymentController(ITenderLotPaymentService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}