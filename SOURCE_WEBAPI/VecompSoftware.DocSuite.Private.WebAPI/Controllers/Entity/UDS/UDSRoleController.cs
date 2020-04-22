using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.UDS
{
    public class UDSRoleController : BaseWebApiController<UDSRole, IUDSRoleService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public UDSRoleController(IUDSRoleService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}