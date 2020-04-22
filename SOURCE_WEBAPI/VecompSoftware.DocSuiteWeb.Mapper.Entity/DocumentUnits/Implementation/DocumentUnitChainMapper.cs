using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits
{
    public class DocumentUnitChainMapper : BaseEntityMapper<DocumentUnitChain, DocumentUnitChain>, IDocumentUnitChainMapper
    {
        public DocumentUnitChainMapper()
        { }

        public override DocumentUnitChain Map(DocumentUnitChain entity, DocumentUnitChain entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DocumentLabel = entity.DocumentLabel;
            entityTransformed.DocumentName = entity.DocumentName;
            entityTransformed.ArchiveName = entity.ArchiveName;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.ChainType = entity.ChainType;
            #endregion

            return entityTransformed;
        }

    }
}
