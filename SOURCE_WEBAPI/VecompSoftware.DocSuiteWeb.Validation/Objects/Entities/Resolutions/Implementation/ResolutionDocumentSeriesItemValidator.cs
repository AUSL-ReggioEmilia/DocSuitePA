using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionDocumentSeriesItemValidator : ObjectValidator<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItemValidator>, IResolutionDocumentSeriesItemValidator
    {
        #region [ Constructor ]
        public ResolutionDocumentSeriesItemValidator(ILogger logger, IValidatorMapper<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItemValidator> mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity) : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }
        #endregion


        #region [ Properties ]
        public Guid UniqueIdResolution { get; set; }
        public Guid UniqueIdDocumentSeriesItem { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public DocumentSeriesItem DocumentSeriesItems { get; set; }
        public Resolution Resolutions { get; set; }
        #endregion

    }
}
