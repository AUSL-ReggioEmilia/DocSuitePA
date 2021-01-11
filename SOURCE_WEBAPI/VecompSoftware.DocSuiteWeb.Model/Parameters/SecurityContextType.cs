namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public enum SecurityContextType : int
    {
        //
        // Summary:
        //     The computer store. This represents the SAM store.
        Machine = 0,
        //
        // Summary:
        //     The domain store. This represents the AD DS store.
        Domain = 1,
        //
        // Summary:
        //     The application directory store. This represents the AD LDS store.
        ApplicationDirectory = 2,
        //
        // Summary:
        //     The OAuth2 authentication with bearer token.
        OAuth2 = ApplicationDirectory * 2
    }
}
