using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Protocols
{
    public class ProtocolContactManualModel
    {
        #region [ Constructor ]
        public ProtocolContactManualModel()
        {

        }
        #endregion

        #region [ Properties ]

        public int Incremental { get; set; }
        public ContactType BaseContactType { get; set; }
        public ComunicationType ComunicationType { get; set; }
        public ProtocolContactType ContactType { get; set; }
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
        public string EMail { get; set; }
        public string CertifiedEmail { get; set; }
        public string Note { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIdentification { get; set; }
        #endregion

    }
}
