using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers
{
    public class DossierFolderRoleValidatorMapper : BaseMapper<DossierFolderRole, DossierFolderRoleValidator>, IDossierFolderRoleValidatorMapper
    {
        public DossierFolderRoleValidatorMapper() { }

        public override DossierFolderRoleValidator Map(DossierFolderRole entity, DossierFolderRoleValidator entityTransformed)
        {
            #region [Base]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RoleAuthorizationType = entity.AuthorizationRoleType;
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
            entityTransformed.DossierFolder = entity.DossierFolder;
            #endregion

            return entityTransformed;
        }
    }
}
