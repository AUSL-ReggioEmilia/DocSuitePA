using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes
{
    public class ProcessFascicleWorkflowRepositoryValidator : ObjectValidator<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepositoryValidator>, IProcessFascicleWorkflowRepositoryValidator
    {
        #region [ Constructor ]

        public ProcessFascicleWorkflowRepositoryValidator(ILogger logger, IValidatorMapper<ProcessFascicleWorkflowRepository, ProcessFascicleWorkflowRepositoryValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity) 
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {

        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Process Process { get; set; }
        public DossierFolder DossierFolder { get; set; }
        public WorkflowRepository WorkflowRepository { get; set; }

        #endregion
    }
}
