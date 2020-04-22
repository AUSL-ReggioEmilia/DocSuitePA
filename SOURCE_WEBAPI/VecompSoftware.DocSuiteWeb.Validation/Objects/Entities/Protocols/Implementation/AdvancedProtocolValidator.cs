using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Protocols;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Protocols
{
    public class AdvancedProtocolValidator : ObjectValidator<AdvancedProtocol, AdvancedProtocolValidator>, IAdvancedProtocolValidator
    {
        #region [ Constructor ]

        public AdvancedProtocolValidator(ILogger logger, IAdvancedProtocolValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }
        #endregion

        #region [ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string ServiceCategory { get; set; }
        public string Subject { get; set; }
        public string ServiceField { get; set; }
        public string Note { get; set; }
        public string Origin { get; set; }
        public int? Package { get; set; }
        public int? Lot { get; set; }
        public int? Incremental { get; set; }
        public int? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public string AccountingSectional { get; set; }
        public short? AccountingYear { get; set; }
        public DateTime? AccountingDate { get; set; }
        public int? AccountingNumber { get; set; }
        public bool? IsClaim { get; set; }
        public string ProtocolStatus { get; set; }
        public string IdentificationSdi { get; set; }
        public int? AccountingSectionalNumber { get; set; }
        public int? InvoiceYear { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
