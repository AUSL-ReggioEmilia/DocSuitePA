using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles
{
    public class FascicleRoleController : BaseWebApiController<FascicleRole, IFascicleRoleService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public FascicleRoleController(IFascicleRoleService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}