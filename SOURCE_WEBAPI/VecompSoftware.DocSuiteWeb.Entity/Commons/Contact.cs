using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class Contact : DSWBaseEntity
    {
        #region [ Constructor ]
        public Contact() : this(Guid.NewGuid()) { }
        public Contact(Guid uniqueId)
            : base(uniqueId)
        {
            OChartItems = new HashSet<OChartItem>();
            ProtocolContacts = new HashSet<ProtocolContact>();
            Fascicles = new HashSet<Fascicle>();
            Dossiers = new HashSet<Dossier>();
            CategoryFascicles = new HashSet<CategoryFascicle>();
            ResolutionContacts = new HashSet<ResolutionContact>();
            UDSContacts = new HashSet<UDSContact>();
            Tenants = new HashSet<Tenant>();
            MetadataValueContacts = new HashSet<MetadataValueContact>();
        }

        #endregion

        #region[ Properties ]

        public string Code { get; set; }
        public string FiscalCode { get; set; }
        public string Address { get; set; }
        public string CivicNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CertifiedMail { get; set; }
        public string Note { get; set; }
        public string FullIncrementalPath { get; set; }
        public int? IncrementalFather { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string SearchCode { get; set; }
        public byte IsActive { get; set; }
        public byte? IsLocked { get; set; }
        public byte? IsNotExpandable { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public short IsChanged { get; set; }
        public string IdContactType { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIdentification { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual ContactPlaceName PlaceName { get; set; }
        public virtual ContactTitle Title { get; set; }
        public virtual Role Role { get; set; }
        public virtual Role RoleRootContact { get; set; }
        public virtual ICollection<OChartItem> OChartItems { get; set; }
        public virtual ICollection<ProtocolContact> ProtocolContacts { get; set; }
        public virtual ICollection<ResolutionContact> ResolutionContacts { get; set; }
        public virtual ICollection<Fascicle> Fascicles { get; set; }
        public virtual ICollection<CategoryFascicle> CategoryFascicles { get; set; }
        public virtual ICollection<Dossier> Dossiers { get; set; }
        public virtual ICollection<UDSContact> UDSContacts { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<MetadataValueContact> MetadataValueContacts { get; set; }
        #endregion
    }
}
