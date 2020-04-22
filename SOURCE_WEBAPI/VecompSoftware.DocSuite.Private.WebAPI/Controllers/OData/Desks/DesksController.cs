using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Desks;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Desks
{
    public class DesksController : BaseODataController<Desk, IDeskService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public DesksController(IDeskService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]
        #endregion
    }
}
