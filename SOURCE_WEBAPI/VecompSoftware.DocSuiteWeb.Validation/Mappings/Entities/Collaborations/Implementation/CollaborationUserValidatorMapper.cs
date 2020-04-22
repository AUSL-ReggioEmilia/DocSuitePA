using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationUserValidatorMapper : BaseMapper<CollaborationUser, CollaborationUserValidator>, ICollaborationUserValidatorMapper
    {
        public CollaborationUserValidatorMapper() { }

        public override CollaborationUserValidator Map(CollaborationUser entity, CollaborationUserValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DestinationEmail = entity.DestinationEmail;
            entityTransformed.DestinationFirst = entity.DestinationFirst;
            entityTransformed.DestinationName = entity.DestinationName;
            entityTransformed.DestinationType = entity.DestinationType;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.Account = entity.Account;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Collaboration;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }

    }
}
