using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionKindDocumentSeriesMapper : BaseEntityMapper<ResolutionKindDocumentSeries, ResolutionKindDocumentSeries>, IResolutionKindDocumentSeriesMapper
    {
        public override ResolutionKindDocumentSeries Map(ResolutionKindDocumentSeries entity, ResolutionKindDocumentSeries entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DocumentRequired = entity.DocumentRequired;
            #endregion

            return entityTransformed;
        }
    }
}
