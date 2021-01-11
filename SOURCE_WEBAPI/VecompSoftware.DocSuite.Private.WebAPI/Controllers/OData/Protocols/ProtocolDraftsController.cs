using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Protocols
{
    public class ProtocolDraftsController : BaseODataController<ProtocolDraft, IProtocolDraftService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public ProtocolDraftsController(IProtocolDraftService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
