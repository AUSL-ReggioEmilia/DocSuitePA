
namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IContactDTO : IAPIArgument
    {

        #region [ Properties ]

        int? Id { get; set; }

        string Code { get; set; }

        string EmailAddress { get; set; }

        string Description { get; set; }

        string PhoneNumber { get; set; }

        string Address { get; set; }

        string City { get; set; }

        string CivicNumber { get; set; }

        string ZipCode { get; set; }

        string CityCode { get; set; }

        string FiscalCode { get; set; }

        #endregion

    }
}
