using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates
{
    public class TemplateCollaborationUserValidatorMapper : BaseMapper<TemplateCollaborationUser, TemplateCollaborationUserValidator>, ITemplateCollaborationUserValidatorMapper
    {
        public TemplateCollaborationUserValidatorMapper() { }

        public override TemplateCollaborationUserValidator Map(TemplateCollaborationUser entity, TemplateCollaborationUserValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Account = entity.Account;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.UserType = entity.UserType;
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.IsValid = entity.IsValid;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TemplateCollaboration = entity.TemplateCollaboration;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }
    }
}
