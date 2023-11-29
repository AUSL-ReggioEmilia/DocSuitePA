using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionKindValidator : ObjectValidator<ResolutionKind, ResolutionKindValidator>, IResolutionKindValidator
    {
        #region [ Constructor ]
        public ResolutionKindValidator(ILogger logger, IResolutionKindValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool AmountEnabled { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public ICollection<Resolution> Resolutions { get; set; }
        public ICollection<ResolutionKindDocumentSeries> ResolutionKindDocumentSeries { get; set; }
        #endregion

    }
}
