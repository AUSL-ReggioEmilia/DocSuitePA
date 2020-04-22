using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSRoleMapper : BaseEntityMapper<UDSRole, UDSRole>, IUDSRoleMapper
    {
        public override UDSRole Map(UDSRole entity, UDSRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.AuthorizationLabel = entity.AuthorizationLabel;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            #endregion

            return entityTransformed;
        }

    }
}
