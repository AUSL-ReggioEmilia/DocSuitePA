using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionContactMapper : BaseEntityMapper<ResolutionContact, ResolutionContact>, IResolutionContactMapper
    {
        public override ResolutionContact Map(ResolutionContact entity, ResolutionContact entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.ComunicationType = entity.ComunicationType;

            #endregion

            return entityTransformed;
        }
    }
}
