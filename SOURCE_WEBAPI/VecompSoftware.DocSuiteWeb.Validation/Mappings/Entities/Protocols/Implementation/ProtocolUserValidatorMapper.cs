using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolUserValidatorMapper : BaseMapper<ProtocolUser, ProtocolUserValidator>, IProtocolUserValidatorMapper
    {
        public ProtocolUserValidatorMapper() { }

        public override ProtocolUserValidator Map(ProtocolUser entity, ProtocolUserValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Account = entity.Account;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.Type = entity.Type;
            entityTransformed.Note = entity.Note;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Protocol;

            #endregion

            return entityTransformed;
        }

    }
}
