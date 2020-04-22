using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolRoleUserValidatorMapper : BaseMapper<ProtocolRoleUser, ProtocolRoleUserValidator>, IProtocolRoleUserValidatorMapper
    {
        public ProtocolRoleUserValidatorMapper() { }

        public override ProtocolRoleUserValidator Map(ProtocolRoleUser entity, ProtocolRoleUserValidator entityTransformed)
        {
            #region[ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.UserName = entity.UserName;
            entityTransformed.Account = entity.Account;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.Status = entity.Status;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Protocol = entity.Protocol;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }
    }
}
