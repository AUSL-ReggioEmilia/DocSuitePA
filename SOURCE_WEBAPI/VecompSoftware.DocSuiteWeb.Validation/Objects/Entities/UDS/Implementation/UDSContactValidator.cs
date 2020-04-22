using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS
{
    public class UDSContactValidator : ObjectValidator<UDSContact, UDSContactValidator>, IUDSContactValidator
    {
        #region [ Constructor ]
        public UDSContactValidator(ILogger logger, IUDSContactValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity security)
            : base(logger, mapper, unitOfWork, security) { }
        #endregion

        #region [ Properties ]

        public Guid? IdUDS { get; set; }
        public int Environment { get; set; }
        public string ContactManual { get; set; }
        public int? ContactType { get; set; }
        public string ContactLabel { get; set; }
        public UDSRelationType RelationType { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string RegistrationUser { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Contact Contact { get; set; }
        public UDSRepository Repository { get; set; }
        #endregion

    }
}
