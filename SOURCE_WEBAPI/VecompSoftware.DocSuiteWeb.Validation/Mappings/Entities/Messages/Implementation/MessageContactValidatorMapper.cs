using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageContactValidatorMapper : BaseMapper<MessageContact, MessageContactValidator>, IMessageContactValidatorMapper
    {
        public MessageContactValidatorMapper() { }

        public override MessageContactValidator Map(MessageContact entity, MessageContactValidator entityTransformed)
        {
            #region [ Base ]            
            entityTransformed.ContactPosition = entity.ContactPosition;
            entityTransformed.ContactType = entity.ContactType;
            entityTransformed.Description = entity.Description;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Base ]
            entityTransformed.Message = entity.Message;
            entityTransformed.MessageContactEmail = entity.MessageContactEmail;
            #endregion



            return entityTransformed;
        }

    }
}
