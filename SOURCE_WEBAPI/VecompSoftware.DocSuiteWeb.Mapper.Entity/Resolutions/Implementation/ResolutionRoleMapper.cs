using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionRoleMapper : BaseEntityMapper<ResolutionRole, ResolutionRole>, IResolutionRoleMapper
    {
        public override ResolutionRole Map(ResolutionRole entity, ResolutionRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdResolutionRoleType = entity.IdResolutionRoleType;
            #endregion

            return entityTransformed;
        }
    }
}
