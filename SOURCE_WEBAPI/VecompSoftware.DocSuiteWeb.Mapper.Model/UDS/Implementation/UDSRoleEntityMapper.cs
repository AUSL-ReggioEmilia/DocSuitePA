using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSRoleEntityMapper : BaseModelMapper<UDSRoleModel, UDSRole>, IUDSRoleEntityMapper
    {
        #region [ Methods ]
        public override UDSRole Map(UDSRoleModel entity, UDSRole entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Environment = entity.Environment;
            entityTransformed.AuthorizationType = (AuthorizationRoleType)entity.AuthorizationType;
            entityTransformed.AuthorizationLabel = entity.AuthorizationLabel;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RelationType = (Entity.UDS.UDSRelationType)entity.RelationType;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
