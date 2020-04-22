
namespace VecompSoftware.DocSuiteWeb.Model.WebAPI.Client
{
    public class Credential : ICredential
    {
        #region [ Constructor ]

        public Credential() { }

        #endregion

        #region [ Properties ]

        public string Password { get; set; }

        public string Username { get; set; }

        #endregion
    }
}
