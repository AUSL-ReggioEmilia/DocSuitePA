using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails
{
    public class PECMailLogValidatorMapper : BaseMapper<PECMailLog, PECMailLogValidator>, IPECMailLogValidatorMapper
    {
        public PECMailLogValidatorMapper() { }

        public override PECMailLogValidator Map(PECMailLog entity, PECMailLogValidator entityTransformed)
        {
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.Description = entity.Description;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.PECMail = entity.PECMail;
            entityTransformed.Hash = entity.Hash;

            return entityTransformed;
        }

    }
}
