
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSMessageValidatorMapper : BaseMapper<UDSMessage, UDSMessageValidator>, IUDSMessageValidatorMapper
    {
        #region [ Constructor ]
        public UDSMessageValidatorMapper() { }

        #endregion

        #region [ Methods ]
        public override UDSMessageValidator Map(UDSMessage entity, UDSMessageValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RelationType = entity.RelationType;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion      

            #region [ Navigation Properties ]
            entityTransformed.Message = entity.Relation;
            entityTransformed.Repository = entity.Repository;
            #endregion

            return entityTransformed;
        }

        #endregion

    }
}
