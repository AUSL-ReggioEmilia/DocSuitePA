using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageAttachmentValidatorMapper : BaseMapper<MessageAttachment, MessageAttachmentValidator>, IMessageAttachmentValidatorMapper
    {
        public MessageAttachmentValidatorMapper() { }

        public override MessageAttachmentValidator Map(MessageAttachment entity, MessageAttachmentValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Archive = entity.Archive;
            entityTransformed.ChainId = entity.ChainId;
            entityTransformed.DocumentEnum = entity.DocumentEnum;
            entityTransformed.Extension = entity.Extension;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Message = entity.Message;
            #endregion

            return entityTransformed;
        }

    }
}
