using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives
{
    public class DocumentSeriesValidatorMapper : BaseMapper<DocumentSeries, DocumentSeriesValidator>, IDocumentSeriesValidatorMapper
    {
        public DocumentSeriesValidatorMapper() { }

        public override DocumentSeriesValidator Map(DocumentSeries entity, DocumentSeriesValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.PublicationEnabled = entity.PublicationEnabled;
            entityTransformed.AttributeSorting = entity.AttributeSorting;
            entityTransformed.AttributeCache = entity.AttributeCache;
            entityTransformed.SubsectionEnabled = entity.SubsectionEnabled;
            entityTransformed.IdDocumentSeriesFamily = entity.IdDocumentSeriesFamily;
            entityTransformed.RoleEnabled = entity.RoleEnabled;
            entityTransformed.SortOrder = entity.SortOrder;
            entityTransformed.AllowAddDocument = entity.AllowAddDocument;
            entityTransformed.AllowNoDocument = entity.AllowNoDocument;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Container = entity.Container;
            entityTransformed.DocumentSeriesItems = entity.DocumentSeriesItems;
            entityTransformed.DocumentSeriesConstraints = entity.DocumentSeriesConstraints;
            entityTransformed.ResolutionKindDocumentSeries = entity.ResolutionKindDocumentSeries;
            #endregion

            return entityTransformed;
        }

    }
}
