using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates
{
    public class TemplateCollaborationDocumentRepositoryValidatorMapper : BaseMapper<TemplateCollaborationDocumentRepository, TemplateCollaborationDocumentRepositoryValidator>, ITemplateCollaborationDocumentRepositoryValidatorMapper
    {
        public TemplateCollaborationDocumentRepositoryValidatorMapper() { }
        public override TemplateCollaborationDocumentRepositoryValidator Map(TemplateCollaborationDocumentRepository entity, TemplateCollaborationDocumentRepositoryValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ChainType = entity.ChainType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion
            return entityTransformed;
        }
    }
}
