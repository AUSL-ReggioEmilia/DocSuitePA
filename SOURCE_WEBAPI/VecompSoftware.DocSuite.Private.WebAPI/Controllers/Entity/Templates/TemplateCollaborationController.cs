using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Service.Entity.Templates;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Templates
{
    public class TemplateCollaborationController : BaseWebApiController<TemplateCollaboration, ITemplateCollaborationService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public TemplateCollaborationController(ITemplateCollaborationService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
