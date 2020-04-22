using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Fascicles
{
    public class FascicleDocumentUnitsController : BaseODataController<FascicleDocumentUnit, IFascicleDocumentUnitService>
    {
        #region [ Fields ]
        private readonly IFascicleDocumentUnitService _service;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitsController(IFascicleDocumentUnitService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security) : base(service, unitOfWork, logger, security)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}