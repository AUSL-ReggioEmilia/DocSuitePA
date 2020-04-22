using VecompSoftware.DocSuite.SPID.Model.Tokens;

namespace VecompSoftware.DocSuite.SPID.Model.Auth
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public TokenResult TokenResult { get; set; }
    }
}
