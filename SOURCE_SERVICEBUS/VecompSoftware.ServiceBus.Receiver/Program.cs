using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace VecompSoftware.ServiceBus.Receiver
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            //new Receiver();
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new Receiver()
            };
            if (Environment.UserInteractive && Debugger.IsAttached)
            {
                //Running in console mode
                Receiver receiver = new Receiver();
                receiver.Start();
                Thread.Sleep(Timeout.Infinite);
                receiver.Stop();
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
