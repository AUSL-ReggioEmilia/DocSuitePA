using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierDocumentMapper : BaseEntityMapper<DossierDocument, DossierDocument>, IDossierDocumentMapper
    {
        public override DossierDocument Map(DossierDocument entity, DossierDocument entityTransformed)
        {
            #region [ Base ]
            entityTransformed.ChainType = entity.ChainType;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            #endregion

            return entityTransformed;
        }
    }
}
