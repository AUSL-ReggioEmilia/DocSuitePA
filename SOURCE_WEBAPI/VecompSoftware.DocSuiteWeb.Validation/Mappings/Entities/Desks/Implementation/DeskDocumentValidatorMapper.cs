using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks
{
    public class DeskDocumentValidatorMapper : BaseMapper<DeskDocument, DeskDocumentValidator>, IDeskDocumentValidatorMapper
    {
        public DeskDocumentValidatorMapper() { }

        public override DeskDocumentValidator Map(DeskDocument entity, DeskDocumentValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IsActive = entity.IsActive;
            entityTransformed.IdDocument = entity.IdDocument;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Desk = entity.Desk;
            entityTransformed.DeskDocumentVersions = entity.DeskDocumentVersions;

            #endregion

            return entityTransformed;
        }

    }
}
