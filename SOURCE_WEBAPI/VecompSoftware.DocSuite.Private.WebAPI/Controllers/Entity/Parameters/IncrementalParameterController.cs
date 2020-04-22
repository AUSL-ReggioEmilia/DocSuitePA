using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Service.Entity.Parameters;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Parameters
{
    public class IncrementalParameterController : BaseWebApiController<Parameter, IParameterService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public IncrementalParameterController(IParameterService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}