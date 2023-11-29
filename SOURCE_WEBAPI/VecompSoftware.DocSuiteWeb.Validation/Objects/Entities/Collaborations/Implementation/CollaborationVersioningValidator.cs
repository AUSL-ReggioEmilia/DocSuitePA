using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationVersioningValidator : ObjectValidator<CollaborationVersioning, CollaborationVersioningValidator>, ICollaborationVersioningValidator
    {
        #region [ Constructor ]

        public CollaborationVersioningValidator(ILogger logger, ICollaborationVersioningValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Get or set CollaborationIncremental
        /// </summary>
        public int CollaborationIncremental { get; set; }

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public int Incremental { get; set; }

        /// <summary>
        /// Get or set IdDocument
        /// </summary>
        public int IdDocument { get; set; }

        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Get or set DocumentName
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// Get or set CheckedOut
        /// </summary>
        public bool? CheckedOut { get; set; }

        /// <summary>
        /// Get or set CheckOutUser
        /// </summary>
        public string CheckOutUser { get; set; }

        /// <summary>
        /// Get or set CheckOutSessionId
        /// </summary>
        public string CheckOutSessionId { get; set; }

        /// <summary>
        /// Get or set CheckOutDate
        /// </summary>
        public DateTimeOffset? CheckOutDate { get; set; }

        /// <summary>
        /// Get or set DocumentChecksum
        /// </summary>
        public string DocumentChecksum { get; set; }

        /// <summary>
        /// Get or set DocumentGroup
        /// </summary>
        public string DocumentGroup { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Collaboration Collaboration { get; set; }
        #endregion
    }
}
