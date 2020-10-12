using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons
{
    public class ContactValidator : ObjectValidator<Contact, ContactValidator>, IContactValidator
    {
        #region [ Constructor ]
        public ContactValidator(ILogger logger, IContactValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region[ Properties ]

        public int EntityId { get; set; }
        public int? IncrementalFather { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Code { get; set; }
        public string SearchCode { get; set; }
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
        public byte isActive { get; set; }
        public byte? isLocked { get; set; }
        public byte? isNotExpandable { get; set; }
        public string FullIncrementalPath { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public short isChanged { get; set; }
        public string IdContactType { get; set; }
        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        public byte[] Timestamp { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIdentification { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public ContactPlaceName PlaceName { get; set; }
        public ContactTitle Title { get; set; }
        public Role Role { get; set; }
        public Role RoleRootContact { get; set; }
        public ICollection<OChartItem> OChartItems { get; set; }
        public ICollection<ProtocolContact> ProtocolContacts { get; set; }
        public ICollection<ResolutionContact> ResolutionContacts { get; set; }
        public ICollection<Fascicle> Fascicles { get; set; }
        public ICollection<Dossier> Dossiers { get; set; }
        public ICollection<Tenant> Tenants { get; set; }
        public ICollection<MetadataValueContact> MetadataValueContacts { get; set; }
        #endregion

    }
}