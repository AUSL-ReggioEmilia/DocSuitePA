using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolLinkValidatorMapper : BaseMapper<ProtocolLink, ProtocolLinkValidator>, IProtocolLinkValidatorMapper
    {
        public ProtocolLinkValidatorMapper() { }

        public override ProtocolLinkValidator Map(ProtocolLink entity, ProtocolLinkValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.LinkType = entity.LinkType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Protocol;
            entityTransformed.ProtocolLinked = entity.ProtocolLinked;
            #endregion

            return entityTransformed;
        }

    }
}
