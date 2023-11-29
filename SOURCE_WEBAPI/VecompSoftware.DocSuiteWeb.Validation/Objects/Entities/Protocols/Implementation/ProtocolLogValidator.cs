using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolLogValidator : ObjectValidator<ProtocolLog, ProtocolLogValidator>, IProtocolLogValidator
    {
        #region [ Constructor ]
        public ProtocolLogValidator(ILogger logger, IProtocolLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region[ Properties ]
        public Guid UniqueId { get; set; }
        public int EntityId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public DateTime LogDate { get; set; }
        public string SystemComputer { get; set; }
        public string RegistrationUser { get; set; }
        public string Program { get; set; }
        public string LogType { get; set; }
        public string LogDescription { get; set; }
        public SeverityLog? Severity { get; set; }
        public string Hash { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public Protocol Protocol { get; set; }


        #endregion


    }
}
