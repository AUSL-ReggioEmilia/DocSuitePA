namespace VecompSoftware.DocSuite.SPID.AuthEngine.Models
{
    public class IdentityProvider
    {
        public string IdpCode { get; set; }
        public string SingleSignOnService { get; set; }
        public string SingleLogoutService { get; set; }
        public string IssuerId { get; set; }
    }
}
