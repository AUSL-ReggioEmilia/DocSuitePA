using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSUserValidatorMapper : BaseMapper<UDSUser, UDSUserValidator>, IUDSUserValidatorMapper
    {
        #region [ Constuctor ]

        public UDSUserValidatorMapper() { }

        #endregion

        #region [ Methods]
        public override UDSUserValidator Map(UDSUser entity, UDSUserValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.Status = entity.Status;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.Account = entity.Account;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ] 
            entityTransformed.Repository = entity.Repository;
            #endregion

            return entityTransformed;
        }
        #endregion

    }
}
