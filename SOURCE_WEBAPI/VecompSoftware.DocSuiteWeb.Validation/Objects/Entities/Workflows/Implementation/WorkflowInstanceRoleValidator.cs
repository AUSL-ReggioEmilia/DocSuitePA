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
    public class WorkflowInstanceRoleValidator : ObjectValidator<WorkflowInstanceRole, WorkflowInstanceRoleValidator>, IWorkflowInstanceRoleValidator
    {
        #region [ Constructor ]
        public WorkflowInstanceRoleValidator(ILogger logger, IWorkflowInstanceRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {

        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public AuthorizationRoleType AuthorizationType { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public WorkflowInstance WorkflowInstance { get; set; }

        public Role Role { get; set; }

        #endregion
    }
}
