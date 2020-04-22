using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Templates
{
    public class TemplateDocumentRepositoryController : BaseWebApiController<TemplateDocumentRepository, ITemplateDocumentRepositoryService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public TemplateDocumentRepositoryController(ITemplateDocumentRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
