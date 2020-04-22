using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationSignValidatorMapper : BaseMapper<CollaborationSign, CollaborationSignValidator>, ICollaborationSignValidatorMapper
    {
        public CollaborationSignValidatorMapper() { }

        public override CollaborationSignValidator Map(CollaborationSign entity, CollaborationSignValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdStatus = entity.IdStatus;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IsRequired = entity.IsRequired;
            entityTransformed.SignDate = entity.SignDate;
            entityTransformed.SignEmail = entity.SignEmail;
            entityTransformed.SignName = entity.SignName;
            entityTransformed.SignUser = entity.SignUser;
            entityTransformed.IsAbsent = entity.IsAbsent;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            #endregion

            return entityTransformed;
        }

    }
}
