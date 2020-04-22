using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierDocumentValidatorMapper : BaseMapper<DossierDocument, DossierDocumentValidator>, IDossierDocumentValidatorMapper
    {
        public DossierDocumentValidatorMapper() { }

        public override DossierDocumentValidator Map(DossierDocument entity, DossierDocumentValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.ChainType = entity.ChainType;
            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Dossier = entity.Dossier;
            #endregion

            return entityTransformed;
        }
    }
}
