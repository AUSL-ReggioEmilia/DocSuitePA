using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentArchives
{
    public class DocumentSeriesItemLogController : BaseWebApiController<DocumentSeriesItemLog, IDocumentSeriesItemLogService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemLogController(IDocumentSeriesItemLogService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}