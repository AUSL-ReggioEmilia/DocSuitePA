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
    public class ResolutionRoleValidator : ObjectValidator<ResolutionRole, ResolutionRoleValidator>, IResolutionRoleValidator
    {
        #region [ Constructor ]
        public ResolutionRoleValidator(ILogger logger, IResolutionRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {
        }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int IdResolutionRoleType { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }


        #endregion

        #region [ Navigation Properties ]

        public Resolution Resolution { get; set; }

        public Role Role { get; set; }

        #endregion

    }
}
