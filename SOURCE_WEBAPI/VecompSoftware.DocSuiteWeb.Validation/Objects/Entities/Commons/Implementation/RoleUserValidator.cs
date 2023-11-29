using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class RoleUserValidator : ObjectValidator<RoleUser, RoleUserValidator>, IRoleUserValidator
    {
        #region [ Constructor ]
        public RoleUserValidator(ILogger logger, IRoleUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region [ Properties ]

        public string Type { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public bool? Enabled { get; set; }
        public string Email { get; set; }
        public bool? IsMainRole { get; set; }
        public DSWEnvironmentType DSWEnvironment { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Role Role { get; set; }

        #endregion
    }
}