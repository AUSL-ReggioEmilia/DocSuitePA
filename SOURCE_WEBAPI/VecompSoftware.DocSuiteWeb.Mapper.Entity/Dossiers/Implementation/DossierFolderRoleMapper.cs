using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierFolderRoleMapper : BaseEntityMapper<DossierFolderRole, DossierFolderRole>, IDossierFolderRoleMapper
    {
        public override DossierFolderRole Map(DossierFolderRole entity, DossierFolderRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.AuthorizationRoleType = entity.AuthorizationRoleType;
            entityTransformed.IsMaster = entity.IsMaster;
            entityTransformed.Status = entity.Status;
            #endregion

            return entityTransformed;
        }
    }
}
