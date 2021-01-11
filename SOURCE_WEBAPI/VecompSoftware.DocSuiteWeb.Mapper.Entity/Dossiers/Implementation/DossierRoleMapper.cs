using VecompSoftware.DocSuiteWeb.Entity.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Dossiers
{
    public class DossierRoleMapper : BaseEntityMapper<DossierRole, DossierRole>, IDossierRoleMapper
    {
        public override DossierRole Map(DossierRole entity, DossierRole entityTransformed)
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
