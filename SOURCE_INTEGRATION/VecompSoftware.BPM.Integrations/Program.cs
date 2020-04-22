using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace VecompSoftware.BPM.Integrations
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new Service()
            };

            if (Environment.UserInteractive && Debugger.IsAttached)
            {
                //Running in console mode
                Service service = new Service();
                service.Start();
                Thread.Sleep(Timeout.Infinite);
                service.Stop();
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
