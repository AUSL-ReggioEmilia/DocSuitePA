using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionLogValidatorMapper : BaseMapper<ResolutionLog, ResolutionLogValidator>, IResolutionLogValidatorMapper
    {
        public ResolutionLogValidatorMapper() { }

        public override ResolutionLogValidator Map(ResolutionLog entity, ResolutionLogValidator entityTransformed)
        {
            #region [ Base ]

            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.IdResolution = entity.IdResolution;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Program = entity.Program;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.UniqueId = entity.UniqueId;

            #endregion

            #region [ Navigation Properties ]

            entityTransformed.Resolution = entity.Entity;

            #endregion

            return entityTransformed;
        }
    }
}
