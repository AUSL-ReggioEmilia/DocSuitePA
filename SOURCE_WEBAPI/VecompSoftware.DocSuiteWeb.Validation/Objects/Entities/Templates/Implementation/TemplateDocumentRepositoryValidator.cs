using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates
{
    public class TemplateDocumentRepositoryValidator : ObjectValidator<TemplateDocumentRepository, TemplateDocumentRepositoryValidator>, ITemplateDocumentRepositoryValidator
    {
        #region [ Constructor ]
        public TemplateDocumentRepositoryValidator(ILogger logger, ITemplateDocumentRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region[ Properties ]
        public Guid UniqueId { get; set; }
        public TemplateDocumentationRepositoryType Status { get; set; }
        public string Name { get; set; }
        public string QualityTag { get; set; }
        public Guid IdArchiveChain { get; set; }


        public int Version { get; set; }
        public string Object { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region[ Navigation Properties ]      

        #endregion


    }
}
