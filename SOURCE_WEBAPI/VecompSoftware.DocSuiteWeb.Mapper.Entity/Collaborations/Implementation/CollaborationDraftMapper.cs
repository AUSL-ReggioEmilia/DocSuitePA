using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationDraftMapper : BaseEntityMapper<CollaborationDraft, CollaborationDraft>, ICollaborationDraftMapper
    {
        public override CollaborationDraft Map(CollaborationDraft entity, CollaborationDraft entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Data = entity.Data;
            entityTransformed.DraftType = entity.DraftType;
            #endregion

            return entityTransformed;
        }

    }
}
