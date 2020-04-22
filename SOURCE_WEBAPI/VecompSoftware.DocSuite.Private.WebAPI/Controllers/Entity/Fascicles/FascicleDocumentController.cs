using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Fascicles
{
    public class FascicleDocumentController : BaseWebApiController<FascicleDocument, IFascicleDocumentService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public FascicleDocumentController(IFascicleDocumentService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
    
        #endregion

    }
}
