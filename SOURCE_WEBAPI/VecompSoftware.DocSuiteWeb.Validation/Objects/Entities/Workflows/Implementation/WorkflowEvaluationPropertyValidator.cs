using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowEvaluationPropertyValidator : ObjectValidator<WorkflowEvaluationProperty, WorkflowEvaluationPropertyValidator>, IWorkflowEvaluationPropertyValidator
    {
        #region [ Constructor ]
        public WorkflowEvaluationPropertyValidator(ILogger logger, IWorkflowEvaluationPropertyValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }
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
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public WorkflowRepository WorkflowRepository { get; set; }
        #endregion
    }
}
