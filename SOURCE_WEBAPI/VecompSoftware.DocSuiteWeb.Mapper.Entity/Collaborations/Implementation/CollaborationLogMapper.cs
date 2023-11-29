using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Collaborations
{
    public class CollaborationLogMapper : BaseEntityMapper<CollaborationLog, CollaborationLog>, ICollaborationLogMapper
    {
        public override CollaborationLog Map(CollaborationLog entity, CollaborationLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.IdChain = entity.IdChain;
            entityTransformed.CollaborationIncremental = entity.CollaborationIncremental;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.Program = entity.Program;
            entityTransformed.Severity = entity.Severity;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            return entityTransformed;
        }

    }
}
