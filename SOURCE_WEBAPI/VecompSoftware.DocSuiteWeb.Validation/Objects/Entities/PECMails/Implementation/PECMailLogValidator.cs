using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails
{
    public class PECMailLogValidator : ObjectValidator<PECMailLog, PECMailLogValidator>, IPECMailLogValidator
    {
        #region [ Constructor ]
        public PECMailLogValidator(ILogger logger, IPECMailLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        public string Description { get; set; }

        public string LogType { get; set; }

        public DateTime LogDate { get; set; }

        public string SystemComputer { get; set; }

        public string RegistrationUser { get; set; }

        public short? Severity { get; set; }

        public string Hash { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public PECMail PECMail { get; set; }

        #endregion
    }
}
