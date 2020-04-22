using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using JeepService.Classes;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;
using Module = VecompSoftware.JeepService.Common.Module;
using Timer = System.Threading.Timer;
using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace JeepService
{
    /// <summary>
    /// Classe che si occupa di gestire un modulo JeepService in un contesto separato dagli altri moduli
    /// </summary>
    public class ThreadedModule : InfiniteMarshalByRefObject
    {
        #region "Private Properties"
        /// <summary>
        /// Insieme dei Timer in funzione. 
        /// </summary>
        /// <remarks>La variabile deve essere statica per tenere attivo il servizio che, altrimenti, si fermerebbe. Credo.</remarks>
        private static readonly Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        /// <summary>
        /// Contiene il modulo JeepService istanziato
        /// </summary>
        private IJeepModule JeepModule { get; set; }

        /// <summary>
        /// Definisce se il modulo è stato già inizializzato
        /// </summary>
        private bool ModuleInitialized { get; set; }

        #endregion

        #region "Public Properties"

        /// <summary>
        /// Contiene la configurazione del modulo da eseguire
        /// </summary>
        public Module ConfigurationModule { get; set; }

        public string Name
        {
            get { return ConfigurationModule.Id; }
        }

        public string ThreadedModuleLogger
        {
            get { return String.Format("{0}_ThreadedModule", Name); }
        }

        /// <summary>
        /// Inoltra al modulo il comando di Stop
        /// </summary>
        private bool Cancel
        {
            set { if (JeepModule != null) JeepModule.Cancel = value; }
        }

        /// <summary>
        /// Richiede al modulo lo stato di attività
        /// </summary>
        public bool IsBusy
        {
            get { return JeepModule == null || (ModuleInitialized && JeepModule.IsBusy); }
        }

        /// <summary>
        /// Abilita/Disabilita il modulo
        /// </summary>
        public bool Enabled
        {
            get
            {
                //FileLogger.Debug(ThreadedModuleLogger,
                //    String.Format("Enabled status: JeepModule->{0} - ModuleInitialized->{1}{2}", JeepModule,
                //        ModuleInitialized,
                //        JeepModule != null
                //            ? String.Format(" - JeepModule.Enabled->{0}", JeepModule.Enabled)
                //            : String.Empty));
                return JeepModule != null && JeepModule.Enabled && ModuleInitialized;
            }
            set
            {
                //FileLogger.Debug(ThreadedModuleLogger, String.Format("Ricevuto Enabled a {0}", value));
                JeepModule.Enabled = value;
            }
        }

        /// <summary>
        /// Ritorna l'ultima esecuzione del modulo (se attivo)
        /// </summary>
        public DateTime? LastExecution
        {
            get { return JeepModule != null ? JeepModule.LastExecution : null; }
        }

        /// <summary>
        /// Ritorna la prossima esecuzione del modulo (se attivo)
        /// </summary>
        public DateTime? NextExecution
        {
            get { return JeepModule != null ? JeepModule.NextExecution : null; }
            set { if (JeepModule != null) JeepModule.NextExecution = value; }
        }

        /// <summary>
        /// Timer che gestisce l'esecuzione del modulo
        /// </summary>
        public TimerWork Timer { get; set; }

        #endregion

        #region "Constructor"

        /// <summary>
        /// Costruttore della classe
        /// </summary>
        /// <param name="configurationModule">Il modulo che deve essere istanziato ed eseguito</param>
        public ThreadedModule(Module configurationModule)
        {
            ConfigurationModule = configurationModule;
            InitializeLogger();
            ModuleInitialized = false;
        }

        private void InitializeLogger()
        {
            // Aggiungo il logger, se definita la modalità Auto
            bool result = false;
            if (bool.TryParse(ConfigurationManager.AppSettings["log4net"], out result) && result &&
                bool.TryParse(ConfigurationManager.AppSettings["AutoLog"], out result) && result)
            {
                // Aggiungo il Log del modulo
                var mainAppender = DynamicLoggerConfigurator.AddAppender(ConfigurationModule.Id);
                DynamicLoggerConfigurator.AddLogger(ConfigurationModule.Id, mainAppender, ConfigurationModule.MainLogLevel.ToLevel());

                // Aggiungo il log Threaded
                if (ConfigurationModule.EnableThreadLog)
                {
                    if (ConfigurationModule.MergeLogs)
                    {
                        DynamicLoggerConfigurator.AddLogger(ThreadedModuleLogger, mainAppender,
                            ConfigurationModule.ThreadLogLevel.ToLevel());
                    }
                    else
                    {
                        DynamicLoggerConfigurator.AddCompleteLogger(ThreadedModuleLogger, ConfigurationModule.ThreadLogLevel.ToLevel());
                    }
                }

                DynamicLoggerConfigurator.Configured();
            }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Metodo che viene eseguito all'attivazione del Thread
        /// </summary>
        public void OnStart()
        {
            // Verifica che il modulo sia stato inizializzato
            ModuleInitialization();
        }

        /// <summary>
        /// Metodo richiamato dal timer principale
        /// </summary>
        private void DoWork()
        {
            // Esecuzione del DoWork del modulo
            JeepModule.DoWork();
        }

        /// <summary>
        /// Metodo richiamato dal servizio in fase di spegnimento
        /// </summary>
        public void Stop()
        {
            FileLogger.Debug(Name, String.Format("Richiesta di Stop ricevuta da AppDomain {0}", AppDomain.CurrentDomain.FriendlyName));
            Cancel = true;
            if (Timers.ContainsKey(Name)) Timers[Name].Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region "Private methods"

        /// <summary>
        /// Effettua l'inizializzazione del modulo
        /// </summary>
        private void ModuleInitialization()
        {
            if (ModuleInitialized) return;

            try
            {
                // Imposto il resolver
                FileLogger.Debug(Name,
                    String.Format("Inizializzazione Thread su AppDomain {0}", AppDomain.CurrentDomain.FriendlyName));
                AppDomain.CurrentDomain.AssemblyResolve +=
                    (s, a) => OnCurrentDomainAssemblyResolve(a, ConfigurationModule);

                // Creo la classe principale del modulo (tutte le successive richieste di dll saranno gestite dall'apposito metodo)
                JeepModule = Tools.ModuleBuilder(ConfigurationModule.FullAssemblyPath, ConfigurationModule.Class,
                    ConfigurationModule.Parameters, ConfigurationModule.Id);

                String useProfiler = ConfigurationManager.AppSettings["NHProfiler"];
                bool profiler;
                if (!string.IsNullOrEmpty(useProfiler) && bool.TryParse(useProfiler, out profiler) && profiler)
                {
                    NHibernateProfiler.Initialize();
                    FileLogger.Debug(Name, String.Format("Profiler agganciato al modulo {0}.", Name));
                }

                // Inizializzo il modulo
                FileLogger.Info(Name, String.Format("Inizializzazione modulo {0} in corso....", Name));
                JeepModule.Initialize(ConfigurationModule.Parameters);

                ModuleInitialized = true;
                FileLogger.Info(Name, String.Format("Modulo {0} correttamente inizializzato.", Name));

                // Genero e attivo il timer di riferimento
                Timers.Add(Name, new Timer(OnTimerCallback, Timer, Timer.Duetime, Timer.Period));
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name,
                    String.Format("Rilevato errore su AppDomain {0} - Stacktrace: {1}", AppDomain.CurrentDomain.FriendlyName, ex.StackTrace), ex);
                Stop();
            }
        }

        /// <summary>
        /// Effettua la ricerca delle dll richieste dal modulo corrente andando a cercare nella cartella del modulo stesso
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configurationModule"></param>
        /// <returns></returns>
        private Assembly OnCurrentDomainAssemblyResolve(ResolveEventArgs args, Module configurationModule)
        {
            try
            {
                // Cerco prima nelle Assembly già caricate dal Domain
                var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var t in currentAssemblies.Where(t => t.FullName == args.Name))
                {
                    return t;
                }

                // Se non trovo allora cerco nelle directory
                return AssemblyResolver(args, new DirectoryInfo(configurationModule.FullAssemblyPath).Parent);
            }
            catch (Exception ex)
            {
                Enabled = false;
                Tools.SendMessage(String.Format("{0} v.{1}", Name, JeepModule.Version), new MessageEventArgs(String.Format("Errore in fase di recupero dll: {0}", ex.Message)));
            }
            return null;
        }

        /// <summary>
        /// Effettua l'effettiva ricerca nelle directory
        /// </summary>
        /// <param name="args"></param>
        /// <param name="moduleDirectoryInfo"></param>
        /// <returns></returns>
        private Assembly AssemblyResolver(ResolveEventArgs args, DirectoryInfo moduleDirectoryInfo)
        {
            FileLogger.Info(ThreadedModuleLogger, String.Format("Richiesta libreria: \"{0}\" da \"{1}\"", args.Name, args.RequestingAssembly));
            if (args.Name.Equals("zx_42cfdad6df8d4258a4536980da7652aa, PublicKeyToken=6dc438ab78a525b3"))
                return null;

            if (args.Name.Equals("zx_8dad43749da44515aa214f53cdd06393, PublicKeyToken=6dc438ab78a525b3"))
                return null;

            if (moduleDirectoryInfo != null && moduleDirectoryInfo.Exists)
            {
                return FindAssemblyInDirectory(args.Name, moduleDirectoryInfo);
            }

            throw new Exception(String.Format("Directory del modulo errata: {0}", moduleDirectoryInfo));
        }

        /// <summary>
        /// Effettua il match nella directory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private Assembly FindAssemblyInDirectory(string name, DirectoryInfo dir)
        {
            //FileLogger.Debug(Logger, string.Format("Cerco la libreria {0} nella directory {1}", name, dir.FullName));
            var searchingAssembly = new AssemblyName(name);
            // Provo a derivare il nome dell'assembly e a cercarla direttamente
            var dll = new FileInfo(Path.Combine(dir.FullName, String.Format("{0}.dll", searchingAssembly.Name)));
            var foundAssembly = GetAssemblyFromFile(dll, searchingAssembly, true);
            // Se la trovo la utilizzo, altrimenti cerco fra tutte le dll
            if (foundAssembly != null) return foundAssembly;

            foreach (var file in dir.GetFiles("*.dll"))
            {
                try
                {
                    var assemblyFound = GetAssemblyFromFile(file, searchingAssembly, false);
                    // Se ho trovato l'assembly la restituisco, altrimenti proseguo
                    if (assemblyFound != null) return assemblyFound;
                }
                catch (Exception ex)
                {
                    FileLogger.Warn(ThreadedModuleLogger, string.Format("Errore in tentativo di caricamento {0}", file.Name), ex);
                }
            }
            return null;
        }

        private Assembly GetAssemblyFromFile(FileSystemInfo file, AssemblyName searchingAssembly, bool loadFrom)
        {
            if (!file.Exists) return null;
            var myAssembly = loadFrom ? Assembly.LoadFrom(file.FullName) : Assembly.LoadFile(file.FullName);
            if ((searchingAssembly.Name != myAssembly.GetName().Name) ||
                (searchingAssembly.Version != null &&
                 searchingAssembly.Version.CompareTo(myAssembly.GetName().Version) > 0))
            {
                return null;
            }
            FileLogger.Debug(ThreadedModuleLogger, String.Format("Libreria {0} caricata dinamicamente da {1}", myAssembly.GetName().Name, file.FullName));
            return myAssembly;
        }

        /// <summary>
        /// Metodo richiamato ad ogni tic del Timer
        /// </summary>
        /// <param name="state">TimerWork di riferimento</param>
        private void OnTimerCallback(object state)
        {
            // Elenco dei moduli da eseguire
            var tw = (TimerWork)state;

            // Loggo lo status del Timer
            FileLogger.Debug(ThreadedModuleLogger, string.Format("TimerCallback: {0} -> IsBusy: {1} | Enabled: {2}", tw, IsBusy, Enabled));

            if (IsBusy || !Enabled) return;

            try
            {
                FileLogger.Info(Name, string.Format("TimerCallback: {0} -> IsBusy: {1} | Enabled: {2}", tw, IsBusy, Enabled));
                var signal = DateTime.Now;
                switch (tw.Type)
                {
                    case TimerType.Single:
                        if (Tools.CheckSignalTime(signal, tw.BeginTime, LastExecution))
                        {
                            FileLogger.Debug(Name, string.Format("Avvio modulo: {0}", Name));
                            // Imposto il valore della prossima esecuzione 
                            NextExecution = Tools.GetNextExecution(signal, tw.BeginTime);
                            DoWork();
                        }
                        else
                        {
                            // Non eseguo il modulo, già eseguito
                            FileLogger.Debug(Name, String.Format("Modulo {0} già eseguito: {1}", Name, LastExecution.GetValueOrDefault(DateTime.MinValue).ToLongTimeString()));
                        }
                        break;
                    case TimerType.Multiple:
                        // Nessuna verifica
                        FileLogger.Debug(Name, String.Format("Avvio modulo: {0}", Name));
                        NextExecution = Tools.GetNextExecution(signal, tw.Period);
                        DoWork();
                        break;
                    case TimerType.Timerange:
                        if (Tools.CheckSignalTime(signal, tw.BeginTime, tw.EndTime))
                        {
                            FileLogger.Debug(Name, String.Format("Avvio modulo: {0}", Name));
                            NextExecution = Tools.GetNextExecution(signal, tw.BeginTime, tw.EndTime, tw.Period);
                            DoWork();
                        }
                        else
                        {
                            FileLogger.Debug(Name, String.Format("Modulo {0} non eseguito, fuori fascia oraria", Name));
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                // Errore in chiamata modulo
                FileLogger.Error(Name, "Errore in avvio modulo", exc);
                Enabled = false;
            }
        }
        #endregion
    }
}
