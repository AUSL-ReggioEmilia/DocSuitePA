using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskCollaborationValidator : ObjectValidator<DeskCollaboration, DeskCollaborationValidator>, IDeskCollaborationValidator
    {
        #region [ Constructor ]
        public DeskCollaborationValidator(ILogger logger, DeskCollaborationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Oggetto esterno riferito ad una collaborazione
        /// </summary>
        public Collaboration Collaboration { get; set; }

        /// <summary>
        /// Oggetto esterno riferito ad un tavolo
        /// </summary>
        public Desk Desk { get; set; }
        #endregion
    }
}
