using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives
{
    public class DocumentSeriesConstraintMapper : BaseEntityMapper<DocumentSeriesConstraint, DocumentSeriesConstraint>, IDocumentSeriesConstraintMapper
    {
        public DocumentSeriesConstraintMapper()
        { }

        public override DocumentSeriesConstraint Map(DocumentSeriesConstraint entity, DocumentSeriesConstraint entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            #endregion

            return entityTransformed;
        }

    }
}
