using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers
{
    public class DossierDocumentController : BaseWebApiController<DossierDocument, IDossierDocumentService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DossierDocumentController(IDossierDocumentService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}