using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolUserValidator : ObjectValidator<ProtocolUser, ProtocolUserValidator>, IProtocolUserValidator
    {
        #region [ Constructor ]
        public ProtocolUserValidator(ILogger logger, IProtocolUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity,parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }

        public string Account { get; set; }

        public ProtocolUserType Type { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public string LastChangedUser { get; set; }

        public byte[] Timestamp { get; set; }
        public string Note { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public Protocol Protocol { get; set; }
        #endregion
    }
}
