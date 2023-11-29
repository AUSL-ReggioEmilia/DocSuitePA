using System;
using NHibernate;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.EntityMapper.Commons;
using APIProtocol = VecompSoftware.DocSuiteWeb.Entity.Protocols;
using DSW = VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.EntityMapper.Protocols
{
    public class MapperProtocolJournalLogEntity : BaseEntityMapper<DSW.ProtocolJournalLog, APIProtocol.ProtocolJournal>
    {
        #region [ Fields ]
        private readonly MapperLocationEntity _mapperLocation;
        #endregion

        #region [ Constructor ]
        public MapperProtocolJournalLogEntity()
        {
            _mapperLocation = new MapperLocationEntity();
        }
        #endregion

        #region [ Methods ]
        protected override IQueryOver<DSW.ProtocolJournalLog, DSW.ProtocolJournalLog> MappingProjection(IQueryOver<DSW.ProtocolJournalLog, DSW.ProtocolJournalLog> queryOver)
        {
            throw new NotImplementedException();
        }

        protected override APIProtocol.ProtocolJournal TransformDTO(DSW.ProtocolJournalLog entity)
        {
            if (entity == null)
                throw new ArgumentException("Impossibile trasformare Protocol se l'entità non è inizializzata");

            APIProtocol.ProtocolJournal apiProtocolJournal = new APIProtocol.ProtocolJournal();
            apiProtocolJournal.UniqueId = entity.UniqueId;
            apiProtocolJournal.EntityId = entity.Id;
            apiProtocolJournal.EndDate = entity.EndDate;
            apiProtocolJournal.IdDocument = entity.IdDocument;
            apiProtocolJournal.TenantAOO = entity.IdTenantAOO.HasValue ? new TenantAOO() { UniqueId = entity.IdTenantAOO.Value} : null;
            apiProtocolJournal.RegistrationDate = entity.RegistrationDate;
            apiProtocolJournal.RegistrationUser = entity.RegistrationUser;
            apiProtocolJournal.LastChangedDate = entity.LastChangedDate;
            apiProtocolJournal.LastChangedUser = entity.LastChangedUser;
            apiProtocolJournal.LogDate = entity.LogDate.HasValue ? entity.LogDate.Value : DateTime.MinValue;
            apiProtocolJournal.LogDescription = entity.LogDescription;
            apiProtocolJournal.ProtocolActive = entity.ProtocolActive.HasValue ? entity.ProtocolActive.Value : 0;
            apiProtocolJournal.ProtocolCancelled = entity.ProtocolCancelled;
            apiProtocolJournal.ProtocolError = entity.ProtocolError;
            apiProtocolJournal.ProtocolJournalDate = entity.ProtocolJournalDate.HasValue ? entity.ProtocolJournalDate.Value : DateTime.MinValue;
            apiProtocolJournal.ProtocolOthers = entity.ProtocolOthers;
            apiProtocolJournal.ProtocolRegister = entity.ProtocolRegister;
            apiProtocolJournal.ProtocolTotal = entity.ProtocolTotal;
            apiProtocolJournal.StartDate = entity.StartDate;
            apiProtocolJournal.SystemComputer = entity.SystemComputer;
            apiProtocolJournal.SystemUser = entity.SystemUser;
            apiProtocolJournal.Location = entity.Location != null ? _mapperLocation.MappingDTO(entity.Location) : null;
            return apiProtocolJournal;
        }
        #endregion
    }
}
