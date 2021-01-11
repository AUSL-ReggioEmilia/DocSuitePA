using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.Services;
using System.ComponentModel;


namespace BiblosDS.Library.Common.Services
{
    public class UserCompaniesConfiguration
    {
        private static UserCompaniesConfiguration _instance;
        private static object syncLock = new object();

        private readonly PreservationService _preservationService;
        private static BindingList<Company> availableCompanies;
        protected UserCompaniesConfiguration()
        {
            _preservationService = new PreservationService();
        }

        public BindingList<Company> AvailableCompanies(string idCustomer)
        {
            BindingList<Company> currentUserCompanies = new BindingList<Company>(_preservationService.GetCustomerCompanies(idCustomer));
            availableCompanies = new BindingList<Company>(currentUserCompanies);
            return currentUserCompanies;
        }


        public static BindingList<Company> GetCachedAvailableCompanies()
        {
            return availableCompanies;
        }

        public static UserCompaniesConfiguration GetUserAvailableCompanies()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserCompaniesConfiguration();
                    }
                }
            }
            return _instance;
        }
    }
}
