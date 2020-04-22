using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolDraftValidator : ObjectValidator<ProtocolDraft, ProtocolDraftValidator>, IProtocolDraftValidator
    {
        #region [ Constructor ]
        public ProtocolDraftValidator(ILogger logger, IProtocolDraftValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
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
