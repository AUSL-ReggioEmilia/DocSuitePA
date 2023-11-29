using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Commons
{
    public class RoleFullTableValuedModelMapper : BaseModelMapper<RoleFullTableValuedModel, RoleModel>, IRoleFullTableValuedModelMapper
    {
        public override RoleModel Map(RoleFullTableValuedModel entity, RoleModel entityTransformed)
        {
            entityTransformed.EntityShortId = entity.IdRole;
            entityTransformed.IdRole = entity.IdRole;
            entityTransformed.IdRoleFather = entity.RoleParent_IdRole;
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.FullIncrementalPath = entity.FullIncrementalPath;
            entityTransformed.ServiceCode = entity.ServiceCode;
            entityTransformed.EMailAddress = entity.EMailAddress;
            entityTransformed.Collapsed = entity.Collapsed;
            entityTransformed.RoleTypology = entity.RoleTypology;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.IdTenantAOO = entity.IdTenantAOO;
            entityTransformed.IsRealResult = entity.IsRealResult;

            return entityTransformed;
        }
    }
}
