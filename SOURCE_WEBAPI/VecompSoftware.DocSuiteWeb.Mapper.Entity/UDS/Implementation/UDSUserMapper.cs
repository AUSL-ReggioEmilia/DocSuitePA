using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSUserMapper : BaseEntityMapper<UDSUser, UDSUser>, IUDSUserMapper
    {
        public override UDSUser Map(UDSUser entity, UDSUser entityTransformed)
        {

            #region [ Base ]
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Status = entity.Status;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.Account = entity.Account;
            #endregion

            return entityTransformed;
        }
    }
}
