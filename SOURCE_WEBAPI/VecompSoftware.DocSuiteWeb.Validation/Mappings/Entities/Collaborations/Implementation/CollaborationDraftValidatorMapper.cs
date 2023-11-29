using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationDraftValidatorMapper : BaseMapper<CollaborationDraft, CollaborationDraftValidator>, ICollaborationDraftValidatorMapper
    {
        public CollaborationDraftValidatorMapper() { }

        public override CollaborationDraftValidator Map(CollaborationDraft entity, CollaborationDraftValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Description = entity.Description;
            entityTransformed.Data = entity.Data;
            entityTransformed.DraftType = entity.DraftType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            #endregion

            return entityTransformed;
        }

    }
}
