using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations
{
    public class CollaborationLogValidatorMapper : BaseMapper<CollaborationLog, CollaborationLogValidator>, ICollaborationLogValidatorMapper
    {
        public CollaborationLogValidatorMapper() { }

        public override CollaborationLogValidator Map(CollaborationLog entity, CollaborationLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.IdChain = entity.IdChain;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Program = entity.Program;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.UniqueId = entity.UniqueId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Collaboration = entity.Entity;
            #endregion

            return entityTransformed;
        }

    }
}
