using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives
{
    public class DocumentSeriesItemLogValidatorMapper : BaseMapper<DocumentSeriesItemLog, DocumentSeriesItemLogValidator>, IDocumentSeriesItemLogValidatorMapper
    {
        public DocumentSeriesItemLogValidatorMapper() { }

        public override DocumentSeriesItemLogValidator Map(DocumentSeriesItemLog entity, DocumentSeriesItemLogValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.IdDocumentSeriesItem = entity.IdDocumentSeriesItem;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Program = entity.Program;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.UniqueId = entity.UniqueId;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.DocumentSeriesItem = entity.Entity;

            #endregion

            return entityTransformed;
        }
    }
}
