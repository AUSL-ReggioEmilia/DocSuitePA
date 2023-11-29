using System;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Conservations;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Conservations
{
    public class ConservationValidator : ObjectValidator<Conservation, ConservationValidator>, IConservationValidator
    {
        #region [ Constructor ]
        public ConservationValidator(ILogger logger, IConservationValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        {

        }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public string EntityType { get; set; }
        public ConservationStatus Status { get; set; }
        public string Message { get; set; }
        public ConservationType Type { get; set; }
        public DateTimeOffset? SendDate { get; set; }
        public string Uri { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
