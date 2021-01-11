using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Service.Entity.Desks;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Desks
{
    public class DeskController : BaseWebApiController<Desk, IDeskService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DeskController(IDeskService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}