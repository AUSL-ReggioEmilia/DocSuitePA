using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Dossiers
{
    public class DossierDocumentsController : BaseODataController<DossierDocument, IDossierDocumentService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DossierDocumentsController(IDossierDocumentService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}