using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FasciclePeriodValidator : ObjectValidator<FasciclePeriod, FasciclePeriodValidator>, IFasciclePeriodValidator
    {
        #region [ Constructor ]
        public FasciclePeriodValidator(ILogger logger, IFasciclePeriodValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public bool IsActive { get; set; }

        public string PeriodName { get; set; }

        public double PeriodDays { get; set; }

        public string RegistrationUser { get; set; }

        /// <summary>
        /// Get or set RegistrationDate
        /// </summary>
        public DateTimeOffset RegistrationDate { get; set; }

        /// <summary>
        /// Get or set LastChangedUser
        /// </summary>
        public string LastChangedUser { get; set; }

        /// <summary>
        /// Get or set LastChangedDate
        /// </summary>
        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]

        public ICollection<CategoryFascicle> CategoryFascicles { get; set; }

        #endregion
    }
}
