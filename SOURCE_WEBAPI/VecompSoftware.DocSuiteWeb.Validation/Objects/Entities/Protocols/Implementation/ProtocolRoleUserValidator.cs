using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolRoleUserValidator : ObjectValidator<ProtocolRoleUser, ProtocolRoleUserValidator>, IProtocolRoleUserValidator
    {
        #region [ Constructor ]
        public ProtocolRoleUserValidator(ILogger logger, IProtocolRoleUserValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public byte IsActive { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        public byte[] Timestamp { get; set; }
        public short Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Protocol Protocol { get; set; }
        public Role Role { get; set; }
        #endregion


    }
}
