using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Collaborations
{
    public class CollaborationAggregatesController : BaseODataController<CollaborationAggregate, ICollaborationAggregateService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public CollaborationAggregatesController(ICollaborationAggregateService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]
        #endregion
    }
}
