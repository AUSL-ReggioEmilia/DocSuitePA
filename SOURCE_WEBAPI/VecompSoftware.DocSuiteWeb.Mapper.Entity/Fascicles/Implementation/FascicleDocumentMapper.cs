using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleDocumentMapper : BaseEntityMapper<FascicleDocument, FascicleDocument>, IFascicleDocumentMapper
    {
        public override FascicleDocument Map(FascicleDocument entity, FascicleDocument entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ChainType = entity.ChainType;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            #endregion

            return entityTransformed;
        }
    }
}
