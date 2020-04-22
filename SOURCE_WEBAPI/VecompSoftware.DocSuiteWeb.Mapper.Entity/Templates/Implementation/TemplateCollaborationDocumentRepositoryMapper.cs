
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates
{
    public class TemplateCollaborationDocumentRepositoryMapper : BaseEntityMapper<TemplateCollaborationDocumentRepository, TemplateCollaborationDocumentRepository>, ITemplateCollaborationDocumentRepositoryMapper
    {
        public override TemplateCollaborationDocumentRepository Map(TemplateCollaborationDocumentRepository entity, TemplateCollaborationDocumentRepository entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ChainType = entity.ChainType;
            #endregion

            return entityTransformed;
        }
    }
}
