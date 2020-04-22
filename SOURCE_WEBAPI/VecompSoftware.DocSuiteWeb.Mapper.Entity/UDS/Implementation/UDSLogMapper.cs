using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.UDS
{
    public class UDSLogMapper : BaseEntityMapper<UDSLog, UDSLog>, IUDSLogMapper
    {
        public override UDSLog Map(UDSLog entity, UDSLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdUDS = entity.IdUDS;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            #endregion

            return entityTransformed;
        }
    }
}
