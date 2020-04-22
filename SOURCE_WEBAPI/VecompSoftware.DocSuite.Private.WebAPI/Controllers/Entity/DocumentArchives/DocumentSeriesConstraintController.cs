using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives
{
    public class DocumentSeriesConstraintController : BaseWebApiController<DocumentSeriesConstraint, IDocumentSeriesConstraintService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public DocumentSeriesConstraintController(IDocumentSeriesConstraintService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
