using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskDocumentVersionValidator : ObjectValidator<DeskDocumentVersion, DeskDocumentVersionValidator>, IDeskDocumentVersionValidator
    {
        #region [ Constructor ]
        public DeskDocumentVersionValidator(ILogger logger, IDeskDocumentVersionValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Versione incrementale del documento
        /// </summary>
        public decimal? Version { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra la "versione" e il "documento".
        /// </summary>
        public DeskDocument DeskDocument { get; set; }
        /// <summary>
        /// Relazione tra la versione corrente del documento e le accetazioni.
        /// </summary>
        public ICollection<DeskDocumentEndorsement> DeskDocumentEndorsements { get; set; }
        /// <summary>
        /// Relazione tra la "lavagna" e la versione attuale del documento.
        /// </summary>
        public ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }
        #endregion
    }
}
