using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionKindDocumentSeriesValidator : ObjectValidator<ResolutionKindDocumentSeries, ResolutionKindDocumentSeriesValidator>, IResolutionKindDocumentSeriesValidator
    {
        #region [ Constructor ]
        public ResolutionKindDocumentSeriesValidator(ILogger logger, IResolutionKindDocumentSeriesValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public bool DocumentRequired { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ResolutionKind ResolutionKind { get; set; }
        public DocumentSeries DocumentSeries { get; set; }
        public DocumentSeriesConstraint DocumentSeriesConstraint { get; set; }
        #endregion
    }
}
