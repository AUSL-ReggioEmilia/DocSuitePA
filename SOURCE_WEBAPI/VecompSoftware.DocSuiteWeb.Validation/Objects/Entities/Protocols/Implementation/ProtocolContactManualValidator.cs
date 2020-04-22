using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class ProtocolContactManualValidator : ObjectValidator<ProtocolContactManual, ProtocolContactManualValidator>, IProtocolContactManualValidator
    {
        #region [ Constructor ]
        public ProtocolContactManualValidator(ILogger logger, IProtocolContactManualValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string ComunicationType { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Code { get; set; }
        public string FiscalCode { get; set; }
        public string Address { get; set; }
        public string CivicNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EMailAddress { get; set; }
        public string CertifydMail { get; set; }
        public string Note { get; set; }
        public string FullIncrementalPath { get; set; }
        public string IdContactType { get; set; }
        public string idAD { get; set; }
        public string Type { get; set; }
        public Guid UniqueId { get; set; }
        public string Account { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public string LastChangedUser { get; set; }
        public byte[] Timestamp { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIdentification { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public Protocol Protocol { get; set; }
        public ContactTitle ContactTitle { get; set; }
        public ContactPlaceName ContactPlaceName { get; set; }
        #endregion
    }
}
