using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Tenants;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Tenants
{
    public class TenantWorkflowRepositoryValidator : ObjectValidator<TenantWorkflowRepository, TenantWorkflowRepositoryValidator>, ITenantWorkflowRepositoryValidator
    {
        #region [ Constructor ]
        public TenantWorkflowRepositoryValidator(ILogger logger, ITenantWorkflowRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string JsonValue { get; set; }
        public string IntegrationModuleName { get; set; }
        public string Conditions { get; set; }
        public TenantWorkflowRepositoryType ConfigurationType { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public WorkflowRepository WorkflowRepository { get; set; }

        public Tenant Tenant { get; set; }

        #endregion
    }
}
