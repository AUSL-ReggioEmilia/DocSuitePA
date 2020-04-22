using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class RoleFullTableValuedModelMapper : BaseModelMapper<RoleFullTableValuedModel, RoleModel>, IRoleFullTableValuedModelMapper
    {
        public override RoleModel Map(RoleFullTableValuedModel entity, RoleModel entityTransformed)
        {
            entityTransformed.IdRole = entity.IdRole;
            entityTransformed.IdRoleFather = entity.RoleParent_IdRole;
            entityTransformed.IdRoleTenant = entity.IdRoleTenant;
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.TenantId = entity.TenantId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.ActiveFrom = entity.ActiveFrom;

            return entityTransformed;
        }
    }
}
