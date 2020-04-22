using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;
using VecompSoftware.Services.Logging;

namespace JeepService.JeepService.DashboardService
{
    static class Program
    {

        private static bool? _consolePresent;
        public static bool ConsolePresent
        {
            get
            {
                if (_consolePresent == null)
                {
                    _consolePresent = true;
                    try
                    {
                        var windowHeight = Console.WindowHeight;
                    }
                    catch
                    {
                        _consolePresent = false;
                    }
                }
                return _consolePresent.Value;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            //FileLogger.Initialize();
            bool install = false, uninstall = false, console = false, rethrow = false;

            try
            {
                foreach (var arg in args)
                {
                    switch (arg)
                    {
                        case "-i":
                        case "-install":
                            install = true; break;
                        case "-u":
                        case "-uninstall":
                            uninstall = true; break;
                        case "-c":
                        case "-console":
                            console = true; break;
                        default:
                            Console.Error.WriteLine("Argument not expected: " + arg);
                            break;
                    }
                }
                if (uninstall)
                {
                    Install(true, args);
                }
                if (install)
                {
                    Install(false, args);
                }
                if (console)
                {
                    Console.WriteLine("Starting...");
                    var service = new DashboardService();
                    service.Debug();
                    if (ConsolePresent)
                    {
                        Console.WriteLine("System running; press any key to stop");
                        Console.ReadKey(true);
                        Console.WriteLine("System stopped");
                    }
                    else
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                }
                else if (!(install || uninstall))
                {
                    rethrow = true; // so that windows sees error...
                    var servicesToRun = new ServiceBase[] { new DashboardService() };
                    ServiceBase.Run(servicesToRun);
                    rethrow = false;
                }
                return 0;
            }
            catch (Exception ex)
            {
                FileLogger.Error("LiveUpdate","Errore del servizio LiveUpdate", ex);
                if (rethrow) throw;
                Console.Error.WriteLine(ex.Message);
                if (console)
                {
                    Console.ReadLine();
                }
                return -1;
            }
        }

        static void Install(bool undo, string[] args)
        {
            try
            {
                Console.WriteLine(undo ? "uninstalling" : "installing");
                using (var inst = new AssemblyInstaller(typeof(Program).Assembly, args))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            inst.Uninstall(state);
                        }
                        else
                        {
                            inst.Install(state);
                            inst.Commit(state);
                        }
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch
                        { }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
