using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskRoleUserValidator : ObjectValidator<DeskRoleUser, DeskRoleUserValidator>, IDeskRoleUserValidator
    {
        #region [ Constructor ]
        public DeskRoleUserValidator(ILogger logger, IDeskRoleUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties  ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Mapping dell'utente sugli utenti di Active Directory
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Ruolo dell'utente all'interno del tavolo.
        /// </summary>
        public DeskPermissionType PermissionType { get; set; }

        #endregion

        #region [ Navigation Properties  ]
        /// <summary>
        /// Riferimento alle approvazioni date dall'utente
        /// </summary>
        public ICollection<DeskDocumentEndorsement> DeskDocumentEndorsements { get; set; }
        /// <summary>
        /// Riferimento dell'utente con il tavolo
        /// </summary>
        public Desk Desk { get; set; }
        /// <summary>
        /// Mapping dell'utente sugli utenti presenti nel database
        /// </summary>
        public SecurityUser SecurityUsers { get; set; }
        /// <summary>
        /// Riferimento alla lavagna
        /// </summary>
        public ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }
        #endregion

        #region [ Methods ]

        #endregion
    }
}
