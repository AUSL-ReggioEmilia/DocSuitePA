using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Service.Entity.MassimariScarto;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.MassimariScarto
{
    public class MassimarioScartoController : BaseWebApiController<MassimarioScarto, IMassimarioScartoService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public MassimarioScartoController(IMassimarioScartoService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
