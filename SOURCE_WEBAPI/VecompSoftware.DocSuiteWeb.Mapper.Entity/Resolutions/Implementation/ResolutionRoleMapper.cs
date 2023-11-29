using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionRoleMapper : BaseEntityMapper<ResolutionRole, ResolutionRole>, IResolutionRoleMapper
    {
        public override ResolutionRole Map(ResolutionRole entity, ResolutionRole entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdResolutionRoleType = entity.IdResolutionRoleType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            return entityTransformed;
        }
    }
}
