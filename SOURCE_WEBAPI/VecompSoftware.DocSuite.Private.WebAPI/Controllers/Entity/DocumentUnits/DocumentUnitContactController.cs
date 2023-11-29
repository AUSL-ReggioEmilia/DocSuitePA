using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentUnits
{
    public class DocumentUnitContactController : BaseWebApiController<DocumentUnitContact, IDocumentUnitContactService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitContactController(IDocumentUnitContactService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}