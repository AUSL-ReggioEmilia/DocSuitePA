using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.UDS
{
    public class UDSPECMailController : BaseWebApiController<UDSPECMail, IUDSPECMailService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public UDSPECMailController(IUDSPECMailService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}