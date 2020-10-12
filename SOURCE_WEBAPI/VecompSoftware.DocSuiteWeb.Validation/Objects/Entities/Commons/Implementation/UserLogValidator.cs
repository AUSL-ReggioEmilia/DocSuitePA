using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class UserLogValidator : ObjectValidator<UserLog, UserLogValidator>, IUserLogValidator
    {
        #region [ Constructor ]
        public UserLogValidator(ILogger logger, IUserLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }
        #endregion

        #region[ Properties ]
        public string SystemUser { get; set; }
        public string SystemServer { get; set; }
        public string SystemComputer { get; set; }
        public int? AccessNumber { get; set; }
        public DateTimeOffset? PrevOperationDate { get; set; }
        public string SessionId { get; set; }
        public bool? AdvancedScanner { get; set; }
        public bool? AdvancedViewer { get; set; }
        public string UserMail { get; set; }
        public string MobilePhone { get; set; }
        public string DefaultAdaptiveSearchControls { get; set; }
        public string AdaptiveSearchStatistics { get; set; }
        public string AdaptiveSearchEvaluated { get; set; }
        public int PrivacyLevel { get; set; }
        public Guid CurrentTenantId { get; set; }
        public string UserProfile { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public Guid UniqueId { get; set; }
        #endregion

        #region [ Navigation Properties ]
        #endregion
    }
}
