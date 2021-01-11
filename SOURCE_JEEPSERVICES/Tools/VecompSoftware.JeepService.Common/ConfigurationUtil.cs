using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Serialization;
using VecompSoftware.Helpers;


namespace VecompSoftware.JeepService.Common
{
    #region [ Utilities ]
    /// <summary>
    /// Utilità per lettura e scrittura del file di configurazione
    /// </summary>
    public static class ConfigurationUtil
    {
        /// <summary>
        /// Salva la configurazione in un file
        /// </summary>
        public static void Save(Configuration configuration, string fileName)
        {
            var ser = new XmlSerializer(typeof(Configuration), new XmlRootAttribute { ElementName = "CONFIG", Namespace = "" });
            // Elimino il namespace
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                // Salvo in utf-8
                var xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8) { Formatting = Formatting.Indented };
                ser.Serialize(xmlTextWriter, configuration, ns);
            }
        }

        /// <summary>
        /// Carica la configurazione da un file
        /// </summary>
        public static Configuration Load(string fileName)
        {
            var ser = new XmlSerializer(typeof(Configuration));
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                return (Configuration)ser.Deserialize(fileStream);
            }
        }

        public static Dictionary<string, string> ModuleVersions;
    }
    #endregion

    #region [ Classi di definizione dell'XML ]
    /// <summary>
    /// Classe per gestire il file di configurazione
    /// </summary>
    [XmlRoot(ElementName = "CONFIG")]
    public class Configuration : InfiniteMarshalByRefObject
    {
        [XmlArray("MODULES"), XmlArrayItem("MODULE")]
        public List<Module> Modules { get; set; }

        [XmlArray("SPOOLS"), XmlArrayItem("SPOOL")]
        public List<SpoolXml> Spools { get; set; }

        public new string ToString()
        {
            string temp;
            using (StringWriter output = new SerializationHelper.Utf8StringWriter())
            {
                // Salvo in utf-8
                var writer = XmlWriter.Create(output, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true });
                var ser = new XmlSerializer(typeof(Configuration), new XmlRootAttribute { ElementName = "CONFIG", Namespace = "" });
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                ser.Serialize(writer, this, ns);
                temp = output.ToString();
            }
            return temp;
        }
    }

    /// <summary>
    /// Classe di configurazione del modulo
    /// </summary>
    [XmlRoot(ElementName = "MODULE", Namespace = "")]
    public class Module : InfiniteMarshalByRefObject
    {
        [XmlAttribute("id")]
        [Description("Identificativo dell'istanza del modulo.")]
        public string Id { get; set; }

        /// <summary>ID dello spool</summary>
        [XmlAttribute("spool")]
        [Description("Identificativo dello Spool.")]
        public string Spool { get; set; }

        /// <summary>ID del timer</summary>
        [XmlAttribute("timer")]
        [Description("Identificativo del Timer che si occupa di eseguire il modulo.")]
        public string Timer { get; set; }

        /// <summary>Percorso completo dell'assembly da caricare</summary>
        [XmlAttribute("assembly")]
        [Description("Assembly di riferimento.")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Assembly { get; set; }

        private string _assemblyPath;
        public string FullAssemblyPath
        {
            get
            {
                if (String.IsNullOrEmpty(_assemblyPath))
                {
                    var modulesPath = ConfigurationManager.AppSettings["ModulesFolder"];
                    _assemblyPath = !String.IsNullOrEmpty(modulesPath) ? Path.Combine(modulesPath, Assembly) : Assembly;
                }
                return _assemblyPath;
            }
        }

        /// <summary>Nome completo della classe da caricare</summary>
        [XmlAttribute("class")]
        [Description("Nome della classe che implementa IJeepModule.")]
        public string Class { get; set; }

        [XmlAttribute("parametersType")]
        [Description("Nome della classe che implementa IJeepParameter.")]
        public string ParametersType { get; set; }

        /// <summary>Nome completo della classe da caricare</summary>
        [XmlAttribute("enabled")]
        [Description("Indica se il modulo debba essere attivo.")]
        public bool Enabled { get; set; }

        [XmlArray("PARAMETERS"), XmlArrayItem("PARAMETER")]
        [Description("Insieme dei parametri di configurazione.")]
        [Browsable(false)]
        public List<Parameter> Parameters { get; set; }

        [Description("Versione del modulo")]
        public string Version
        {
            get
            {
                if (ConfigurationUtil.ModuleVersions != null && ConfigurationUtil.ModuleVersions.ContainsKey(Id))
                {
                    return ConfigurationUtil.ModuleVersions[Id];
                }
                if (new FileInfo(FullAssemblyPath).Exists)
                {
                    var moduleAssembly = System.Reflection.Assembly.LoadFile(FullAssemblyPath);
                    var moduleAssemblyFileVersioneInfo = FileVersionInfo.GetVersionInfo(moduleAssembly.Location);
                    return moduleAssemblyFileVersioneInfo.FileVersion;
                }
                return "0.0.0.0";
            }
        }

        [XmlAttribute("EnableMainLog")]
        [Category("Logging")]
        [Description("Definisce se deve essere attivato il log del modulo")]
        [DefaultValue(true)]
        public bool EnableMainLog { get; set; }

        [XmlAttribute("MainLogLevel")]
        [Category("Logging")]
        [Description("Definisce il livello del log principale del modulo")]
        [DefaultValue(LogLevel.Info)]
        public LogLevel MainLogLevel { get; set; }

        [XmlAttribute("EnableThreadLog")]
        [Category("Logging")]
        [Description("Definisce se deve essere attivato il log del thread del modulo")]
        [DefaultValue(false)]
        public bool EnableThreadLog { get; set; }

        [XmlAttribute("ThreadLogLevel")]
        [Category("Logging")]
        [Description("Definisce il livello del log del thread del modulo")]
        [DefaultValue(LogLevel.All)]
        public LogLevel ThreadLogLevel { get; set; }

        [XmlAttribute("MergeLogs")]
        [Category("Logging")]
        [Description("Definisce se il log del thread deve essere mescolato con quello del modulo principale")]
        [DefaultValue(false)]
        public bool MergeLogs { get; set; }

    }

    /// <summary>
    /// Classe dei parametri di un modulo
    /// </summary>
    [XmlRoot("PARAMETER")]
    public class Parameter : InfiniteMarshalByRefObject
    {
        /// <summary>ID dello spool</summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <summary>ID dello spool</summary>
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Classe di configurazione di uno spool
    /// </summary>
    [XmlRoot("SPOOL")]
    public class SpoolXml : InfiniteMarshalByRefObject
    {
        [XmlAttribute("id")]
        [Description("Identificativo dello Spool.")]
        public string Id { get; set; }

        [XmlArray("TIMERS"), XmlArrayItem("TIMER")]
        [Browsable(false)]
        public List<Timer> Timers { get; set; }
    }

    /// <summary>
    /// Classe di configurazione dei timer degli spool
    /// </summary>
    [XmlRoot("TIMER")]
    public class Timer
    {
        [XmlAttribute("id")]
        [Description("Identificativo del Timer")]
        public string Id { get; set; }

        [XmlAttribute("duetime")]
        [Description("Millisecondi di attesa per l'avvio del Timer.")]
        public int DueTime { get; set; }

        [XmlAttribute("period")]
        [Description("Millisecondi tra una esecuzione e l'altra.")]
        public int Period { get; set; }

        [XmlAttribute]
        [Description("Orario di inizio in formato HH:MM.")]
        public string BeginTime { get; set; }

        [XmlAttribute]
        [Description("Orario di fine in formato HH:MM.")]
        public string EndTime { get; set; }

        [XmlAttribute("type")]
        [Description("Tipologia del Timer.")]
        public TimerType Type { get; set; }
    }
    #endregion
}
