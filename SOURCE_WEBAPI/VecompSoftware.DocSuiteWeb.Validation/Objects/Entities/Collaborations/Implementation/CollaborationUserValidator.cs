using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationUserValidator : ObjectValidator<CollaborationUser, CollaborationUserValidator>, ICollaborationUserValidator
    {
        #region [ Constructor ]
        public CollaborationUserValidator(ILogger logger, ICollaborationUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
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
        /// Get or set DestinationFirst
        /// </summary>
        public bool? DestinationFirst { get; set; }

        /// <summary>
        /// Get or set DestinationType
        /// </summary>
        public string DestinationType { get; set; }

        /// <summary>
        /// Get or set DestinationName
        /// </summary>
        public string DestinationName { get; set; }

        /// <summary>
        /// Get or set DestinationEmail
        /// </summary>
        public string DestinationEmail { get; set; }

        /// <summary>
        /// Get or set Account
        /// </summary>
        public string Account { get; set; }


        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Get or set Collaboration reference
        /// </summary>
        public Collaboration Collaboration { get; set; }

        /// <summary>
        /// Get or set Role reference
        /// </summary>
        public Role Role { get; set; }
        #endregion
    }
}
