using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowPropertyValidator : ObjectValidator<WorkflowProperty, WorkflowPropertyValidator>, IWorkflowPropertyValidator
    {
        #region [ Constructor ]
        public WorkflowPropertyValidator(ILogger logger, IWorkflowPropertyValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public WorkflowType WorkflowType { get; set; }

        public WorkflowPropertyType PropertyType { get; set; }

        public long? ValueInt { get; set; }

        public DateTime? ValueDate { get; set; }

        public double? ValueDouble { get; set; }

        public bool? ValueBoolean { get; set; }

        public Guid? ValueGuid { get; set; }

        public string ValueString { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public WorkflowInstance WorkflowInstance { get; set; }

        public WorkflowActivity WorkflowActivity { get; set; }
        #endregion
    }
}
