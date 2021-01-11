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
            entityTransformed.ActiveFrom = entity.ActiveFrom;
            entityTransformed.ActiveTo = entity.ActiveTo;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.Collapsed = entity.Collapsed;
            entityTransformed.EMailAddress = entity.EMailAddress;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.IdRoleTenant = entity.IdRoleTenant;
            entityTransformed.TenantId = entity.TenantId;
            #endregion

            return entityTransformed;
        }

    }
}
