using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Messages
{
    public class MessageAttachmentModelMapper : BaseModelMapper<MessageAttachment, MessageAttachmentModel>, IMessageAttachmentModelMapper
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MessageAttachmentModelMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public override MessageAttachmentModel Map(MessageAttachment entity, MessageAttachmentModel entityTransformed)
        {
            entityTransformed.IdMessageAttachment = entity.EntityId;
            entityTransformed.Archive = entity.Archive;
            entityTransformed.ChainId = entity.ChainId;
            entityTransformed.DocumentEnum = entity.DocumentEnum;
            entityTransformed.Extension = entity.Extension;
            entityTransformed.Server = entity.Server;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            return entityTransformed;
        }
        #endregion
    }
}
