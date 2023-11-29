using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Collaborations
{
    public class CollaborationDraftController : BaseWebApiController<CollaborationDraft, ICollaborationDraftService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public CollaborationDraftController(ICollaborationDraftService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}
