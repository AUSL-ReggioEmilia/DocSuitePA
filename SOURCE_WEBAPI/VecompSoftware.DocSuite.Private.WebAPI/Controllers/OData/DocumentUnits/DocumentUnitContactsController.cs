using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentUnits
{
    public class DocumentUnitContactsController : BaseODataController<DocumentUnitContact, IDocumentUnitContactService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public DocumentUnitContactsController(IDocumentUnitContactService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]
        #endregion
    }
}