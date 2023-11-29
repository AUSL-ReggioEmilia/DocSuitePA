using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionLogValidator : ObjectValidator<ResolutionLog, ResolutionLogValidator>, IResolutionLogValidator
    {
        #region [ Constructor ]

        public ResolutionLogValidator(ILogger logger, IResolutionLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, security, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public int EntityId { get; set; }
        public int IdResolution { get; set; }
        public DateTime LogDate { get; set; }
        public string SystemComputer { get; set; }
        public string RegistrationUser { get; set; }
        public string Program { get; set; }
        public string LogType { get; set; }
        public string LogDescription { get; set; }
        public SeverityLog? Severity { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Resolution Resolution { get; set; }
        #endregion
    }
}
