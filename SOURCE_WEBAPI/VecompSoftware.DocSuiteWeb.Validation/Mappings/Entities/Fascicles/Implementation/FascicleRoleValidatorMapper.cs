using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleRoleValidatorMapper : BaseMapper<FascicleRole, FascicleRoleValidator>, IFascicleRoleValidatorMapper
    {
        public FascicleRoleValidatorMapper() { }

        public override FascicleRoleValidator Map(FascicleRole entity, FascicleRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.RoleAuthorizationType = entity.AuthorizationRoleType;
            entityTransformed.Timestamp = entity.Timestamp;
            entityTransformed.IsMaster = entity.IsMaster;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }

    }
}
