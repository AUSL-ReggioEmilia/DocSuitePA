using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class SecurityUserValidatorMapper : BaseMapper<SecurityUser, SecurityUserValidator>, ISecurityUserValidatorMapper
    {
        public SecurityUserValidatorMapper() { }

        public override SecurityUserValidator Map(SecurityUser entity, SecurityUserValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Account = entity.Account;
            entityTransformed.Description = entity.Description;
            entityTransformed.UserDomain = entity.UserDomain;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Group = entity.Group;
            entityTransformed.DeskRoleUsers = entity.DeskRoleUsers;
            #endregion

            return entityTransformed;
        }

    }
}
