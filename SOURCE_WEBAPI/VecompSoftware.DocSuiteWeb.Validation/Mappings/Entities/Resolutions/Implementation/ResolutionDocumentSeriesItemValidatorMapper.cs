using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionDocumentSeriesItemValidatorMapper : BaseMapper<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItemValidator>, IResolutionDocumentSeriesItemValidatorMapper
    {
        public ResolutionDocumentSeriesItemValidatorMapper(){ }

        public override ResolutionDocumentSeriesItemValidator Map(ResolutionDocumentSeriesItem entity, ResolutionDocumentSeriesItemValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentSeriesItems = entity.DocumentSeriesItem;
            entityTransformed.Resolutions = entity.Resolution;
            #endregion

            return entityTransformed;
        }
    }
}
