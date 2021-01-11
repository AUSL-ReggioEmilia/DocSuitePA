using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSUserEntityMapper : BaseModelMapper<UDSUserModel, UDSUser>, IUDSUserEntityMapper
    {
        #region [ Methods ]
        public override UDSUser Map(UDSUserModel entity, UDSUser entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Environment = entity.Environment;
            entityTransformed.AuthorizationType = (AuthorizationRoleType)entity.AuthorizationType;
            entityTransformed.Account = entity.Account;
            entityTransformed.Status = UDSUserStatus.Active;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
