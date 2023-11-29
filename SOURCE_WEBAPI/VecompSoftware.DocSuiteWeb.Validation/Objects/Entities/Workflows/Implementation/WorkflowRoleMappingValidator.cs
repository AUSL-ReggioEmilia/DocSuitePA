using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Workflows;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Workflows
{
    public class WorkflowRoleMappingValidator : ObjectValidator<WorkflowRoleMapping, WorkflowRoleMappingValidator>, IWorkflowRoleMappingValidator
    {
        #region [ Constructor ]
        public WorkflowRoleMappingValidator(ILogger logger, IWorkflowRoleMappingValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public string MappingTag { get; set; }

        public string IdInternalActivity { get; set; }

        public string AccountName { get; set; }

        public WorkflowAuthorizationType AuthorizationType { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public WorkflowRepository WorkflowRepository { get; set; }

        public Role Role { get; set; }

        #endregion

    }
}
