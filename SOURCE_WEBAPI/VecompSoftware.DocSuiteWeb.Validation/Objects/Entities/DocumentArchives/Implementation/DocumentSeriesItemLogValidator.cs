using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesItemLogValidator : ObjectValidator<DocumentSeriesItemLog, DocumentSeriesItemLogValidator>, IDocumentSeriesItemLogValidator
    {
        #region [ Constructor ]
        public DocumentSeriesItemLogValidator(ILogger logger, IDocumentSeriesItemLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security)
            : base(logger, mapper, unitOfWork, security) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public int EntityId { get; set; }
        public DateTime LogDate { get; set; }
        public int IdDocumentSeriesItem { get; set; }
        public string SystemComputer { get; set; }
        public string RegistrationUser { get; set; }
        public string Program { get; set; }
        public string LogType { get; set; }
        public string LogDescription { get; set; }
        public SeverityLog? Severity { get; set; }
        public string Hash { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItem { get; set; }
        #endregion
    }
}
