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
    public class DeskStoryBoardValidator : ObjectValidator<DeskStoryBoard, DeskStoryBoardValidator>, IDeskStoryBoardValidator
    {
        public DeskStoryBoardValidator(ILogger logger, IDeskStoryBoardValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Commento lasciato sulla lavagna
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Data e ora d'inserimento del commento
        /// </summary>
        public DateTime? DateBoard { get; set; }
        /// <summary>
        /// Nome completo dell'autore del commento
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Tipologia di commento lasciato in lavagna
        /// </summary>
        public DeskStoryBoardType BoardType { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra la "Lavagna" e "Tavoli"
        /// </summary>
        public Desk Desk { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e l'utente che l'ha scritto.
        /// </summary>
        public DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e una specifica versione di un documento.
        /// </summary>
        public DeskDocumentVersion DeskDocumentVersion { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
