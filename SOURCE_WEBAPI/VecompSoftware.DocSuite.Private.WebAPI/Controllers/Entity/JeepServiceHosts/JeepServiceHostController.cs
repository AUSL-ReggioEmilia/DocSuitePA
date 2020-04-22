using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Service.Entity.JeepServiceHosts;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.JeepServiceHosts
{
    public class JeepServiceHostController : BaseWebApiController<JeepServiceHost, IJeepServiceHostService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public JeepServiceHostController(IJeepServiceHostService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}