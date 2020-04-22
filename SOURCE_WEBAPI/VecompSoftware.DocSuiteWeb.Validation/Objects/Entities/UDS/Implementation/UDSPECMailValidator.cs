using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSPECMailValidator : ObjectValidator<UDSPECMail, UDSPECMailValidator>, IUDSPECMailValidator
    {
        #region [ Constructor ]
        public UDSPECMailValidator(ILogger logger, IUDSPECMailValidatorMapper mapper,
            IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public Guid IdUDS { get; set; }
        public int Environment { get; set; }
        public UDSRelationType RelationType { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public UDSRepository Repository { get; set; }
        public PECMail PECMail { get; set; }
        #endregion
    }
}
