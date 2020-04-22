using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageAttachmentMapper : BaseEntityMapper<MessageAttachment, MessageAttachment>, IMessageAttachmentMapper
    {
        public override MessageAttachment Map(MessageAttachment entity, MessageAttachment entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Server = entity.Server;
            entityTransformed.Archive = entity.Archive;
            entityTransformed.ChainId = entity.ChainId;
            entityTransformed.DocumentEnum = entity.DocumentEnum;
            entityTransformed.Extension = entity.Extension;
            #endregion

            return entityTransformed;
        }
    }
}
