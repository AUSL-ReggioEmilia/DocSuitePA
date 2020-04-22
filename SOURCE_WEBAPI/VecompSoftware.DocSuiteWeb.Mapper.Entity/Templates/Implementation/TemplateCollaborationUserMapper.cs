
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates
{
    public class TemplateCollaborationUserMapper : BaseEntityMapper<TemplateCollaborationUser, TemplateCollaborationUser>, ITemplateCollaborationUserMapper
    {
        public override TemplateCollaborationUser Map(TemplateCollaborationUser entity, TemplateCollaborationUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = entity.Account;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.UserType = entity.UserType;
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.IsValid = entity.IsValid;
            #endregion
            return entityTransformed;
        }
    }
}
