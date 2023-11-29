using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierLinkValidator : ObjectValidator<DossierLink, DossierLinkValidator>, IDossierLinkValidator
    {
        #region [ Constructor ]
        public DossierLinkValidator(ILogger logger, IDossierLinkValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public DossierLinkType DossierLinkType { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public Dossier Dossier { get; set; }

        #endregion
    }
}
