using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSLogValidator : ObjectValidator<UDSLog, UDSLogValidator>, IUDSLogValidator
    {
        #region [ Constructor ]
        public UDSLogValidator(ILogger logger, IUDSLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string SystemComputer { get; set; }

        public string RegistrationUser { get; set; }

        public int Environment { get; set; }

        public Guid IdUDS { get; set; }

        public UDSLogType LogType { get; set; }

        public string LogDescription { get; set; }

        public SeverityLog? Severity { get; set; }

        public string Hash { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public UDSRepository UDSRepository { get; set; }
        #endregion
    }
}
