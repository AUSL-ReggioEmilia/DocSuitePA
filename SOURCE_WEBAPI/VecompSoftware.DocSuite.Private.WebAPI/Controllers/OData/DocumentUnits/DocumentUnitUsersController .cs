using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentUnits
{
    public class DocumentUnitUsersController : BaseODataController<DocumentUnitUser, IDocumentUnitUserService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitUsersController(ILogger logger, IDocumentUnitUserService service, IDataUnitOfWork unitOfWork, ISecurity security)
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
