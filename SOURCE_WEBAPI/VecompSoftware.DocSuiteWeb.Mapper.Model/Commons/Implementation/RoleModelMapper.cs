using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class RoleModelMapper : BaseModelMapper<Role, RoleModel>, IRoleModelMapper
    {
        public override RoleModel Map(Role entity, RoleModel entityTransformed)
        {
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.IdRole = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.Collapsed = entity.Collapsed;
            entityTransformed.EMailAddress = entity.EMailAddress;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.RoleTypology = (DocSuiteWeb.Model.Entities.Commons.RoleTypology)entity.RoleTypology;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.IdTenantAOO = entity.TenantAOO.UniqueId;
            entityTransformed.IsActive = entity.IsActive;

            return entityTransformed;
        }

    }
}
