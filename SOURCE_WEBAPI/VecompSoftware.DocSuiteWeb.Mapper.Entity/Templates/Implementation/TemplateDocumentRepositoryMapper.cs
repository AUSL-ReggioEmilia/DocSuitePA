
using VecompSoftware.DocSuiteWeb.Entity.Templates;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Templates
{
    public class TemplateDocumentRepositoryMapper : BaseEntityMapper<TemplateDocumentRepository, TemplateDocumentRepository>, ITemplateDocumentRepositoryMapper
    {
        public override TemplateDocumentRepository Map(TemplateDocumentRepository entity, TemplateDocumentRepository entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Status = entity.Status;
            entityTransformed.Object = entity.Object;
            entityTransformed.Name = entity.Name;
            entityTransformed.QualityTag = entity.QualityTag;
            entityTransformed.Version = entity.Version;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;

            #endregion

            return entityTransformed;
        }

    }
}
