using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolContactValidatorMapper : BaseMapper<ProtocolContact, ProtocolContactValidator>, IProtocolContactValidatorMapper
    {
        public ProtocolContactValidatorMapper() { }

        public override ProtocolContactValidator Map(ProtocolContact entity, ProtocolContactValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.ComunicationType = entity.ComunicationType;
            entityTransformed.Type = entity.Type;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Protocol;
            entityTransformed.Contact = entity.Contact;

            #endregion

            return entityTransformed;
        }

    }
}
