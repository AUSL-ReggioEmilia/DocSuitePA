using VecompSoftware.DocSuiteWeb.Entity.PECMails;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.PECMails
{
    public class PECMailLogMapper : BaseEntityMapper<PECMailLog, PECMailLog>, IPECMailLogMapper
    {
        public override PECMailLog Map(PECMailLog entity, PECMailLog entityTransformed)
        {
            #region [ Base ]

            entityTransformed.Description = entity.Description;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.Hash = entity.Hash;

            #endregion

            return entityTransformed;
        }

    }
}
