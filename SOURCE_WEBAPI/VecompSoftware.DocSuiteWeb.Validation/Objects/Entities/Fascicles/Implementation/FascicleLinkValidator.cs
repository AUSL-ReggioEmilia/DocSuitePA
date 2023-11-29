using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleLinkValidator : ObjectValidator<FascicleLink, FascicleLinkValidator>, IFascicleLinkValidator
    {
        #region [ Constructor ]
        public FascicleLinkValidator(ILogger logger, IFascicleLinkValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public FascicleLinkType FascicleLinkType { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]
        public Fascicle Fascicle { get; set; }

        #endregion
    }
}
