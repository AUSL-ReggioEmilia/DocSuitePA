using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages
{
    public class MessageLogValidatorMapper : BaseMapper<MessageLog, MessageLogValidator>, IMessageLogValidatorMapper
    {
        public MessageLogValidatorMapper() { }

        public override MessageLogValidator Map(MessageLog entity, MessageLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Message = entity.Entity;
            #endregion


            return entityTransformed;
        }

    }
}
