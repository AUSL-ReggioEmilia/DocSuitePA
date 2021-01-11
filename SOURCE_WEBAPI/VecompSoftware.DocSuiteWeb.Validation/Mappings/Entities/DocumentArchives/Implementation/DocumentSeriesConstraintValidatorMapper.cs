using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives
{
    public class DocumentSeriesConstraintValidatorMapper : BaseMapper<DocumentSeriesConstraint, DocumentSeriesConstraintValidator>, IDocumentSeriesConstraintValidatorMapper
    {
        public DocumentSeriesConstraintValidatorMapper() { }

        public override DocumentSeriesConstraintValidator Map(DocumentSeriesConstraint entity, DocumentSeriesConstraintValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentSeries = entity.DocumentSeries;
            entityTransformed.ResolutionKindDocumentSeries = entity.ResolutionKindDocumentSeries;
            #endregion

            return entityTransformed;
        }

    }
}
