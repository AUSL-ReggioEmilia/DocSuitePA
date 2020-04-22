using System;
using System.Collections;
using System.Configuration;
using System.Configuration.Install;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using log4net.Config;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace JeepService
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
                        var window_height = Console.WindowHeight;
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
            var defaultCulture = ConfigurationManager.AppSettings["DefaultCulture"];
            if (!string.IsNullOrEmpty(defaultCulture))
            {
                var culture = new CultureInfo(defaultCulture);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            //Inizializzo Log4Net
            bool result;
            if (bool.TryParse(ConfigurationManager.AppSettings["log4net"], out result) && result)
            {
                if (bool.TryParse(ConfigurationManager.AppSettings["AutoLog"], out result) && result)
                {
                    BasicConfigurator.Configure();
                    // Aggiungo il Log di JeepService
                    DynamicLoggerConfigurator.AddCompleteLogger("JeepService");

                    // Aggiungo il Log di DocSuite
                    DynamicLoggerConfigurator.AddCompleteLogger("DocSuiteLog", LogLevel.All.ToLevel());

                    // Finalizzo l'impostazione
                    DynamicLoggerConfigurator.Configured();
                }
                else
                {
                    FileLogger.Initialize();
                }
            }

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
                    var service = new JeepService();
                    service.Debug();
                    if (ConsolePresent)
                    {
                        Console.WriteLine("System running; press any key to stop");
                        if (args.Length > 1)
                        {
                            Console.Write("Spegnimento fra {0} sec", args[1]);
                            Thread.Sleep(int.Parse(args[1]) * 1000);
                            service.Stop();
                        }
                        else Console.ReadKey(true);
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
                    var servicesToRun = new ServiceBase[] { new JeepService() };
                    ServiceBase.Run(servicesToRun);
                    rethrow = false;
                }
                return 0;
            }
            catch (Exception ex)
            {
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
