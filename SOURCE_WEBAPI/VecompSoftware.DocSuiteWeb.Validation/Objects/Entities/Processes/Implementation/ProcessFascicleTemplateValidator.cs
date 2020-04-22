using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Processes
{
    public class ProcessFascicleTemplateValidator : ObjectValidator<ProcessFascicleTemplate, ProcessFascicleTemplateValidator>, IProcessFascicleTemplateValidator
    {
        #region [ Constructor ]

        public ProcessFascicleTemplateValidator(ILogger logger, IValidatorMapper<ProcessFascicleTemplate, ProcessFascicleTemplateValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
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
        public string JsonModel { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Process Process { get; set; }
        public DossierFolder DossierFolder { get; set; }

        #endregion
    }
}
