using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class RoleUserValidatorMapper : BaseMapper<RoleUser, RoleUserValidator>, IRoleUserValidatorMapper
    {
        public RoleUserValidatorMapper() { }

        public override RoleUserValidator Map(RoleUser entity, RoleUserValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Account = entity.Account;
            entityTransformed.Description = entity.Description;
            entityTransformed.DSWEnvironment = entity.DSWEnvironment;
            entityTransformed.Email = entity.Email;
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.IsMainRole = entity.IsMainRole;
            entityTransformed.Type = entity.Type;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Role = entity.Role;

            #endregion

            return entityTransformed;
        }

    }
}
