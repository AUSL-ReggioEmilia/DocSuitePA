using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceProcess;

namespace BiblosDS.WindowsService.WCFStampaConformeConverterHost
{
    static class Program
    {

        static void Main()
        {
            var openOfficeConverter = new WCFStampaConformeConverter();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("WCFStampaConformeConverter running on console mode.");
                var services = (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services");

                foreach (ServiceElement service in (ServiceElementCollection)services.Services)
                    foreach (ServiceEndpointElement endpoint in service.Endpoints)
                        Console.WriteLine("Address: {0}", endpoint.Address);

                openOfficeConverter.ConsoleOnStart();
                Console.WriteLine("Press any key to stop program...");
                Console.Read();
                openOfficeConverter.ConsoleOnStop();
            }
            else
                ServiceBase.Run(openOfficeConverter);
        }

    }
}
