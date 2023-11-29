using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Collaborations
{
    public class CollaborationDraftsController : BaseODataController<CollaborationDraft, ICollaborationDraftService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public CollaborationDraftsController(ICollaborationDraftService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
