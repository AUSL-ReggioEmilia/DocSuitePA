using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class ContactDTO : IContactDTO
    {
        #region [ Constructors ]

        public ContactDTO()
        {
        }

        public ContactDTO(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        #endregion

        #region [ Properties ]

        public int? Id { get; set; }

        public string Code { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string CivicNumber { get; set; }

        public string ZipCode { get; set; }

        public string CityCode { get; set; }

        public string FiscalCode { get; set; }

        public DateTime? BirthDate { get; set; }

        #endregion

        #region [ Methods ]

        public bool HasId()
        {
            return this.Id.HasValue && !this.Id.Equals(0);
        }

        #endregion
    }
}