using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates
{
    public class TemplateCollaborationMapper : BaseEntityMapper<TemplateCollaboration, TemplateCollaboration>, ITemplateCollaborationMapper
    {
        public override TemplateCollaboration Map(TemplateCollaboration entity, TemplateCollaboration entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IdPriority = entity.IdPriority;
            entityTransformed.Object = entity.Object;
            entityTransformed.Note = entity.Note;
            entityTransformed.IsLocked = entity.IsLocked;
            entityTransformed.WSDeletable = entity.WSDeletable;
            entityTransformed.WSManageable = entity.WSManageable;
            entityTransformed.JsonParameters = entity.JsonParameters;
            #endregion

            return entityTransformed;
        }
    }
}
