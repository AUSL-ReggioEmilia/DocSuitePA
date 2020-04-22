using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Dossiers
{
    public class DossierCommentsController : BaseODataController<DossierComment, IDossierCommentService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ] 

        public DossierCommentsController(IDossierCommentService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}