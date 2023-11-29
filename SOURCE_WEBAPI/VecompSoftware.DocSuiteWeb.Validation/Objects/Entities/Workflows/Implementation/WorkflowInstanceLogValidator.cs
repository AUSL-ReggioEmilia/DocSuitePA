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
    public class WorkflowInstanceLogValidator : ObjectValidator<WorkflowInstanceLog, WorkflowInstanceLogValidator>, IWorkflowInstanceLogValidator
    {
        #region [ Constructor ]
        public WorkflowInstanceLogValidator(ILogger logger, WorkflowInstanceLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string SystemComputer { get; set; }

        public string RegistrationUser { get; set; }

        public WorkflowInstanceLogType LogType { get; set; }

        public string LogDescription { get; set; }

        public SeverityLog? Severity { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]

        public WorkflowInstance WorkflowInstance { get; set; }


        #endregion


    }
}
