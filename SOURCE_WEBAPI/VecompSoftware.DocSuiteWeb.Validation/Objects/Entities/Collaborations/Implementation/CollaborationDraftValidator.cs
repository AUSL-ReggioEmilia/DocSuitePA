using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Collaborations
{
    public class CollaborationDraftValidator : ObjectValidator<CollaborationDraft, CollaborationDraftValidator>, ICollaborationDraftValidator
    {
        #region [ Constructor ]
        public CollaborationDraftValidator(ILogger logger, ICollaborationDraftValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }
        #endregion

        #region[ Properties ]
        public Guid UniqueId { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public int DraftType { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public Collaboration Collaboration { get; set; }
        #endregion
    }
}
