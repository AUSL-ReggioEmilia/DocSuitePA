using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class PrivacyLevelValidator : ObjectValidator<PrivacyLevel, PrivacyLevelValidator>, IPrivacyLevelValidator
    {
        #region [ Constructor ]
        public PrivacyLevelValidator(ILogger logger, IPrivacyLevelValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region[ Properties ]

        public Guid UniqueId { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public string Colour { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]  
        #endregion

    }
}