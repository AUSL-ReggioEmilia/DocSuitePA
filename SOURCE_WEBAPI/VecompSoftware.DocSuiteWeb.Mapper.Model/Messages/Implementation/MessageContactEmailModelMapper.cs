using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Messages
{
    public class MessageContactEmailModelMapper : BaseModelMapper<MessageContactEmail, MessageContactEmailModel>, IMessageContactEmailModelMapper
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public MessageContactEmailModelMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public override MessageContactEmailModel Map(MessageContactEmail entity, MessageContactEmailModel entityTransformed)
        {
            entityTransformed.IdMessageContactEmail = entity.EntityId;
            entityTransformed.Description = entity.Description;
            entityTransformed.Email = entity.Email;
            entityTransformed.User = entity.User;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            return entityTransformed;
        }
        #endregion
    }
}
