using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSRolesController : BaseODataController<UDSRole, IUDSRoleService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public UDSRolesController(IUDSRoleService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}