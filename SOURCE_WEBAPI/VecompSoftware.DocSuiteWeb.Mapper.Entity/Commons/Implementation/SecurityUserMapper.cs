using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class SecurityUserMapper : BaseEntityMapper<SecurityUser, SecurityUser>, ISecurityUserMapper
    {
        public SecurityUserMapper()
        {

        }

        public override SecurityUser Map(SecurityUser entity, SecurityUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = entity.Account;
            entityTransformed.Description = entity.Description;
            entityTransformed.UserDomain = entity.UserDomain;
            #endregion

            return entityTransformed;
        }

    }
}
