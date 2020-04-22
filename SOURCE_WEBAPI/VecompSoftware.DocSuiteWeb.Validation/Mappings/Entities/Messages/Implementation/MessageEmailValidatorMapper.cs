using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageEmailValidatorMapper : BaseMapper<MessageEmail, MessageEmailValidator>, IMessageEmailValidatorMapper
    {
        public MessageEmailValidatorMapper() { }

        public override MessageEmailValidator Map(MessageEmail entity, MessageEmailValidator entityTransformed)
        {
            #region [ Base ]            
            entityTransformed.Body = entity.Body;
            entityTransformed.Priority = entity.Priority;
            entityTransformed.EmlDocumentId = entity.EmlDocumentId;
            entityTransformed.IsDispositionNotification = entity.IsDispositionNotification;
            entityTransformed.SentDate = entity.SentDate;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Base ]
            entityTransformed.Message = entity.Message;
            entityTransformed.DeskMessages = entity.DeskMessages;
            #endregion

            return entityTransformed;
        }

    }
}
