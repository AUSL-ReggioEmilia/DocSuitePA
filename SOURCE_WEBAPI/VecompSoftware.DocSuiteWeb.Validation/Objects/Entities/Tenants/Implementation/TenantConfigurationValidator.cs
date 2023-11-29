using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants
{
    public class TenantConfigurationValidator : ObjectValidator<TenantConfiguration, TenantConfigurationValidator>, ITenantConfigurationValidator
    {
        #region [ Constructor ]
        public TenantConfigurationValidator(ILogger logger, ITenantConfigurationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public TenantConfigurationType ConfigurationType { get; set; }
        public string JsonValue { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public Tenant Tenant { get; set; }

        #endregion
    }
}
