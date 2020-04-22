using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierLinkValidatorMapper : BaseMapper<DossierLink, DossierLinkValidator>, IDossierLinkValidatorMapper
    {
        public DossierLinkValidatorMapper() { }

        public override DossierLinkValidator Map(DossierLink entity, DossierLinkValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.DossierLinkType = entity.DossierLinkType;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Dossier = entity.Dossier;
            #endregion

            return entityTransformed;
        }
    }
}
