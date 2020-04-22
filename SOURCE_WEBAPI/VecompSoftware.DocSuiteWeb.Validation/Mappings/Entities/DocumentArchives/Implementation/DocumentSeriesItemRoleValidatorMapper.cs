using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives
{
    public class DocumentSeriesItemRoleValidatorMapper : BaseMapper<DocumentSeriesItemRole, DocumentSeriesItemRoleValidator>, IDocumentSeriesItemRoleValidatorMapper
    {
        public DocumentSeriesItemRoleValidatorMapper() { }

        public override DocumentSeriesItemRoleValidator Map(DocumentSeriesItemRole entity, DocumentSeriesItemRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LinkType = entity.LinkType;
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.DocumentSeriesItem = entity.DocumentSeriesItem;
            entityTransformed.Role = entity.Role;
            #endregion

            return entityTransformed;
        }
    }
}
