using System.Linq;
using System.Web;
using VecompSoftware.DocSuiteWeb.Common.Securities;

namespace VecompSoftware.DocSuite.Private.WebAPI.Scope
{
    public class WebAPICurrentIdentity : ICurrentIdentity
    {
        #region [ Fields ]

        private const string _default_anonymous_api = "localmachine\\anonymous_api";
        private string _fullUserName = _default_anonymous_api;
        private string _account = string.Empty;
        private string _domain = string.Empty;
        #endregion

        #region [ Properties ]
        public string FullUserName
        {
            get
            {
                if (_fullUserName.Equals(_default_anonymous_api) && HttpContext.Current != null && HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    _fullUserName = HttpContext.Current.User.Identity.Name;
                }
                return _fullUserName;
            }
        }

        public string Account
        {
            get
            {
                if (string.IsNullOrEmpty(_account) && FullUserName.Contains("\\"))
                {
                    _account = FullUserName.Split('\\').LastOrDefault();
                }
                return _account;
            }
        }

        public string Domain
        {
            get
            {
                if (string.IsNullOrEmpty(_domain) && FullUserName.Contains("\\"))
                {
                    _domain = FullUserName.Split('\\').FirstOrDefault();
                }
                return _domain;
            }
        }

        #endregion

        #region [ Constructor ]

        #endregion
    }
}
