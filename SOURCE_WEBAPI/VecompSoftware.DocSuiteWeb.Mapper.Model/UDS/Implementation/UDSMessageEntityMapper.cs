using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.UDS
{
    public class UDSMessageEntityMapper : BaseModelMapper<UDSMessageModel, UDSMessage>, IUDSMessageEntityMapper
    {
        #region [ Methods ]
        public override UDSMessage Map(UDSMessageModel entity, UDSMessage entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.RelationType = (Entity.UDS.UDSRelationType)entity.RelationType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
