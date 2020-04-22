namespace VecompSoftware.DocSuite.SPID.Common.Helpers.SAML
{
    public class SamlNamespaceHelper
    {
        #region [ Request ]
        public const string SAML_PROTOCOL_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:protocol";
        public const string SAML_ASSERTION_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:assertion";
        public const string SAML_ENTITY_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
        public const string SAML_TRANSIENT_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
        public const string SAML_PROTOCOL_BINDING_REDIRECT_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";
        public const string SAML_LOGOUT_USER_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:logout:user";
        public const string SAML_AUTHCONTEXTREF_PASSWORD_PROTECT_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";
        public const string SAML_AUTHCONTEXTREF_SECURE_REMOTE_PASSWORD_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:ac:classes:SecureRemotePassword";
        public const string SAML_AUTHCONTEXTREF_SMARTCARD_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:ac:classes:Smartcard";
        #endregion

        #region [ Response Status ]
        public const string SAML_STATUS_SUCCESS_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:Success";
        public const string SAML_STATUS_REQUESTERERROR_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:Requester";
        public const string SAML_STATUS_RESPONDERERROR_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:Responder";
        public const string SAML_STATUS_VERSIONMISMATCHERROR_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:VersionMismatch";
        public const string SAML_STATUS_AUTHNFAILED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:AuthnFailed";
        public const string SAML_STATUS_INVALIDATTRNAMEORVALUE_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue";
        public const string SAML_STATUS_INVALIDNAMEIDPOLICY_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy";
        public const string SAML_STATUS_NOAUTHNCONTEXT_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext";
        public const string SAML_STATUS_NOAVAILABLEIDP_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP";
        public const string SAML_STATUS_NOPASSIVE_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:NoPassive";
        public const string SAML_STATUS_NOSUPPORTEDIDP_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP";
        public const string SAML_STATUS_PARTIALLOGOUT_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:PartialLogout";
        public const string SAML_STATUS_PROXYCOUNTEXCEEDED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded";
        public const string SAML_STATUS_REQUESTDENIED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:RequestDenied";
        public const string SAML_STATUS_REQUESTUNSUPPORTED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported";
        public const string SAML_STATUS_REQUESTVERSIONDEPRECATED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated";
        public const string SAML_STATUS_REQUESTVERSIONTOOHIGH_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh";
        public const string SAML_STATUS_REQUESTVERSIONTOOLOW_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow";
        public const string SAML_STATUS_RESOURCENOTRECOGNIZED_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized";
        public const string SAML_STATUS_TOOMANYRESPONSES_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:TooManyResponses";
        public const string SAML_STATUS_UNKNOWNATTRPROFILE_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile";
        public const string SAML_STATUS_UNKNOWNPRINCIPAL_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal";
        public const string SAML_STATUS_UNSUPPORTEDBINDING_NAMESPACE = "urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding";
        #endregion
    }
}
