using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolContactManual : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public ProtocolContactManual() : this(Guid.NewGuid()) { }
        public ProtocolContactManual(Guid uniqueId)
            : base(uniqueId) { }
        #endregion

        #region[ Properties ]
        public short Year { get; set; }
        public int Number { get; set; }
        public string IdContactType { get; set; }
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
        public string idAD { get; set; }
        public string Type { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIdentification { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual Protocol Protocol { get; set; }
        public virtual ContactTitle ContactTitle { get; set; }
        public virtual ContactPlaceName ContactPlaceName { get; set; }

        #endregion
    }
}
