using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Messages
{
    public class MessageEmailModelMapper : BaseModelMapper<MessageEmail, MessageEmailModel>, IMessageEmailModelMapper
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MessageEmailModelMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public override MessageEmailModel Map(MessageEmail entity, MessageEmailModel entityTransformed)
        {
            entityTransformed.Body = entity.Body;
            entityTransformed.EmlDocumentId = entity.EmlDocumentId;
            entityTransformed.IdMessageEmail = entity.EntityId;
            entityTransformed.IsDispositionNotification = entity.IsDispositionNotification;
            entityTransformed.Priority = entity.Priority;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SentDate = entity.SentDate;
            entityTransformed.Subject = entity.Subject;
            return entityTransformed;
        }
        #endregion
    }
}
