using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class RoleModelMapper : BaseModelMapper<Role, RoleModel>, IRoleModelMapper
    {
        public override RoleModel Map(Role entity, RoleModel entityTransformed)
        {
            entityTransformed.IdRole = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.ActiveFrom = entity.ActiveFrom;

            return entityTransformed;
        }

    }
}
