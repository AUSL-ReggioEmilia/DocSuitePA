using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Fascicles
{
    public class FascicleLogMapper : BaseEntityMapper<FascicleLog, FascicleLog>, IFascicleLogMapper
    {
        public override FascicleLog Map(FascicleLog entity, FascicleLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;
            #endregion

            return entityTransformed;
        }
    }
}
