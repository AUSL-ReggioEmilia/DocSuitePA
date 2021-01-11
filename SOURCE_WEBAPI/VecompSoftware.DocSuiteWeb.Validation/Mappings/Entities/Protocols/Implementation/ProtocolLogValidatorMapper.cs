using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolLogValidatorMapper : BaseMapper<ProtocolLog, ProtocolLogValidator>, IProtocolLogValidatorMapper
    {
        public ProtocolLogValidatorMapper() { }

        public override ProtocolLogValidator Map(ProtocolLog entity, ProtocolLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Program = entity.Program;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Entity;

            #endregion

            return entityTransformed;
        }

    }
}
