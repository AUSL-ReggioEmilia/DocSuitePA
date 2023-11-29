using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.DocumentArchives;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.DocumentArchives
{
    public class DocumentSeriesConstraintValidator : ObjectValidator<DocumentSeriesConstraint, DocumentSeriesConstraintValidator>, IDocumentSeriesConstraintValidator
    {
        #region [ Constructor ]
        public DocumentSeriesConstraintValidator(ILogger logger, IDocumentSeriesConstraintValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }

        #endregion

        #region[ Properties ]

        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public DocumentSeries DocumentSeries { get; set; }
        public ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        #endregion
    }
}