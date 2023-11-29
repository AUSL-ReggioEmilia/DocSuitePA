
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class ContactModel
    {
        #region [ Constructors ]

        public ContactModel()
        {
            UniqueId = Guid.NewGuid();
        }

        public ContactModel(string emailAddress)
        {
            Email = emailAddress;
        }

        #endregion

        #region [ Properties ]

        public int? Id { get; set; }
        public int? EntityId { get; set; }
        public ContactType ContactType { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string CertifiedMail { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string FiscalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CivicNumber { get; set; }
        public string ZipCode { get; set; }
        public string CityCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Note { get; set; }
        public string Nationality { get; set; }
        public LanguageType? Language { get; set; }
        public string SDIIndentification { get; set; }
        public Guid UniqueId { get; set; }
        public Guid? ParentInsertId { get; set; }
        public int? IncrementalFather { get; set; }
        public string SearchCode { get; set; }
        public short? IdPlaceName { get; set; }
        public short? IdRole { get; set; }
        public bool IsActive { get; set; }
        public byte? IsLocked { get; set; }
        public byte? IsNotExpandable { get; set; }
        public int? IdTitle { get; set; }
        public short? IdRoleRootContact { get; set; }
        public string FullIncrementalPath { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return Id.HasValue && !Id.Equals(0);
        }

        #endregion
    }
}
