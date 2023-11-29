using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Templates;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Templates
{
    public class TemplateCollaborationDocumentRepositoryValidator : ObjectValidator<TemplateCollaborationDocumentRepository, TemplateCollaborationDocumentRepositoryValidator>, ITemplateCollaborationDocumentRepositoryValidator
    {
        #region [ Constructor ]

        public TemplateCollaborationDocumentRepositoryValidator(ILogger logger, ITemplateCollaborationDocumentRepositoryValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentsecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentsecurity, parameterEnvSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public ChainType ChainType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion
    }
}
