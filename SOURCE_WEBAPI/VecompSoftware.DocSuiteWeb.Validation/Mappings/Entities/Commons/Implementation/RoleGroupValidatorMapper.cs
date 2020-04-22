using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class RoleGroupValidatorMapper : BaseMapper<RoleGroup, RoleGroupValidator>, IRoleGroupValidatorMapper
    {
        public RoleGroupValidatorMapper() { }

        public override RoleGroupValidator Map(RoleGroup entity, RoleGroupValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.GroupName = entity.GroupName;
            entityTransformed.ProtocolRights = entity.ProtocolRights;
            entityTransformed.ResolutionRights = entity.ResolutionRights;
            entityTransformed.DocumentRights = entity.DocumentRights;
            entityTransformed.DocumentSeriesRights = entity.DocumentSeriesRights;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.FascicleRights = entity.FascicleRights;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Role = entity.Role;
            entityTransformed.SecurityGroup = entity.SecurityGroup;
            #endregion

            return entityTransformed;
        }

    }
}
