using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceProcess;

namespace BiblosDS.WindowsService.StampaConforme.Office.Converter
{
    static class Program
    {

        static void Main()
        {
            var officeConverter = new OfficeConverterService();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("OfficeConverterService running on console mode.");
                var services = (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services");

                foreach (ServiceElement service in (ServiceElementCollection)services.Services)
                    foreach (ServiceEndpointElement endpoint in service.Endpoints)
                        Console.WriteLine("Address: {0}", endpoint.Address);

                officeConverter.ConsoleOnStart();
                Console.WriteLine("Press any key to stop program...");
                Console.Read();
                officeConverter.ConsoleOnStop();
            }
            else
                ServiceBase.Run(officeConverter);
        }

    }
}
