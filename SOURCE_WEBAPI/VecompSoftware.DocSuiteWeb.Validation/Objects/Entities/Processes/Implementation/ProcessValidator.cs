using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes
{
    public class ProcessValidator : ObjectValidator<Process, ProcessValidator>, IProcessValidator
    {
        #region [ Constructor ]

        public ProcessValidator(ILogger logger, IValidatorMapper<Process, ProcessValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
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
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Note { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Dossier Dossier { get; set; }
        public Category Category { get; set; }
        public ICollection<ProcessFascicleTemplate> FascicleTemplates { get; set; }
        public ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }

        #endregion

    }
}
