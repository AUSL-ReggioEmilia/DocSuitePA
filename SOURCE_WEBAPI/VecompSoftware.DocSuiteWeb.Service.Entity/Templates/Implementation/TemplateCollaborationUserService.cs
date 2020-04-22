using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Templates
{
    public class TemplateCollaborationUserService : BaseService<TemplateCollaborationUser>, ITemplateCollaborationUserService
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ] 
        public TemplateCollaborationUserService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            ITemplateCollaborationRuleset collaborationRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, collaborationRuleset, mapperUnitOfWork, security)
        {
        }
        #endregion

        protected override TemplateCollaborationUser BeforeCreate(TemplateCollaborationUser entity)
        {
            entity.IsValid = true;
            return base.BeforeCreate(entity);
        }
    }
}
