using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolJournalValidator : ObjectValidator<ProtocolJournal, ProtocolJournalValidator>, IProtocolJournalValidator
    {
        #region [ Constructor ]
        public ProtocolJournalValidator(ILogger logger, IProtocolJournalValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public DateTime ProtocolJournalDate { get; set; }
        public DateTime LogDate { get; set; }
        public string SystemComputer { get; set; }
        public string SystemUser { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProtocolTotal { get; set; }
        public int? ProtocolRegister { get; set; }
        public int? ProtocolError { get; set; }
        public int? ProtocolCancelled { get; set; }
        public int ProtocolActive { get; set; }
        public int? ProtocolOthers { get; set; }
        public int? IdDocument { get; set; }
        public string LogDescription { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual TenantAOO TenantAOO { get; set; }
        public virtual Location Location { get; set; }

        #endregion


    }
}
