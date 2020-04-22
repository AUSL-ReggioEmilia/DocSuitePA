using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Messages
{
    public class MessageLogMapper : BaseEntityMapper<MessageLog, MessageLog>, IMessageLogMapper
    {
        public override MessageLog Map(MessageLog entity, MessageLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.Severity = entity.Severity;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }

    }
}
