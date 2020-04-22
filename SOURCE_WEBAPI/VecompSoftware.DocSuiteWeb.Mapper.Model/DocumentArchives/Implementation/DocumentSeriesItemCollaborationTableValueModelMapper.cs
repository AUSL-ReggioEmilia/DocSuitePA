using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentArchives
{
    public class DocumentSeriesItemCollaborationTableValueModelMapper : BaseModelMapper<CollaborationTableValuedModel, DocumentSeriesItemModel>, IDocumentSeriesItemCollaborationTableValueModelMapper
    {
        public override DocumentSeriesItemModel Map(CollaborationTableValuedModel entity, DocumentSeriesItemModel entityTransformed)
        {
            entityTransformed = null;
            if (entity.DocumentSeriesItem_IdDocumentSeriesItem.HasValue)
            {
                entityTransformed = new DocumentSeriesItemModel
                {
                    IdDocumentSeriesItem = entity.DocumentSeriesItem_IdDocumentSeriesItem,
                    Number = entity.DocumentSeriesItem_Number,
                    Year = entity.DocumentSeriesItem_Year
                };
            }

            return entityTransformed;
        }

    }
}
