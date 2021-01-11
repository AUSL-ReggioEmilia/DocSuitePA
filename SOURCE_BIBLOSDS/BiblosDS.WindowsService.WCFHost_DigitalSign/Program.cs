using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace BiblosDS.WindowsService.WCFHost_DigitalSign
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));
            logger.Info("Program -> ServiceBase -> New");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new WCFHost_DigitalSign() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
