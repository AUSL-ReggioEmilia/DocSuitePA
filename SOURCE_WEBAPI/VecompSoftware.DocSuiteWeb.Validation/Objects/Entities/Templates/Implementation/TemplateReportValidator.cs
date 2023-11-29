using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates
{
    public class TemplateReportValidator : ObjectValidator<TemplateReport, TemplateReportValidator>, ITemplateReportValidator
    {
        #region [ Constructor ]
        public TemplateReportValidator(ILogger logger, ITemplateReportValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public Guid IdArchiveChain { get; set; }
        public TemplateReportStatus Status { get; set; }
        public int Environment { get; set; }
        public string ReportBuilderJsonModel { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
