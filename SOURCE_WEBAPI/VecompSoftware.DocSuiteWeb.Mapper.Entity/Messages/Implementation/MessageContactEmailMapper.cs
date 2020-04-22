using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageContactEmailMapper : BaseEntityMapper<MessageContactEmail, MessageContactEmail>, IMessageContactEmailMapper
    {
        public override MessageContactEmail Map(MessageContactEmail entity, MessageContactEmail entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Email = entity.Email;
            entityTransformed.User = entity.User;
            entityTransformed.Description = entity.Description;
            #endregion

            return entityTransformed;
        }
    }
}
