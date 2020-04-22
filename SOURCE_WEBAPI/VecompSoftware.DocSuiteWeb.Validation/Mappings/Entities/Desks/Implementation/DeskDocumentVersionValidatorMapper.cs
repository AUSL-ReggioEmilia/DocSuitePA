using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskDocumentVersionValidatorMapper : BaseMapper<DeskDocumentVersion, DeskDocumentVersionValidator>, IDeskDocumentVersionValidatorMapper
    {
        public DeskDocumentVersionValidatorMapper() { }

        public override DeskDocumentVersionValidator Map(DeskDocumentVersion entity, DeskDocumentVersionValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Version = entity.Version;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DeskDocument = entity.DeskDocument;
            entityTransformed.DeskDocumentEndorsements = entity.DeskDocumentEndorsements;
            entityTransformed.DeskStoryBoards = entity.DeskStoryBoards;

            #endregion

            return entityTransformed;
        }

    }
}
