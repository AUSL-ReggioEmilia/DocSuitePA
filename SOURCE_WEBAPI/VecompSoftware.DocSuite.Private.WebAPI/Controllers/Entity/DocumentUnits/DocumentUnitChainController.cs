using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.DocumentUnits
{
    public class DocumentUnitChainController : BaseWebApiController<DocumentUnitChain, IDocumentUnitChainService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public DocumentUnitChainController(IDocumentUnitChainService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}