using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Parameters;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Parameters;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Parameter
{
    public class ParameterValidator : ObjectValidator<Entity.Parameters.Parameter, ParameterValidator>, IParameterValidator
    {
        #region [ Constructor ]

        public ParameterValidator(ILogger logger, IParameterValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }
        public int Incremental { get; set; }
        public short LastUsedYear { get; set; }
        public int LastUsedNumber { get; set; }
        public bool Locked { get; set; }
        public string Password { get; set; }
        public short LastUsedidCategory { get; set; }
        public short LastUsedidRecipient { get; set; }
        public short LastUsedidContainer { get; set; }
        public short Version { get; set; }
        public short LastUsedidDistributionList { get; set; }
        public string DomainName { get; set; }
        public string AlternativePassword { get; set; }
        public string ServiceField { get; set; }
        public short LastUsedidRole { get; set; }
        public short LastUsedIdRoleUser { get; set; }
        public int? LastUsedidResolution { get; set; }
        public short LastUsedResolutionYear { get; set; }
        public short LastUsedResolutionNumber { get; set; }
        public short LastUsedBillNumber { get; set; }
        public short LastUsedYearReg { get; set; }
        public int? LastUsedNumberReg { get; set; }
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
