using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives
{
    public class DocumentSeriesItemMapper : BaseEntityMapper<DocumentSeriesItem, DocumentSeriesItem>, IDocumentSeriesItemMapper
    {
        public DocumentSeriesItemMapper()
        { }

        public override DocumentSeriesItem Map(DocumentSeriesItem entity, DocumentSeriesItem entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.IdDocumentSeriesSubsection = entity.IdDocumentSeriesSubsection;
            entityTransformed.IdMain = entity.IdMain;
            entityTransformed.IdAnnexed = entity.IdAnnexed;
            entityTransformed.PublishingDate = entity.PublishingDate;
            entityTransformed.RetireDate = entity.RetireDate;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Status = entity.Status;
            entityTransformed.IdUnpublishedAnnexed = entity.IdUnpublishedAnnexed;
            entityTransformed.Priority = entity.Priority;
            entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;
            entityTransformed.HasMainDocument = entity.HasMainDocument;
            entityTransformed.ConstraintValue = entity.ConstraintValue;

            #endregion

            return entityTransformed;
        }

    }
}
