using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Service.Entity.Dossiers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Dossiers
{
    public class DossierFolderRoleController : BaseWebApiController<DossierFolderRole, IDossierFolderRoleService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DossierFolderRoleController(IDossierFolderRoleService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}