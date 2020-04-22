using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Templates
{
    public class TemplateCollaborationDocumentRepositoriesController : BaseODataController<TemplateCollaborationDocumentRepository, ITemplateCollaborationDocumentRepositoryService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public TemplateCollaborationDocumentRepositoriesController(ITemplateCollaborationDocumentRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ISecurity security)
            : base(service, unitOfWork, logger, security)
        {

        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}