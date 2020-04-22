using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageEmailMapper : BaseEntityMapper<MessageEmail, MessageEmail>, IMessageEmailMapper
    {
        public override MessageEmail Map(MessageEmail entity, MessageEmail entityTransformed)
        {
            #region [ Base ]
            entityTransformed.SentDate = entity.SentDate;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Body = entity.Body;
            entityTransformed.Priority = entity.Priority;
            entityTransformed.EmlDocumentId = entity.EmlDocumentId;
            entityTransformed.IsDispositionNotification = entity.IsDispositionNotification;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
