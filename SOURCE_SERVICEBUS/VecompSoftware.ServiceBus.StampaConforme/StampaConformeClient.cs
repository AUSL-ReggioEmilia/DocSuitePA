namespace VecompSoftware.ServiceBus.StampaConforme
{
    public class StampaConformeClient
    {
        public StampaConforme.BiblosDSConvSoapClient StampaConforme { get; }

        public StampaConformeClient()
        {
            StampaConforme = new StampaConforme.BiblosDSConvSoapClient();
        }

        public static string GetSignature(string signature, string source)
        {
            return signature.Replace("(SIGNATURE)", source);
        }
    }
}
