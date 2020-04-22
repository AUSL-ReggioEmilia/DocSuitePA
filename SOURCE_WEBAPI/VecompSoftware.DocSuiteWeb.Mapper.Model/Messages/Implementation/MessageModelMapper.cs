using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Messages
{
    public class MessageModelMapper : BaseModelMapper<Message, MessageModel>, IMessageModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapper;
        #endregion

        #region [ Constructor ]
        public MessageModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapper = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override MessageModel Map(Message entity, MessageModel entityTransformed)
        {
            entityTransformed.IdMessage = entity.EntityId;
            entityTransformed.MessageType = (DocSuiteWeb.Model.Entities.Messages.MessageType)entity.MessageType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Status = (DocSuiteWeb.Model.Entities.Messages.MessageStatus)entity.Status;
            entityTransformed.MessageAttachments = _mapper.Repository<IMessageAttachmentModelMapper>().MapCollection(entity.MessageAttachments);
            entityTransformed.MessageEmails = _mapper.Repository<IMessageEmailModelMapper>().MapCollection(entity.MessageEmails);
            entityTransformed.MessageContacts = _mapper.Repository<IMessageContactModelMapper>().MapCollection(entity.MessageContacts);
            return entityTransformed;
        }
        #endregion
    }
}
