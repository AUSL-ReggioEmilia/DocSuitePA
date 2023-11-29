using VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Protocols
{
    public class ProtocolJournalMapper : BaseEntityMapper<ProtocolJournal, ProtocolJournal>, IProtocolJournalMapper
    {
        public override ProtocolJournal Map(ProtocolJournal entity, ProtocolJournal entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ProtocolJournalDate = entity.ProtocolJournalDate;
            entityTransformed.LogDate = entity.LogDate;
            entityTransformed.SystemUser = entity.SystemUser;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.ProtocolTotal = entity.ProtocolTotal;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.ProtocolRegister = entity.ProtocolRegister;
            entityTransformed.ProtocolError = entity.ProtocolError;
            entityTransformed.ProtocolCancelled = entity.ProtocolCancelled;
            entityTransformed.ProtocolActive = entity.ProtocolActive;
            entityTransformed.ProtocolOthers = entity.ProtocolOthers;
            entityTransformed.IdDocument = entity.IdDocument;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.TenantAOO = entity.TenantAOO;
            entityTransformed.Location = entity.Location;

            #endregion

            return entityTransformed;
        }

    }
}
