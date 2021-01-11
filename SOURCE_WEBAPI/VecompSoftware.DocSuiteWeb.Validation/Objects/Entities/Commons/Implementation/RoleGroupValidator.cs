using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class RoleGroupValidator : ObjectValidator<RoleGroup, RoleGroupValidator>, IRoleGroupValidator
    {
        #region [ Constructor ]
        public RoleGroupValidator(ILogger logger, IRoleGroupValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public Guid UniqueId { get; set; }

        public string GroupName { get; set; }
        public string ProtocolRights { get; set; }
        public string ResolutionRights { get; set; }
        public string DocumentRights { get; set; }
        public string DocumentSeriesRights { get; set; }
        public string FascicleRights { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Role Role { get; set; }

        public SecurityGroup SecurityGroup { get; set; }


        #endregion
    }
}