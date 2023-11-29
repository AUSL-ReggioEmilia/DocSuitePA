using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationSignValidator : ObjectValidator<CollaborationSign, CollaborationSignValidator>, ICollaborationSignValidator
    {
        #region [ Constructor ]
        public CollaborationSignValidator(ILogger logger, ICollaborationSignValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        /// <summary>
        /// Get or set Incremental
        /// </summary>
        public int Incremental { get; set; }

        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Get or set IdStatus
        /// </summary>
        public string IdStatus { get; set; }

        /// <summary>
        /// Get or set SignUser
        /// </summary>
        public string SignUser { get; set; }

        /// <summary>
        /// Get or set SignName
        /// </summary>
        public string SignName { get; set; }

        /// <summary>
        /// Get or set SignEmail
        /// </summary>
        public string SignEmail { get; set; }

        /// <summary>
        /// Get or set SignDate
        /// </summary>
        public DateTime? SignDate { get; set; }

        /// <summary>
        /// Get or set IsRequired
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Get or set IsAbsent
        /// </summary>
        public bool? IsAbsent { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public Collaboration Collaboration { get; set; }
        #endregion
    }
}
