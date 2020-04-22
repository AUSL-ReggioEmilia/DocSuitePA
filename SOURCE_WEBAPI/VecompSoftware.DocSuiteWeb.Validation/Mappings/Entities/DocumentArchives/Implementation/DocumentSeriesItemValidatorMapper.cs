using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives
{
    public class DocumentSeriesItemValidatorMapper : BaseMapper<DocumentSeriesItem, DocumentSeriesItemValidator>, IDocumentSeriesItemValidatorMapper
    {
        public DocumentSeriesItemValidatorMapper() { }

        public override DocumentSeriesItemValidator Map(DocumentSeriesItem entity, DocumentSeriesItemValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.UniqueId = entity.UniqueId;
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
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.DematerialisationChainId = entity.DematerialisationChainId;
            entityTransformed.HasMainDocument = entity.HasMainDocument;
            entityTransformed.ConstraintValue = entity.ConstraintValue;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Category = entity.Category;
            entityTransformed.Location = entity.Location;
            entityTransformed.LocationAnnexed = entity.LocationAnnexed;
            entityTransformed.LocationUnpublishedAnnexed = entity.LocationUnpublishedAnnexed;
            entityTransformed.DocumentSeries = entity.DocumentSeries;
            entityTransformed.DocumentSeriesItemLogs = entity.DocumentSeriesItemLogs;

            #endregion

            return entityTransformed;
        }

    }
}
