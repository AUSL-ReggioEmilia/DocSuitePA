using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Protocols
{
    public class ProtocolJournalController : BaseWebApiController<ProtocolJournal, IProtocolJournalService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public ProtocolJournalController(IProtocolJournalService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}