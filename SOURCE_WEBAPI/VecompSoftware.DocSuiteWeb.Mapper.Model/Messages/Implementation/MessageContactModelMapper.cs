using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Messages
{
    public class MessageContactModelMapper : BaseModelMapper<MessageContact, MessageContactModel>, IMessageContactModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapper;
        #endregion

        #region [ Constructor ]
        public MessageContactModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapper = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]
        public override MessageContactModel Map(MessageContact entity, MessageContactModel entityTransformed)
        {
            entityTransformed.IdMessageContact = entity.EntityId;
            entityTransformed.ContactPosition = (DocSuiteWeb.Model.Entities.Messages.MessageContantTypology)entity.ContactPosition;
            entityTransformed.ContactType = (DocSuiteWeb.Model.Entities.Messages.MessageContactType)entity.ContactType;
            entityTransformed.Description = entity.Description;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.MessageContactEmail = _mapper.Repository<IMessageContactEmailModelMapper>().MapCollection(entity.MessageContactEmail);
            return entityTransformed;
        }
        #endregion
    }
}
