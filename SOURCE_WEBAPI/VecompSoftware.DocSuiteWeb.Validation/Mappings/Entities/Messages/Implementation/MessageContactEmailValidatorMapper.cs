using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageContactEmailValidatorMapper : BaseMapper<MessageContactEmail, MessageContactEmailValidator>, IMessageContactEmailValidatorMapper
    {
        public MessageContactEmailValidatorMapper() { }

        public override MessageContactEmailValidator Map(MessageContactEmail entity, MessageContactEmailValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Description = entity.Description;
            entityTransformed.Email = entity.Email;
            entityTransformed.User = entity.User;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.MessageContact = entity.MessageContact;
            #endregion
            return entityTransformed;
        }

    }
}
