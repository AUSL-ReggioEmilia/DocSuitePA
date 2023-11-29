using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Dossiers;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Dossiers
{
    public class DossierCommentValidator : ObjectValidator<DossierComment, DossierCommentValidator>, IDossierCommentValidator
    {
        #region [ Constructor ]

        public DossierCommentValidator(ILogger logger, IDossierCommentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
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
