using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSRoleValidatorMapper : BaseMapper<UDSRole, UDSRoleValidator>, IUDSRoleValidatorMapper
    {

        #region [ Constructor ]
        public UDSRoleValidatorMapper() { }
        #endregion

        #region [ Methods ]
        public override UDSRoleValidator Map(UDSRole entity, UDSRoleValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.AuthorizationLabel = entity.AuthorizationLabel;
            entityTransformed.AuthorizationType = entity.AuthorizationType;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Authorization = entity.Relation;
            entityTransformed.Repository = entity.Repository;
            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
