using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives
{
    public class DocumentSeriesMapper : BaseEntityMapper<DocumentSeries, DocumentSeries>, IDocumentSeriesMapper
    {
        public DocumentSeriesMapper()
        { }

        public override DocumentSeries Map(DocumentSeries entity, DocumentSeries entityTransformed)
        {
            #region [ Base ]
            entityTransformed.PublicationEnabled = entity.PublicationEnabled;
            entityTransformed.AttributeSorting = entity.AttributeSorting;
            entityTransformed.AttributeCache = entity.AttributeCache;
            entityTransformed.SubsectionEnabled = entity.SubsectionEnabled;
            entityTransformed.IdDocumentSeriesFamily = entity.IdDocumentSeriesFamily;
            entityTransformed.RoleEnabled = entity.RoleEnabled;
            entityTransformed.SortOrder = entity.SortOrder;
            entityTransformed.AllowAddDocument = entity.AllowAddDocument;
            entityTransformed.AllowNoDocument = entity.AllowNoDocument;
            #endregion

            return entityTransformed;
        }

    }
}
