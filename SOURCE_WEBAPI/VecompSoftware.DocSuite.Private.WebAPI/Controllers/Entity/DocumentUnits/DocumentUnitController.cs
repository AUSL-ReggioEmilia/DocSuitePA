using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentUnits
{
    public class DocumentUnitController : BaseWebApiController<DocumentUnit, IDocumentUnitService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitController(IDocumentUnitService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}