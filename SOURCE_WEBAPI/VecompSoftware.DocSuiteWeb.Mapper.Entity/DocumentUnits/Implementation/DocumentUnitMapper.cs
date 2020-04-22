using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitMapper : BaseEntityMapper<DocumentUnit, DocumentUnit>, IDocumentUnitMapper
    {
        public DocumentUnitMapper()
        { }

        public override DocumentUnit Map(DocumentUnit entity, DocumentUnit entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DocumentUnitName = entity.DocumentUnitName;
            entityTransformed.Year = entity.Year;
            entityTransformed.Number = entity.Number;
            entityTransformed.Subject = entity.Subject;
            entityTransformed.Title = entity.Title;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.Status = entity.Status;
            #endregion

            return entityTransformed;
        }

    }
}
