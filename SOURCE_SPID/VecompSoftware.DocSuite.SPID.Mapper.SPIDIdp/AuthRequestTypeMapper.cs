using System;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.SPIDIdp
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
                Destination = source.IdpEntityId,
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
                    Items = new string[] { string.Concat("https://www.spid.gov.it/SpidL", ((int)source.SPIDLevel)) }
                }
            };
        }
    }
}
