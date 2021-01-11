using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierLogValidator : ObjectValidator<DossierLog, DossierLogValidator>, IDossierLogValidator
    {
        #region [ Constructor ]

        public DossierLogValidator(ILogger logger, IDossierLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string SystemComputer { get; set; }

        public string RegistrationUser { get; set; }

        public DossierLogType LogType { get; set; }

        public string LogDescription { get; set; }

        public SeverityLog? Severity { get; set; }

        public string Hash { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Dossier Dossier { get; set; }

        public DossierFolder DossierFolder { get; set; }
        #endregion
    }
}
