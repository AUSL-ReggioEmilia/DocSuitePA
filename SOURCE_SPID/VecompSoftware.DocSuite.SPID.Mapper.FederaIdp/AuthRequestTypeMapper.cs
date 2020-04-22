using System;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.FederaIdp
{
    public class AuthRequestTypeMapper : IAuthRequestTypeMapper
    {
        public AuthnRequestType Map(SamlRequestOption source)
        {
            DateTime requestDateTime = DateTime.UtcNow;
            return new AuthnRequestType()
            {
                ID = string.Concat("_", source.Id.ToString()),
                Version = source.Version,
                IssueInstant = requestDateTime,
                Destination = source.Destination,
                AttributeConsumingServiceIndex = source.AttributeConsumingServiceIndex ?? 0,
                AttributeConsumingServiceIndexSpecified = source.AttributeConsumingServiceIndex.HasValue,
                ForceAuthnSpecified = true,
                ForceAuthn = source.SPIDLevel != SamlAuthLevel.SpidL1,
                AssertionConsumerServiceIndex = source.AssertionConsumerServiceIndex,
                AssertionConsumerServiceIndexSpecified = true,
                Issuer = new NameIDType()
                {
                    Format = SamlNamespaceHelper.SAML_ENTITY_NAMESPACE,
                    NameQualifier = source.SPDomain,
                    Value = source.SPDomain
                },
                NameIDPolicy = new NameIDPolicyType()
                {
                    Format = SamlNamespaceHelper.SAML_TRANSIENT_NAMESPACE,
                    AllowCreate = true
                },
                Conditions = new ConditionsType()
                {
                    NotBefore = requestDateTime.Add(source.NotBefore),
                    NotBeforeSpecified = true,
                    NotOnOrAfter = requestDateTime.Add(source.NotOnOrAfter),
                    NotOnOrAfterSpecified = true
                },
                RequestedAuthnContext = new RequestedAuthnContextType()
                {
                    Comparison = AuthnContextComparisonType.minimum,
                    ComparisonSpecified = true,
                    ItemsElementName = new ItemsChoiceType7[] { ItemsChoiceType7.AuthnContextClassRef },
                    Items = new string[] { AuthContextSecurityLevel(source.SPIDLevel) }
                }
            };
        }

        private string AuthContextSecurityLevel(SamlAuthLevel authLevel)
        {
            string result = string.Empty;
            switch (authLevel)
            {
                case SamlAuthLevel.SpidL1:
                    result = SamlNamespaceHelper.SAML_AUTHCONTEXTREF_PASSWORD_PROTECT_NAMESPACE;
                    break;
                case SamlAuthLevel.SpidL2:
                    result = SamlNamespaceHelper.SAML_AUTHCONTEXTREF_SECURE_REMOTE_PASSWORD_NAMESPACE;
                    break;
                case SamlAuthLevel.SpidL3:
                    result = SamlNamespaceHelper.SAML_AUTHCONTEXTREF_SMARTCARD_NAMESPACE;
                    break;
            }
            return result;
        }
    }
}
