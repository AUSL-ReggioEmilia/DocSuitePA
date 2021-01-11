using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSUserValidator : ObjectValidator<UDSUser, UDSUserValidator>, IUDSUserValidator
    {
        #region [ Constructor ]
        public UDSUserValidator(ILogger logger, IUDSUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security)
            : base(logger, mapper, unitOfWork, security) { }
        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public int Environment { get; set; }

        public Guid IdUDS { get; set; }

        public string Account { get; set; }

        public AuthorizationRoleType AuthorizationType { get; set; }

        public UDSUserStatus Status { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public UDSRepository Repository { get; set; }
        #endregion
    }
}
