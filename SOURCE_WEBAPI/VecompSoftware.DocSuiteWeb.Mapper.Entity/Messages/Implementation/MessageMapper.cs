using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageMapper : BaseEntityMapper<Message, Message>, IMessageMapper
    {
        public override Message Map(Message entity, Message entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Status = entity.Status;
            entityTransformed.MessageType = entity.MessageType;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
