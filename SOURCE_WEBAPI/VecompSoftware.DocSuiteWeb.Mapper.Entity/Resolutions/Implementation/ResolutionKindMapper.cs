using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionKindMapper : BaseEntityMapper<ResolutionKind, ResolutionKind>, IResolutionKindMapper
    {
        public override ResolutionKind Map(ResolutionKind entity, ResolutionKind entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.AmountEnabled = entity.AmountEnabled;
            #endregion

            return entityTransformed;
        }
    }
}
