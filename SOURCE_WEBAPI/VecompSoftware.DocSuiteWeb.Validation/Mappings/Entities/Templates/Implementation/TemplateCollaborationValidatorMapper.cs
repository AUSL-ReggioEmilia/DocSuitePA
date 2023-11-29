using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates
{
    public class TemplateCollaborationValidatorMapper : BaseMapper<TemplateCollaboration, TemplateCollaborationValidator>, ITemplateCollaborationValidatorMapper
    {
        public TemplateCollaborationValidatorMapper() { }
        public override TemplateCollaborationValidator Map(TemplateCollaboration entity, TemplateCollaborationValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IdPriority = entity.IdPriority;
            entityTransformed.Object = entity.Object;
            entityTransformed.Note = entity.Note;
            entityTransformed.IsLocked = entity.IsLocked;
            entityTransformed.WSManageable = entity.WSManageable;
            entityTransformed.WSDeletable = entity.WSDeletable;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.JsonParameters = entity.JsonParameters;
            entityTransformed.TemplateCollaborationPath = entity.TemplateCollaborationPath;
            entityTransformed.TemplateCollaborationLevel = entity.TemplateCollaborationLevel;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            entityTransformed.RepresentationType = entity.RepresentationType;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Roles = entity.Roles;
            entityTransformed.TemplateCollaborationUsers = entity.TemplateCollaborationUsers;
            entityTransformed.TemplateCollaborationDocumentRepositories = entity.TemplateCollaborationDocumentRepositories;
            #endregion
            return entityTransformed;
        }
    }
}
