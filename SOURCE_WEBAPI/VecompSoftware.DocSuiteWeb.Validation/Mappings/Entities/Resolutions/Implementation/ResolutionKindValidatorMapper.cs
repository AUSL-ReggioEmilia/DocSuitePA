using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionKindValidatorMapper : BaseMapper<ResolutionKind, ResolutionKindValidator>, IResolutionKindValidatorMapper
    {
        public ResolutionKindValidatorMapper() { }

        public override ResolutionKindValidator Map(ResolutionKind entity, ResolutionKindValidator entityTransformed)
        {

            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.AmountEnabled = entity.AmountEnabled;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Resolutions = entity.Resolutions;
            entityTransformed.ResolutionKindDocumentSeries = entity.ResolutionKindDocumentSeries;
            #endregion

            return entityTransformed;
        }

    }
}
