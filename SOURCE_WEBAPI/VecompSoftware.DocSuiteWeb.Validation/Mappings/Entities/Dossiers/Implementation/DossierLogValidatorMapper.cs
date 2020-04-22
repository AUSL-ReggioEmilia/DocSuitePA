using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierLogValidatorMapper : BaseMapper<DossierLog, DossierLogValidator>, IDossierLogValidatorMapper
    {
        public DossierLogValidatorMapper() { }

        public override DossierLogValidator Map(DossierLog entity, DossierLogValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Dossier = entity.Entity;
            entityTransformed.DossierFolder = entity.DossierFolder;
            #endregion

            return entityTransformed;
        }
    }
}
