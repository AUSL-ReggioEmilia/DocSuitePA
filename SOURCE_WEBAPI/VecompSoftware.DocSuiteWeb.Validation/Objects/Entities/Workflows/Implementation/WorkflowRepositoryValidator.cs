using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowRepositoryValidator : ObjectValidator<WorkflowRepository, WorkflowRepositoryValidator>, IWorkflowRepositoryValidator
    {
        #region [ Constructor ]
        public WorkflowRepositoryValidator(ILogger logger, IWorkflowRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }

        public string Version { get; set; }

        public DateTimeOffset ActiveFrom { get; set; }

        public DateTimeOffset? ActiveTo { get; set; }

        public string Xaml { get; set; }

        public string Json { get; set; }

        public WorkflowStatus Status { get; set; }

        public string CustomActivities { get; set; }

        public byte[] Timestamp { get; set; }

        public DSWEnvironmentType DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public ICollection<WorkflowInstance> WorkflowInstances { get; set; }

        public ICollection<Role> Roles { get; set; }

        public ICollection<WorkflowRoleMapping> WorkflowRoleMappings { get; set; }

        public ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }
        #endregion

    }
}
