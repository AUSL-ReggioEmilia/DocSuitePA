using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.FederaIdp
{
    public class SamlResponseStatusMapper : ISamlResponseStatusMapper
    {
        public SamlResponseStatus Map(StatusCodeType statusCode)
        {
            switch (statusCode.Value)
            {
                case SamlNamespaceHelper.SAML_STATUS_SUCCESS_NAMESPACE:
                    return SamlResponseStatus.Success;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTERERROR_NAMESPACE:
                    return SamlResponseStatus.RequesterError;
                case SamlNamespaceHelper.SAML_STATUS_RESPONDERERROR_NAMESPACE:
                    return SamlResponseStatus.ResponderError;
                case SamlNamespaceHelper.SAML_STATUS_VERSIONMISMATCHERROR_NAMESPACE:
                    return SamlResponseStatus.VersionMismatchError;
                case SamlNamespaceHelper.SAML_STATUS_AUTHNFAILED_NAMESPACE:
                    return SamlResponseStatus.AuthnFailed;
                case SamlNamespaceHelper.SAML_STATUS_INVALIDATTRNAMEORVALUE_NAMESPACE:
                    return SamlResponseStatus.InvalidAttrNameOrValue;
                case SamlNamespaceHelper.SAML_STATUS_INVALIDNAMEIDPOLICY_NAMESPACE:
                    return SamlResponseStatus.InvalidNameIDPolicy;
                case SamlNamespaceHelper.SAML_STATUS_NOAUTHNCONTEXT_NAMESPACE:
                    return SamlResponseStatus.NoAuthnContext;
                case SamlNamespaceHelper.SAML_STATUS_NOAVAILABLEIDP_NAMESPACE:
                    return SamlResponseStatus.NoAvailableIDP;
                case SamlNamespaceHelper.SAML_STATUS_NOPASSIVE_NAMESPACE:
                    return SamlResponseStatus.NoPassive;
                case SamlNamespaceHelper.SAML_STATUS_NOSUPPORTEDIDP_NAMESPACE:
                    return SamlResponseStatus.NoSupportedIDP;
                case SamlNamespaceHelper.SAML_STATUS_PARTIALLOGOUT_NAMESPACE:
                    return SamlResponseStatus.PartialLogout;
                case SamlNamespaceHelper.SAML_STATUS_PROXYCOUNTEXCEEDED_NAMESPACE:
                    return SamlResponseStatus.ProxyCountExceeded;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTDENIED_NAMESPACE:
                    return SamlResponseStatus.RequestDenied;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTUNSUPPORTED_NAMESPACE:
                    return SamlResponseStatus.RequestUnsupported;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTVERSIONDEPRECATED_NAMESPACE:
                    return SamlResponseStatus.RequestVersionDeprecated;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTVERSIONTOOHIGH_NAMESPACE:
                    return SamlResponseStatus.RequestVersionTooHigh;
                case SamlNamespaceHelper.SAML_STATUS_REQUESTVERSIONTOOLOW_NAMESPACE:
                    return SamlResponseStatus.RequestVersionTooLow;
                case SamlNamespaceHelper.SAML_STATUS_RESOURCENOTRECOGNIZED_NAMESPACE:
                    return SamlResponseStatus.ResourceNotRecognized;
                case SamlNamespaceHelper.SAML_STATUS_TOOMANYRESPONSES_NAMESPACE:
                    return SamlResponseStatus.TooManyResponses;
                case SamlNamespaceHelper.SAML_STATUS_UNKNOWNATTRPROFILE_NAMESPACE:
                    return SamlResponseStatus.UnknownAttrProfile;
                case SamlNamespaceHelper.SAML_STATUS_UNKNOWNPRINCIPAL_NAMESPACE:
                    return SamlResponseStatus.UnknownPrincipal;
                case SamlNamespaceHelper.SAML_STATUS_UNSUPPORTEDBINDING_NAMESPACE:
                    return SamlResponseStatus.UnsupportedBinding;
                default:
                    return SamlResponseStatus.GenericError;
            }
        }
    }
}
