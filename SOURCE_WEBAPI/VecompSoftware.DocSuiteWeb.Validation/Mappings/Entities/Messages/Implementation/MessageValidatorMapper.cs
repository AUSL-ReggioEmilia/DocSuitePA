using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageValidatorMapper : BaseMapper<Message, MessageValidator>, IMessageValidatorMapper
    {
        public MessageValidatorMapper() { }

        public override MessageValidator Map(Message entity, MessageValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Status = entity.Status;
            entityTransformed.MessageType = entity.MessageType;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Base ]
            entityTransformed.Location = entity.Location;
            entityTransformed.MessageEmails = entity.MessageEmails;
            entityTransformed.MessageAttachments = entity.MessageAttachments;
            entityTransformed.MessageLogs = entity.MessageLogs;
            entityTransformed.MessageContacts = entity.MessageContacts;
            entityTransformed.Protocols = entity.Protocols;
            entityTransformed.Dossiers = entity.Dossiers;
            #endregion

            return entityTransformed;
        }

    }
}
