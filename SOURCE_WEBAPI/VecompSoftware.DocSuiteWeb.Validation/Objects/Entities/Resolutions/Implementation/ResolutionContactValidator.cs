using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions
{
    public class ResolutionContactValidator : ObjectValidator<ResolutionContact, ResolutionContactValidator>, IResolutionContactValidator
    {
        #region [ Constructor ]
        public ResolutionContactValidator(ILogger logger, IResolutionContactValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        {
        }

        #endregion

        #region [ Properties ]


        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        //public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        public int IdResolution { get; set; }

        // La propretà ComunicationType dovrà essere dichiarata come enumeratore
        public string ComunicationType { get; set; }
        public short? Incremental { get; set; }

        #endregion

        #region [ Navigation Properties ]

        //public Resolution Resolution { get; set; }
        public Contact Contact { get; set; }

        public Resolution Resolution { get; set; }

        #endregion
    }
}
