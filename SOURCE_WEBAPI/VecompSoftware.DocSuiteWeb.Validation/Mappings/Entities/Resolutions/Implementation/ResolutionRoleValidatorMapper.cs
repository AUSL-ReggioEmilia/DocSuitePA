using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionRoleValidatorMapper : BaseMapper<ResolutionRole, ResolutionRoleValidator>, IResolutionRoleValidatorMapper
    {
        public ResolutionRoleValidatorMapper() { }

        public override ResolutionRoleValidator Map(ResolutionRole entity, ResolutionRoleValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.IdResolutionRoleType = entity.IdResolutionRoleType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Role = entity.Role;

            #endregion

            return entityTransformed;
        }

    }
}
