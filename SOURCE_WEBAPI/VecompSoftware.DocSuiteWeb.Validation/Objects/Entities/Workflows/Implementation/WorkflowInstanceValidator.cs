using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowInstanceValidator : ObjectValidator<WorkflowInstance, WorkflowInstanceValidator>, IWorkflowInstanceValidator
    {
        #region [ Constructor ]
        public WorkflowInstanceValidator(ILogger logger, IWorkflowInstanceValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public Guid? InstanceId { get; set; }

        public string Json { get; set; }

        public WorkflowStatus Status { get; set; }

        public byte[] Timestamp { get; set; }
        public string Subject { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public WorkflowRepository WorkflowRepository { get; set; }

        public ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public ICollection<WorkflowActivity> WorkflowActivities { get; set; }

        public ICollection<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; }

        public ICollection<Collaboration> Collaborations { get; set; }

        public ICollection<WorkflowInstanceRole> WorkflowInstanceRoles { get; set; }

        public ICollection<Dossier> Dossiers { get; set; }

        public ICollection<Fascicle> Fascicles { get; set; }
        #endregion
    }
}
