using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class RoleMapper : BaseEntityMapper<Role, Role>, IRoleMapper
    {
        public RoleMapper()
        {

        }

        public override Role Map(Role entity, Role entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.Collapsed = entity.Collapsed;
            entityTransformed.EMailAddress = entity.EMailAddress;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.RoleTypology = entity.RoleTypology;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.TenantAOO = entity.TenantAOO;
            #endregion

            return entityTransformed;
        }

    }
}
