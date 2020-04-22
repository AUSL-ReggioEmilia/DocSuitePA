namespace VecompSoftware.DocSuite.SPID.Model.Auth
{
    public class TokenRequest
    {
        public string IdpName { get; set; }
        public string ReferenceCode { get; set; }
        public string Token { get; set; }
        public TokenGrantType TokenGrantType { get; set; }
    }
}
