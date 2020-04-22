using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolParerValidatorMapper : BaseMapper<ProtocolParer, ProtocolParerValidator>, IProtocolParerValidatorMapper
    {
        public ProtocolParerValidatorMapper() { }

        public override ProtocolParerValidator Map(ProtocolParer entity, ProtocolParerValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.ArchiviedDate = entity.ArchiviedDate;
            entityTransformed.IsForArchive = entity.IsForArchive;
            entityTransformed.ParerUri = entity.ParerUri;
            entityTransformed.HasError = entity.HasError;
            entityTransformed.LastError = entity.LastError;
            entityTransformed.LastSendDate = entity.LastSendDate;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Protocol = entity.Protocol;

            #endregion

            return entityTransformed;
        }

    }
}
