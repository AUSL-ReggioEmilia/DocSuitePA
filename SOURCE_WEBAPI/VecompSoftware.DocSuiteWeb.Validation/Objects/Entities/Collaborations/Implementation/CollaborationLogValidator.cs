using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    [HasSelfValidation]
    public class CollaborationLogValidator : ObjectValidator<CollaborationLog, CollaborationLogValidator>, ICollaborationLogValidator
    {
        #region [ Constructor ]
        public CollaborationLogValidator(ILogger logger, ICollaborationLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {

        }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        /// <summary>
        /// Get or set CollaborationIncremental
        /// </summary>
        public int? CollaborationIncremental { get; set; }

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public short? Incremental { get; set; }

        /// <summary>
        /// Get or set IdChain
        /// </summary>
        public int? IdChain { get; set; }

        /// <summary>
        /// Get or set LogDate
        /// </summary>
        public DateTime? LogDate { get; set; }

        /// <summary>
        /// Get or set SystemComputer
        /// </summary>
        public string SystemComputer { get; set; }

        /// <summary>
        /// Get or set RegistrationUser
        /// </summary>
        public string RegistrationUser { get; set; }

        /// <summary>
        /// Get or set SessionId
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Get or set Program
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// Get or set LogType
        /// </summary>
        public string LogType { get; set; }

        /// <summary>
        /// Get or set LogDescription
        /// </summary>
        public string LogDescription { get; set; }

        /// <summary>
        /// Get or set Severity
        /// </summary>
        public SeverityLog? Severity { get; set; }

        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Collaboration Collaboration { get; set; }
        #endregion
    }
}
