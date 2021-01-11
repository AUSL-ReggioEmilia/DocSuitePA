using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Resolutions;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Resolutions
{
    public class ResolutionKindDocumentSeriessController : BaseODataController<ResolutionKindDocumentSeries, IResolutionKindDocumentSeriesService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public ResolutionKindDocumentSeriessController(IResolutionKindDocumentSeriesService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
