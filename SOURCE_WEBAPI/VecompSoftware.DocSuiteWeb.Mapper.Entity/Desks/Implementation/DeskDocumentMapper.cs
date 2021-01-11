using VecompSoftware.DocSuiteWeb.Entity.Desks;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Desks
{
    public class DeskDocumentMapper : BaseEntityMapper<DeskDocument, DeskDocument>, IDeskDocumentMapper
    {
        public DeskDocumentMapper()
        { }

        public override DeskDocument Map(DeskDocument entity, DeskDocument entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdDocument = entity.IdDocument;
            entityTransformed.DocumentType = entity.DocumentType;
            entityTransformed.IsActive = entity.IsActive;
            #endregion

            return entityTransformed;
        }

    }
}
