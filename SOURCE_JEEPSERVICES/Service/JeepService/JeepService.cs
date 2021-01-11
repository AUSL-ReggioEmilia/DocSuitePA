using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Serialization;
using JeepService.Classes;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;
using Configuration = VecompSoftware.JeepService.Common.Configuration;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace JeepService
{
    partial class JeepService : ServiceBase
    {

        #region [ Fields ]

        public const string Logger = "JeepService";
        private static readonly string ApplicationVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        private Configuration _configuration;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private static readonly Dictionary<ThreadedModule, Thread> JeepModules = new Dictionary<ThreadedModule, Thread>();

        Thread _thread;

        #endregion

        #region [ Properties ]
        public static Dictionary<string, ThreadedModule> Modules
        {
            get { return _modules; }
        }
        /// <summary>
        /// Dictionary che contiene gli spool caricati.
        /// </summary>
        private static readonly Dictionary<string, Spool> Spools = new Dictionary<string, Spool>();
        public Configuration Cfg
        {
            get
            {
                if (_configuration == null)
                {
                    var directoryInfo = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent;
                    if (directoryInfo != null)
                    {
                        var path = Path.Combine(directoryInfo.FullName, ConfigurationManager.AppSettings["JeepConfig"]);
                        if (!File.Exists(path))
                        {
                            throw new FileNotFoundException(string.Format("File di configurazione [{0}] non trovato.", path), path);
                        }

                        var serializer = new XmlSerializer(typeof(Configuration));
                        using (Stream stream = new FileStream(path, FileMode.Open))
                        {
                            _configuration = (Configuration)serializer.Deserialize(stream);
                        }
                    }
                    else throw new FileNotFoundException("File di configurazione JeepConfig.xml non trovato.");
                }
                return _configuration;
            }
        }
        #endregion

        #region [ _ctor ]
        public JeepService()
        {
            InitializeComponent();
        }
        #endregion

        #region [ Events ]

        protected override void OnStart(string[] args)
        {
            try
            {
                InitModules();

                _thread = new Thread(new JeepService().InitializeSpools);
                _thread.Start();

                var a = new MessageEventArgs("Il Servizio è stato avviato.");
                Tools.SendMessage(String.Format("JeepService v.{0}", ApplicationVersion), a);
            }
            catch (FileNotFoundException exception)
            {
                Tools.SendMessage("OnStart", new MessageEventArgs(exception.ToString()));

            }
        }

        protected override void OnStop()
        {
            FileLogger.Info(Logger, "STOP Servizio");
            _tokenSource.Cancel(false);
            // Ciclo su tutti i moduli per chiedere lo STOP
            foreach (var m in _modules)
            {
                FileLogger.Info(Logger, "Modulo " + m.Key + ": richiesta stop.");
                m.Value.Stop();
            }

            // Aspetto per vedere se tutti i moduli sono fermi
            var someWorking = false;
            CheckWorkingModules(ref someWorking);

            // Se ancora non si sono fermati i moduli invoco il Join
            if (someWorking)
            {
                foreach (var m in _modules)
                {
                    JeepModules[m.Value].Join();
                }
                CheckWorkingModules(ref someWorking);
            }

            // Se ancora non si sono fermati i moduli invoco l'Abort
            if (someWorking)
                foreach (var m in _modules)
                    JeepModules[m.Value].Abort();

            var a = new MessageEventArgs("Il Servizio è stato fermato.");
            Tools.SendMessage(String.Format("JeepService v.{0}", ApplicationVersion), a);
        }

        private void CheckWorkingModules(ref bool someWorking)
        {
            for (var i = 0; i < 10; i++)
            {
                foreach (var m in _modules.Values)
                {
                    someWorking = false;
                    if (m.IsBusy)
                    {
                        FileLogger.Info(Logger, string.Format("Modulo {0}: ancora attivo.", m.Name));
                        someWorking = true;
                    }
                    else
                    {
                        FileLogger.Info(Logger, string.Format("Modulo {0}: inattivo.", m.Name));
                    }
                }

                // Se nessun modulo è operativo allora posso fermare il servizio.
                if (!someWorking)
                {
                    FileLogger.Info(Logger, "Nessun modulo attivo.");
                    break;
                }

                // Eseguo il test 5 volte con pausa di un secondo tra uno e l'altro
                FileLogger.Info(Logger, "Attesa spegnimento 15 secondi.");
                Thread.Sleep(15000);
            }
        }

        #endregion

        #region [ Methods ]

        private void InitModules()
        {
            // Imposto il metodo che risolve dinamicamento gli Assembly non trovati: esegue una ricerca sulle cartelle dei vari Moduli.
            FileLogger.Info(Logger, "Moduli configurati: " + Cfg.Modules.Count);

            foreach (VecompSoftware.JeepService.Common.Module module in Cfg.Modules.Where(module => module.Enabled))
            {
                FileLogger.Info(Logger, "Caricamento modulo: " + module.Id);

                AppDomain moduleDomain = AppDomain.CreateDomain(module.Id);
                ThreadedModule threadedModule = (ThreadedModule)moduleDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ThreadedModule).FullName, false, BindingFlags.CreateInstance, null, new object[] { module }, null, null);

                Thread thread = new Thread(threadedModule.OnStart);
                FileLogger.Debug(Logger, "Modulo definito");
                JeepModules.Add(threadedModule, thread);
                _modules.Add(module.Id, threadedModule);

                FileLogger.Info(Logger, "Modulo caricato e configurato: " + module.Id);
            }
        }

        private void InitializeSpools()
        {
            try
            {
                FileLogger.Info(Logger, "Spool configurati: " + Cfg.Spools.Count);
                foreach (var spool in Cfg.Spools)
                {
                    var mySpool = new Spool(spool.Id);
                    FileLogger.Info(Logger, "Configurazione Spool: " + mySpool.Id);

                    // Inizializzo i timers dello spool
                    FileLogger.Info(Logger, "Timers configurati: " + spool.Timers.Count);
                    foreach (var timer in spool.Timers)
                    {
                        FileLogger.Debug(Logger, String.Format("Configurazione timer: {0} - {1}", timer.Id, timer.Type));

                        //Timer di riferimento
                        var tw = new TimerWork(timer.Type)
                        {
                            Id = timer.Id,
                            Duetime = timer.DueTime,
                            Period = timer.Period,
                            BeginTime = timer.BeginTime,
                            EndTime = timer.EndTime
                        };

                        var configuredModules = Cfg.Modules.Where(module => module.Spool == spool.Id && module.Timer == timer.Id && module.Enabled).ToList();
                        if (configuredModules.Count <= 0) continue;

                        var timerModules = configuredModules.Select(configuredModule => Modules[configuredModule.Id]).ToList();

                        // Inizializzo tutti i moduli del timer
                        foreach (var jeepThreadedModule in timerModules)
                        {
                            jeepThreadedModule.Timer = tw;
                            JeepModules[jeepThreadedModule].Start();
                        }
                    }

                    FileLogger.Debug(Logger, "Spool configurato correttamente: " + mySpool.Id);
                    Spools.Add(mySpool.Id, mySpool);
                }

                foreach (var s in Spools.Values)
                {
                    FileLogger.Debug(Logger, "Spool configurato: " + s.Id);
                    foreach (var timer in s.Timers.Values)
                    {
                        FileLogger.Debug(Logger, "TimerWork configurato: " + timer);
                    }
                }

                if (Spools.Count != 0) return;
                // Nessun modulo caricato
                FileLogger.Error(Logger, "Nessun modulo caricato");
                // Fermo il servizio
                Stop();
            }
            catch (Exception exc)
            {
                FileLogger.Error(Logger, "Errore in configurazione Spools", exc);
                Stop();
            }
        }

        /// <summary> Metodo per avvio Debug. </summary>
        public void Debug()
        {
            FileLogger.Info(Logger, "Avvio Servizio in modalità DEBUG");
            InitModules();
            InitializeSpools();
            var a = new MessageEventArgs("Avvio Servizio in modalità DEBUG");
            Tools.SendMessage(String.Format("JeepService v.{0}", ApplicationVersion), a);
        }

        #endregion
    }
}
