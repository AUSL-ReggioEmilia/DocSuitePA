using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Resolutions
{
    public class ResolutionRoleModelMapper : BaseModelMapper<ResolutionRole, ResolutionRoleModel>, IResolutionRoleModelMapper
    {

        #region [ Fields ]
        private readonly IRoleModelMapper _roleModelMapper;
        #endregion

        #region [ Constructor ]
        public ResolutionRoleModelMapper(IRoleModelMapper roleModelMapper)
        {
            _roleModelMapper = roleModelMapper;
        }
        #endregion


        public override ResolutionRoleModel Map(ResolutionRole entity, ResolutionRoleModel entityTransformed)
        {
            entityTransformed.IdResolution = entity.EntityId;
            entityTransformed.Role = _roleModelMapper.Map(entity.Role, new RoleModel());
            entityTransformed.IdResolutionRoleType = entity.IdResolutionRoleType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;

            return entityTransformed;
        }
    }
}
