using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationVersioningValidatorMapper : BaseMapper<CollaborationVersioning, CollaborationVersioningValidator>, ICollaborationVersioningValidatorMapper
    {
        public CollaborationVersioningValidatorMapper() { }

        public override CollaborationVersioningValidator Map(CollaborationVersioning entity, CollaborationVersioningValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.CheckedOut = entity.CheckedOut;
            entityTransformed.CheckOutDate = entity.CheckOutDate;
            entityTransformed.CheckOutUser = entity.CheckOutUser;
            entityTransformed.CheckOutSessionId = entity.CheckOutSessionId;
            entityTransformed.DocumentChecksum = entity.DocumentChecksum;
            entityTransformed.DocumentGroup = entity.DocumentGroup;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            #endregion

            return entityTransformed;
        }

    }
}
