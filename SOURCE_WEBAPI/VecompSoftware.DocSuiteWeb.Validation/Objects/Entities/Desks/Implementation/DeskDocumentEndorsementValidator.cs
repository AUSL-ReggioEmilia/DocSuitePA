using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskDocumentEndorsementValidator : ObjectValidator<DeskDocumentEndorsement, DeskDocumentEndorsementValidator>, IDeskDocumentEndorsementValidator
    {
        #region [ Constructor ]
        public DeskDocumentEndorsementValidator(ILogger logger, IDeskDocumentEndorsementValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Endorsement: identifica l'accettazione di una versione di un documento da parte dell'utente.
        /// </summary>
        public bool Endorsement { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra la tabella Endorsement e la tabella DeskRoleUser
        /// </summary>
        public DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra la tabella Endorsement e la versione del DeskDocument
        /// </summary>
        public DeskDocumentVersion DeskDocumentVersion { get; set; }
        #endregion
    }
}
