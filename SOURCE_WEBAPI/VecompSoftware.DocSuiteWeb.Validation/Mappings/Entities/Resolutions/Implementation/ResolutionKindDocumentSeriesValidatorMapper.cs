using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionKindDocumentSeriesValidatorMapper : BaseMapper<ResolutionKindDocumentSeries, ResolutionKindDocumentSeriesValidator>, IResolutionKindDocumentSeriesValidatorMapper
    {
        public ResolutionKindDocumentSeriesValidatorMapper() { }

        public override ResolutionKindDocumentSeriesValidator Map(ResolutionKindDocumentSeries entity, ResolutionKindDocumentSeriesValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DocumentRequired = entity.DocumentRequired;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.ResolutionKind = entity.ResolutionKind;
            entityTransformed.DocumentSeries = entity.DocumentSeries;
            entityTransformed.DocumentSeriesConstraint = entity.DocumentSeriesConstraint;
            #endregion

            return entityTransformed;
        }

    }
}
