using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Documents.Signs
{
    public class RemoteSignProperty
    {
        #region [ Fields ]
        public const string ARUBA_DELEGATED_DOMAIN = "DelegatedDomain";
        public const string ARUBA_DELEGATED_PASSWORD = "DelegatedPassword";
        public const string ARUBA_DELEGATED_USER = "DelegatedUser";
        public const string ARUBA_OTP_AUTHTYPE = "OTPAuthType";
        public const string ARUBA_USER = "User";
        public const string ARUBA_CERTIFICATEID = "CertificateId";
        public const string PROVIDER_TYPE = "ProviderType";
        public const string REQUEST_TYPE = "RequestType";
        #endregion

        #region [ Contructors ]

        public RemoteSignProperty()
        {
            CustomProperties = new Dictionary<string, string>();
        }
        #endregion

        #region [ Properties ]
        public bool IsDefault { get; set; }
        public string Alias { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
        public string PIN { get; set; }
        public OTPType OTPType { get; set; }
        public RemoteSignType RemoteSignType { get; set; }
        public StorageInformationType StorageInformationType { get; set; }
        public IDictionary<string, string> CustomProperties { get; set; }
        #endregion
    }

}
