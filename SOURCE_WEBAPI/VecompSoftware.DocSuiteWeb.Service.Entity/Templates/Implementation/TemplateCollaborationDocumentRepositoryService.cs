using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Templates
{
    public class TemplateCollaborationDocumentRepositoryService : BaseService<TemplateCollaborationDocumentRepository>, ITemplateCollaborationDocumentRepositoryService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public TemplateCollaborationDocumentRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITemplateCollaborationRuleset collaborationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, collaborationRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion
    }
}
