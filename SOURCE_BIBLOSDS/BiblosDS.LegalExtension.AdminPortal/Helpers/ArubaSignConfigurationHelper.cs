using BiblosDS.LegalExtension.AdminPortal.Models;
using BiblosDS.Library.Common.Services;
using Newtonsoft.Json;
using System;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
    public class ArubaSignConfigurationHelper
    {
        #region [ Fields ]
        public static string NO_CREDENTIALS = "no-credentials";
        public static string SIGN_COMPLETE = "successfully-signed";
        public static string SIGN_ERROR = "error-signing";
        #endregion

        #region [ Methods ]
        public static ArubaSignModel GetArubaSignConfiguration(Guid idCompany, string username)
        {
            string signInfo = CustomerService.GetCustomerOrCompanySignInfo(idCompany, username);

            if (string.IsNullOrEmpty(signInfo))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ArubaSignModel>(signInfo);
        }
        #endregion
    }
}