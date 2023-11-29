using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationValidator : ObjectValidator<Collaboration, CollaborationValidator>, ICollaborationValidator
    {
        #region [ Constructor ]
        public CollaborationValidator(ILogger logger, ICollaborationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }
        /// <summary>
        /// Get or set DocumentType
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Get or set IdPriority
        /// </summary>
        public string IdPriority { get; set; }

        /// <summary>
        /// Get or set IdStatus
        /// </summary>
        public string IdStatus { get; set; }

        /// <summary>
        /// Get or set SignCount
        /// </summary>
        public short? SignCount { get; set; }

        /// <summary>
        /// Get or set MemorandumDate
        /// </summary>
        public DateTime? MemorandumDate { get; set; }

        /// <summary>
        /// Get or set Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Get or set Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Get or set TemplateName
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Get or set PublicationUser
        /// </summary>
        public string PublicationUser { get; set; }

        /// <summary>
        /// Get or set PublicationDate
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// Get or set RegistrationName
        /// </summary>
        public string RegistrationName { get; set; }

        /// <summary>
        /// Get or set RegistrationEmail
        /// </summary>
        public string RegistrationEmail { get; set; }

        /// <summary>
        /// Get or set SourceProtocolYear
        /// </summary>
        public short? SourceProtocolYear { get; set; }

        /// <summary>
        /// Get or set SourceProtocolNumber
        /// </summary>
        public int? SourceProtocolNumber { get; set; }

        /// <summary>
        /// Get or set AlertDate
        /// </summary>
        public DateTime? AlertDate { get; set; }

        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Get or set Desk reference
        /// </summary>
        public DeskCollaboration DeskCollaboration { get; set; }

        /// <summary>
        /// Get or set Location reference
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Get or set CollaborationLog reference
        /// </summary>
        public ICollection<CollaborationLog> CollaborationLogs { get; set; }

        /// <summary>
        /// Get or set CollaborationSign reference
        /// </summary>
        public ICollection<CollaborationSign> CollaborationSigns { get; set; }

        /// <summary>
        /// Get or set CollaborationUser reference
        /// </summary>
        public ICollection<CollaborationUser> CollaborationUsers { get; set; }

        /// <summary>
        /// Get or set CollaborationVersioning reference
        /// </summary>
        public ICollection<CollaborationVersioning> CollaborationVersionings { get; set; }


        #endregion
    }
}
