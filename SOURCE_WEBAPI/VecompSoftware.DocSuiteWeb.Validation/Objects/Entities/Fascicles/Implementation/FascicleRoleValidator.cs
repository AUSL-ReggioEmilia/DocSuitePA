using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles
{
    public class FascicleRoleValidator : ObjectValidator<FascicleRole, FascicleRoleValidator>, IFascicleRoleValidator
    {
        #region [ Constructor ]
        public FascicleRoleValidator(ILogger logger, IFascicleRoleValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public AuthorizationRoleType RoleAuthorizationType { get; set; }

        public byte[] Timestamp { get; set; }

        public bool IsMaster { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public Fascicle Fascicle { get; set; }

        public Role Role { get; set; }
        #endregion
    }
}
