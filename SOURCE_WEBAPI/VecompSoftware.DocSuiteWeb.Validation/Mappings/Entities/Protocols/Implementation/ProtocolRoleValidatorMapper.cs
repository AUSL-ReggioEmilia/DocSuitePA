using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolRoleValidatorMapper : BaseMapper<ProtocolRole, ProtocolRoleValidator>, IProtocolRoleValidatorMapper
    {
        public ProtocolRoleValidatorMapper() { }

        public override ProtocolRoleValidator Map(ProtocolRole entity, ProtocolRoleValidator entityTransformed)
        {
            #region[ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Rights = entity.Rights;
            entityTransformed.Note = entity.Note;
            entityTransformed.DistributionType = entity.DistributionType;
            entityTransformed.Type = entity.Type;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Role = entity.Role;
            entityTransformed.Protocol = entity.Protocol;
            #endregion

            return entityTransformed;
        }
    }
}
