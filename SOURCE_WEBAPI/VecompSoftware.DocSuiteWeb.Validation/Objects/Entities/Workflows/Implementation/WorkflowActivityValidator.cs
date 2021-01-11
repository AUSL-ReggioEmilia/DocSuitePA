using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowActivityValidator : ObjectValidator<WorkflowActivity, WorkflowActivityValidator>, IWorkflowActivityValidator
    {
        public WorkflowActivityValidator(ILogger logger, IWorkflowActivityValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public WorkflowActivityType ActivityType { get; set; }

        public WorkflowActivityAction ActivityAction { get; set; }

        public WorkflowActivityArea ActivityArea { get; set; }

        public WorkflowStatus Status { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public string Subject { get; set; }

        public byte[] Timestamp { get; set; }

        public Guid? IdArchiveChain { get; set; }

        public WorkflowPriorityType? Priority { get; set; }

        public string Note { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public ICollection<WorkflowActivityLog> WorkflowActivityLogs { get; set; }

        public ICollection<WorkflowAuthorization> WorkflowAuthorizations { get; set; }

        public WorkflowInstance WorkflowInstance { get; set; }

        public DocumentUnit DocumentUnitReferenced { get; set; }

        public Tenant Tenant { get; set; }
        #endregion
    }
}
