using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.UDS
{
    public class UDSSchemaRepositoryController : BaseWebApiController<UDSSchemaRepository, IUDSSchemaRepositoryService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public UDSSchemaRepositoryController(IUDSSchemaRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
