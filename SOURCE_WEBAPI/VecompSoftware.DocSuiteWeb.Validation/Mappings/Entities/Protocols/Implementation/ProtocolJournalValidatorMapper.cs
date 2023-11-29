using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols
{
    public class ProtocolJournalValidatorMapper : BaseMapper<ProtocolJournal, ProtocolJournalValidator>, IProtocolJournalValidatorMapper
    {
        public ProtocolJournalValidatorMapper() { }

        public override ProtocolJournalValidator Map(ProtocolJournal entity, ProtocolJournalValidator entityTransformed)
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
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            #endregion

            #region [ Navigation Properties ]

            entityTransformed.TenantAOO = entity.TenantAOO;
            entityTransformed.Location = entity.Location;

            #endregion

            return entityTransformed;
        }

    }
}
