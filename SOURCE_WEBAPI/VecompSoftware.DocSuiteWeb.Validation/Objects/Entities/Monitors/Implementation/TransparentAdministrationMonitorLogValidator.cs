using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Monitors;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Monitors
{
    public class TransparentAdministrationMonitorLogValidator : ObjectValidator<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogValidator>, ITransparentAdministrationMonitorLogValidator
    {
        #region [ Constructor ]
        public TransparentAdministrationMonitorLogValidator(ILogger logger, ITransparentAdministrationMonitorLogValidatorMapper mapper,
            IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string DocumentUnitName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Note { get; set; }
        public string Rating { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public DocumentUnit DocumentUnit { get; set; }
        public Role Role { get; set; }
        #endregion
    }
}
