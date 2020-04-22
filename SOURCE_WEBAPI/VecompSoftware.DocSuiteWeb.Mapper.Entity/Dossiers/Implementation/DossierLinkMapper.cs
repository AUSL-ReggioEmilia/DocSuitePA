using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierLinkMapper : BaseEntityMapper<DossierLink, DossierLink>, IDossierLinkMapper
    {
        public override DossierLink Map(DossierLink entity, DossierLink entityTransformed)
        {
            #region [ Base ]
            entityTransformed.DossierLinkType = entity.DossierLinkType;
            #endregion

            return entityTransformed;
        }
    }
}
