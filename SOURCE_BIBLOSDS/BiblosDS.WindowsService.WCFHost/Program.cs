using log4net;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceProcess;
using VecompSoftware.BiblosDS.WindowsService.Common.Configurations;
using VecompSoftware.BiblosDS.WindowsService.WebAPI;

namespace BiblosDS.WindowsService.WCFHost
{
    static class Program
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        #endregion

        #region [ Methods ]
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            WebAPIBuilder.BuildWebAPI(ServiceConfiguration.WebAPIUrl);
            WCFHost wcfHost = new WCFHost();
            if (Environment.UserInteractive && Debugger.IsAttached)
            {
                Console.WriteLine("WCFHost running on console mode.");
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ServiceModelSectionGroup serviceModel = ServiceModelSectionGroup.GetSectionGroup(configuration);
                ServicesSection servicesSection = serviceModel.Services;

                foreach (ServiceEndpointElement endpoint in servicesSection.Services.Cast<ServiceElement>().Select(s => s.Endpoints).SelectMany(ss => ss.OfType<ServiceEndpointElement>()))
                {
                    Console.WriteLine("Address: {0}", endpoint.Address);
                }

                wcfHost.Start();

                Console.WriteLine("Press any key to stop program...");
                Console.ReadKey();

                wcfHost.Stop();
            }
            else
            {
                ServiceBase.Run(wcfHost);
            }
        }
        #endregion        
    }
}
