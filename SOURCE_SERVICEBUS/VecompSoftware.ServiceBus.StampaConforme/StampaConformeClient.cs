using System;
using System.IO;

namespace VecompSoftware.ServiceBus.StampaConforme
{
    public class StampaConformeClient
    {
        private readonly StampaConforme.BiblosDSConvSoapClient _stampaConforme;

        public StampaConforme.BiblosDSConvSoapClient StampaConforme => _stampaConforme;

        public StampaConformeClient()
        {
            _stampaConforme = new StampaConforme.BiblosDSConvSoapClient();
        }

        public static string GetLabel(string signature)
        {
            string templatePath = Path.Combine(Environment.CurrentDirectory, "SignatureTemplate.xml");
            return File.ReadAllText(templatePath).Replace("(SIGNATURE)", signature);
        }
    }
}
