using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentArchives
{
    public class DocumentSeriesItemRoleMapper : BaseEntityMapper<DocumentSeriesItemRole, DocumentSeriesItemRole>, IDocumentSeriesItemRoleMapper
    {
        public DocumentSeriesItemRoleMapper()
        { }

        public override DocumentSeriesItemRole Map(DocumentSeriesItemRole entity, DocumentSeriesItemRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LinkType = entity.LinkType;
            #endregion

            return entityTransformed;
        }
    }
}
