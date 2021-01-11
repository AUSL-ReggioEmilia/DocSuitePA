using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class RoleUserMapper : BaseEntityMapper<RoleUser, RoleUser>, IRoleUserMapper
    {
        public RoleUserMapper()
        {

        }

        public override RoleUser Map(RoleUser entity, RoleUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Account = entity.Account;
            entityTransformed.Description = entity.Description;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            entityTransformed.Email = entity.Email;
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.IsMainRole = entity.IsMainRole;
            entityTransformed.Type = entity.Type;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            #endregion

            return entityTransformed;
        }

    }
}
