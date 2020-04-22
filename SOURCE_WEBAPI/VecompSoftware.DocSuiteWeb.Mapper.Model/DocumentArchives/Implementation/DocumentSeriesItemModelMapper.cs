using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentArchives
{
    public class DocumentSeriesItemModelMapper : BaseModelMapper<DocumentSeriesItem, DocumentSeriesItemModel>, IDocumentSeriesItemModelMapper
    {
        public override DocumentSeriesItemModel Map(DocumentSeriesItem entity, DocumentSeriesItemModel entityTransformed)
        {
            entityTransformed.IdDocumentSeriesItem = entity.EntityId;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;

            return entityTransformed;
        }

    }
}
