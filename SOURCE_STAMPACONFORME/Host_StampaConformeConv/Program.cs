using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Host_StampaConformeConv
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {               

                // Create a ServiceHost for the CalculatorService type and 
                // provide the base address.
                var serviceHostStampaConforme = new ServiceHost(typeof(BiblosDS.WCF.StampaConforme.Converter.StampaConformeConverter));

                // Open the ServiceHostBase to create listeners and start 
                // listening for messages.
                serviceHostStampaConforme.Open();
                Console.Write("OpenOffice stampa conforme converter started OK..");
                Console.ReadLine();
                serviceHostStampaConforme.Close();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                Console.ReadLine();
            }
        }
    }
}
