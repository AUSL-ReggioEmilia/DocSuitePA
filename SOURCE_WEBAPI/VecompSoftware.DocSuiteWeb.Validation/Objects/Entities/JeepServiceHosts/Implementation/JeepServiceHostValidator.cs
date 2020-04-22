using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.JeepServiceHost;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.JeepServiceHosts
{
    public class JeepServiceHostValidator : ObjectValidator<JeepServiceHost, JeepServiceHostValidator>, IJeepServiceHostValidator
    {
        #region [ Constructor ]
        public JeepServiceHostValidator(ILogger logger, IJeepServiceHostValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public string Hostname { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
