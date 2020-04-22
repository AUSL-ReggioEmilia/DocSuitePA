using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierRoleValidatorMapper : BaseMapper<DossierRole, DossierRoleValidator>, IDossierRoleValidatorMapper
    {
        public DossierRoleValidatorMapper() { }

        public override DossierRoleValidator Map(DossierRole entity, DossierRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.AuthorizationRoleType = entity.AuthorizationRoleType;
            entityTransformed.Status = entity.Status;
            entityTransformed.IsMaster = entity.IsMaster;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Role = entity.Role;
            entityTransformed.Dossier = entity.Dossier;
            #endregion

            return entityTransformed;

        }
    }
}
