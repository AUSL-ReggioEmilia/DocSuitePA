using System;
using VecompSoftware.DocSuite.SPID.Common.Helpers.SAML;
using VecompSoftware.DocSuite.SPID.Mapper.SAML;
using VecompSoftware.DocSuite.SPID.Model.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.SPIDIdp
{
    public class LogOutRequestTypeMapper : ILogOutRequestTypeMapper
    {
        public LogoutRequestType Map(SamlRequestOption source)
        {
            DateTime requestDateTime = DateTime.UtcNow;
            LogoutRequestType logoutRequest = new LogoutRequestType()
            {
                ID = string.Concat("_", source.Id.ToString()),
                Version = source.Version,
                IssueInstant = requestDateTime,
                Destination = source.Destination,
                Issuer = new NameIDType
                {
                    Format = SamlNamespaceHelper.SAML_ENTITY_NAMESPACE,
                    NameQualifier = source.SPDomain,
                    Value = source.SPDomain
                },
                Item = new NameIDType
                {
                    SPNameQualifier = source.SPDomain,
                    Format = SamlNamespaceHelper.SAML_TRANSIENT_NAMESPACE,
                    Value = source.SubjectNameId
                },
                NotOnOrAfterSpecified = true,
                NotOnOrAfter = requestDateTime.Add(source.NotOnOrAfter),
                Reason = SamlNamespaceHelper.SAML_LOGOUT_USER_NAMESPACE,
                SessionIndex = new string[] { source.AuthnStatementSessionIndex }
            };
            return logoutRequest;
        }
    }
}
