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
    public class DeskValidator : ObjectValidator<Desk, DeskValidator>, IDeskValidator
    {
        #region [ Constructor ]
        public DeskValidator(ILogger logger, IDeskValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Nome del tavolo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrizione del tavolo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data di scadenza del tavolo
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Stato del tavolo.
        /// 1) aperto
        /// 2) chiuso
        /// 3) approvazione
        /// </summary>
        public DeskState? Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Collezione di Documenti presenti nel Tavolo
        /// </summary>
        public ICollection<DeskDocument> DeskDocuments { get; set; }
        /// <summary>
        /// Collezione di Log che riferiscono al tavolo
        /// </summary>
        public ICollection<DeskLog> DeskLogs { get; set; }
        /// <summary>
        /// Collezione di Messaggi che riferiscono al tavolo
        /// </summary>
        public ICollection<DeskMessage> DeskMessages { get; set; }
        /// <summary>
        /// Collezione di RoleUser che riferiscono al tavolo
        /// </summary>
        public ICollection<DeskRoleUser> DeskRoleUsers { get; set; }
        /// <summary>
        /// Collezione di Story Board che riferiscono al tavolo
        /// </summary>
        public ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }

        /// <summary>
        /// Collezione contenente i riferimenti tra Desk e Collaboration
        /// </summary>
        public ICollection<DeskCollaboration> DeskCollaborations { get; set; }
        #endregion
    }
}
