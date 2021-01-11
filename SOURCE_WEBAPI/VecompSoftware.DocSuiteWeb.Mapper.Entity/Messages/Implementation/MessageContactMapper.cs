using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageContactMapper : BaseEntityMapper<MessageContact, MessageContact>, IMessageContactMapper
    {
        public override MessageContact Map(MessageContact entity, MessageContact entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Description = entity.Description;
            entityTransformed.ContactPosition = entity.ContactPosition;
            entityTransformed.ContactType = entity.ContactType;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
