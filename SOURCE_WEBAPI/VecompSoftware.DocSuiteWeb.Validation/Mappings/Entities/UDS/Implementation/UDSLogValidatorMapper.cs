using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSLogValidatorMapper : BaseMapper<UDSLog, UDSLogValidator>, IUDSLogValidatorMapper
    {
        public UDSLogValidatorMapper() { }

        public override UDSLogValidator Map(UDSLog entity, UDSLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.UDSRepository = entity.Entity;
            #endregion

            return entityTransformed;
        }
    }
}
