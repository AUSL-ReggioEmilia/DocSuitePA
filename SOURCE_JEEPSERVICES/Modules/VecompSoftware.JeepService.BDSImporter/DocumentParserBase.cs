using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BiblosDS.Library.Common.StampaConforme;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    public abstract class DocumentParserBase<T> : IDocumentParser
    {

        #region [ Constructors ]

        public DocumentParserBase(BDSImporterParameters parameters)
        {
            Parameters = parameters;
            InitializeFolders();
        }

        #endregion


        #region [ Fields ]

        private IList<string> _currentStack;

        private string _parserInputFolder;
        private string _parserOutputFolder;
        private string _parserBackupFolder;
        private string _parserErrorFolder;
        private string _parserRejectedFolder;

        private string _parserXsdPath;

        #endregion

        #region [ Properties ]

        // Common
        protected BDSImporterParameters Parameters { get; set; }
        protected string LoggerName
        {
            get { return typeof(T).Name; }
        }
        protected string ValidationLoggerName
        {
            get { return LoggerName + "Validation"; }
        }

        // Paths
        protected string ParserInputFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_parserInputFolder))
                    _parserInputFolder = Path.Combine(Parameters.InputFolder, typeof(T).Name);
                return _parserInputFolder;
            }
        }
        protected string ParserOutputFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_parserOutputFolder))
                    _parserOutputFolder = Path.Combine(Parameters.OutputFolder, typeof(T).Name);
                return _parserOutputFolder;
            }
        }
        protected string ParserBackupFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_parserBackupFolder))
                    _parserBackupFolder = Path.Combine(ParserOutputFolder, "Backup");
                return _parserBackupFolder;
            }
        }
        protected string ParserErrorFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_parserErrorFolder))
                    _parserErrorFolder = Path.Combine(ParserOutputFolder, "Error");
                return _parserErrorFolder;
            }
        }

        protected string ParserRejectedFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_parserRejectedFolder))
                    _parserRejectedFolder = Parameters.RejectedFolder;
                return _parserRejectedFolder;
            }
        }

        protected string ParserXsdPath
        {
            get
            {
                if (string.IsNullOrEmpty(_parserXsdPath))
                {
                    var path = Path.Combine(Parameters.XsdsFolder, typeof(T).Name + ".xsd");
                    if (!File.Exists(path))
                        throw new ArgumentNullException("Percorso non trovato: " + path);
                    _parserXsdPath = path;
                }
                return _parserXsdPath;
            }
        }

        // Stack
        private IList<string> CurrentStack
        {
            get
            {
                if (_currentStack == null)
                    _currentStack = GetXmls();
                return _currentStack;
            }
        }
        public int StackSize
        {
            get
            {
                if (CurrentStack.IsNullOrEmpty())
                    return 0;
                return CurrentStack.Count;
            }
        }

        #endregion


        #region [ Methods ]

        // Configuration
        private string CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                FileLogger.Warn(LoggerName, "Percorso non trovato: " + path);
                FileLogger.Warn(LoggerName, "Creazione in corso...");
                Directory.CreateDirectory(path);
                FileLogger.Warn(LoggerName, "Completata.");
            }
            return path;
        }
        private void InitializeFolders()
        {
            FileLogger.Info(LoggerName, "Inizio sessione di verifica struttura cartelle");
            CreateDirectory(ParserInputFolder);
            CreateDirectory(ParserOutputFolder);
            CreateDirectory(ParserBackupFolder);
            CreateDirectory(ParserErrorFolder);
            FileLogger.Info(LoggerName, "Fine sessione di verifica struttura cartelle.");
        }

        // Common
        private bool IsValidXml(string xmlPath, string xsdPath)
        {
            try
            {
                var schema = new XmlSchemaSet();
                schema.Add(null, new XmlTextReader(xsdPath));

                var settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas.Add(schema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

                using (var reader = XmlReader.Create(xmlPath, settings))
                {
                    while (reader.Read()) ;
                    reader.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("ERRORE IN VALIDAZIONE SCHEMA XML.")
                    .AppendLine(ex.Message)
                    .Append("XmlPath: ").AppendLine(xmlPath)
                    .Append("XsdPath: ").AppendLine(xsdPath);

                var renamed = string.Format("{0}_{1}.error", xmlPath, Guid.NewGuid().ToString("N"));
                sb.Append("Renamed XmlPath: ").AppendLine(renamed);

                FileLogger.Error(LoggerName, sb.ToString(), ex);
                FileLogger.Error(ValidationLoggerName, sb.ToString(), ex);

                sb.AppendLine()
                    .Append(ex.ToVerboseString());
                OnParsingError(sb.ToString());

                File.Move(xmlPath, renamed);
                FileLogger.Warn(LoggerName, "Rinominato in: " + renamed);

                return false;
            }
        }
        private T DeserializeXml(string xmlPath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
                return (T)serializer.Deserialize(fs);
        }
        private string GetFormattedValue(object source, PropertyInfo pi)
        {
            var value = pi.GetValue(source, null);
            if (value == null)
                return string.Empty;

            switch (pi.PropertyType.FullName)
            {
                case "System.DateTime":
                    return string.Format(Parameters.BiblosDateTimeFormat, (DateTime)value);
                default:
                    return value.ToString();
            }
        }
        protected virtual IDictionary<string, string> GetPropertiesDictionary(object source)
        {
            return source.GetType().GetProperties()
                .ToDictionary(pi => pi.Name, pi => GetFormattedValue(source, pi));
        }

        // Read
        protected virtual IList<string> GetXmls()
        {
            var found = Directory.GetFiles(ParserInputFolder, "*.xml");

            if (Parameters.MaxIterationPerCycle > 0)
                return found.Where(p => IsValidXml(p, ParserXsdPath))
                    .Take(Parameters.MaxIterationPerCycle).ToList();

            return found.Where(p => IsValidXml(p, ParserXsdPath))
                .ToList();
        }
        protected abstract IList<FileDocumentInfo> GetDocumentInfos(T deserialized);
        protected XmlDocument GetSignInfo(FileDocumentInfo document)
        {
            if (!document.Extension.Eq(".p7m"))
                throw new InvalidOperationException("Estensione non valida: " + document.Name);

            object p7mStream = document.Stream;
            var encodedInfo = new P7Mmanager().ExtractXmlSignatureInfo(ref p7mStream, 0, 0, false);
            var result = new XmlDocument();
            result.LoadXml(encodedInfo);
            return result;
        }

        // Write
        private string MoveToFolder(IList<FileDocumentInfo> documents, string prefix, string destination)
        {
            var sessionId = string.Format("{0}_{1}", prefix, Guid.NewGuid().ToString("N"));
            FileLogger.Info(LoggerName, "Inizio sessione di copia: " + sessionId);
            var sessionDestination = Path.Combine(destination, sessionId);
            FileLogger.Info(LoggerName, "Destinazione: " + sessionDestination);
            var dir = Directory.CreateDirectory(sessionDestination);
            foreach (var item in documents)
            {
                var copied = item.SaveToDisk(dir);
                FileLogger.Info(LoggerName, "Copiato: " + copied.FullName);
            }
            foreach (var item in documents)
            {
#if DEBUG
                FileLogger.Info(LoggerName, "#if DEBUG: item.FileInfo.Delete();");
#else
                item.FileInfo.Delete();
#endif
                FileLogger.Info(LoggerName, "Eliminato: " + item.FileInfo.FullName);
            }
            FileLogger.Info(LoggerName, "Fine sessione di copia: " + sessionId);
            return sessionDestination;
        }
        private void MoveToBackup(IList<FileDocumentInfo> documents, string prefix)
        {
            MoveToFolder(documents, prefix, ParserBackupFolder);
        }
        private void MoveToError(IList<FileDocumentInfo> documents, string prefix, Exception ex)
        {
            var destination = MoveToFolder(documents, prefix, ParserErrorFolder);
            if (ex == null)
            {
                var message = "Si è verificato un errore indefinito.{0}Documenti spostati in: {1}";
                message = string.Format(message, Environment.NewLine, destination);
                OnParsingError(message);
                return;
            }

            try
            {
                var verbose = ex.ToVerboseString();
                OnParsingError(verbose);

                var fileName = prefix + ".error.txt";
                var outputPath = Path.Combine(destination, fileName);
                using (var sw = new StreamWriter(outputPath))
                    sw.Write(verbose);
            }
            catch (Exception ex2)
            {
                FileLogger.Error(LoggerName, "Errore in ", ex2);
            }
        }
        private void MoveToError(FileDocumentInfo document, string prefix, Exception ex)
        {
            var list = new List<FileDocumentInfo> { document };
            MoveToError(list, prefix, ex);
        }

        private void MoveToRejected(IList<FileDocumentInfo> documents, string prefix, SignatureDocumentValidationException sdve)
        {
            string destination = MoveToFolder(documents, prefix, ParserRejectedFolder);
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(string.Concat("Sono avvenuti i seguenti errori nella validazione della firma digitale per il documento ", Path.GetFileName(sdve.FilePath), ":"));
                foreach (string error in sdve.Errors)
                {
                    builder.AppendLine(string.Concat("\u2022 ", error));
                }

                string fileName = string.Concat(prefix, ".error.txt");
                string outputPath = Path.Combine(destination, fileName);
                File.WriteAllText(outputPath, builder.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Error(LoggerName, "Si è verificato un errore in creazione del file con le specifiche di errata validazione firma", ex);
            }
        }

        protected abstract void SaveToBiblos(IList<FileDocumentInfo> documents, T deserialized);

        // Dismiss
        private bool IsExpired(FileInfo document, int threshold)
        {
            var elapsed = (DateTime.Today.Date - document.CreationTime.Date).TotalDays;
            return elapsed > Math.Abs(threshold);
        }
        private bool IsExpired(DirectoryInfo folder, int threshold)
        {
            var elapsed = (DateTime.Today.Date - folder.CreationTime.Date).TotalDays;
            return elapsed > Math.Abs(threshold);
        }
        public virtual void ImportDocuments()
        {
            InitializeFolders();
            var xmlPath = CurrentStack.First();
            FileLogger.Info(LoggerName, "Importazione documenti per: " + xmlPath);
            var xmlInfo = new FileDocumentInfo(new FileInfo(xmlPath));
            try
            {
                var deserialized = DeserializeXml(xmlPath);
                FileLogger.Info(LoggerName, "Deserializzato in: " + deserialized.GetType().ToString());

                var documents = GetDocumentInfos(deserialized);
                if (documents.IsNullOrEmpty())
                    throw new InvalidCastException("Nessun documento trovato per: " + xmlInfo.Name);
                FileLogger.Info(LoggerName, "Documenti trovati: " + documents.Count.ToString());
                SaveToBiblos(documents, deserialized);
                FileLogger.Info(LoggerName, "Salvataggio in Biblos completato.");
                var disposable = documents;
                disposable.Add(xmlInfo);
                MoveToBackup(disposable, Path.GetFileNameWithoutExtension(xmlInfo.Name));
                FileLogger.Info(LoggerName, "Importazione documenti completata.");
            }
            catch (FileNotFoundException fnf)
            {
                FileLogger.Warn(LoggerName, "Uno o più documenti mancanti o non validi.", fnf);
                if (!IsExpired(xmlInfo.FileInfo, Parameters.PostponeMissingExpires))
                    return;

                var message = string.Format("Documenti mancanti per {0} del {1} da più di {2} giorni.", xmlInfo.Name, xmlInfo.FileInfo.CreationTime, Parameters.PostponeMissingExpires);
                var expired = new Exception(message, fnf);
                FileLogger.Error(LoggerName, "Importazione documenti interrotta.", expired);
                MoveToError(xmlInfo, Path.GetFileNameWithoutExtension(xmlInfo.Name), expired);
            }
            catch (EndpointNotFoundException enf)
            {
                var temp = new Exception("Servizio Biblos non disponibile.", enf);
                FileLogger.Warn(LoggerName, temp.Message, temp);
                OnParsingError(temp.ToVerboseString());
            }
            catch (SignatureDocumentValidationException sdve)
            {
                FileLogger.Error(LoggerName, "Importazione documenti interrotta.", sdve);
                if (sdve.ToRetry)
                {
                    FileLogger.Warn(LoggerName, string.Format(@"Il server di validazione firme ha restituito un errore temporaneo per il documento {0}. 
                                                                    Il documento verrà lasciato nella directory corrente per provare nuovamente l'importazione.", xmlInfo.Name));
                    return;
                }
                FileDocumentInfo document = new FileDocumentInfo(new FileInfo(sdve.FilePath));
                IList<FileDocumentInfo> documents = new List<FileDocumentInfo>()
                {
                    document,
                    xmlInfo
                };      
                MoveToRejected(documents, Path.GetFileNameWithoutExtension(xmlInfo.Name), sdve);
            }
            catch (Exception ex)
            {
                FileLogger.Error(LoggerName, "Importazione documenti interrotta.", ex);
                MoveToError(xmlInfo, Path.GetFileNameWithoutExtension(xmlInfo.Name), ex);
            }
            finally
            {
                FileLogger.Info(LoggerName, "Rimozione dallo stack: " + xmlPath);
                CurrentStack.Remove(xmlPath);
            }
        }
        public virtual void DisposeLocalCopies()
        {
            FileLogger.Info(LoggerName, "Inizio dismissione copie di backup scadute.");

            var copies = Directory.GetDirectories(ParserBackupFolder)
                .Select(d => new DirectoryInfo(d))
                .ToList();
            FileLogger.Info(LoggerName, "Copie trovate: " + copies.Count.ToString());

            var expired = copies
                .Where(d => IsExpired(d, Parameters.LocalCopyExpires))
                .ToList();
            FileLogger.Info(LoggerName, "Di cui scadute: " + expired.Count.ToString());

            foreach (var item in expired)
            {
                var message = string.Format("Eliminazione copia del {0}: {1}", item.CreationTime, item.FullName);
                FileLogger.Debug(LoggerName, message);
#if DEBUG
                FileLogger.Warn(LoggerName, "#if DEBUG: eliminazione non effettuata.");
#else
                item.Delete(true);
#endif
                FileLogger.Debug(LoggerName, "Eliminazione completata.");
            }

            FileLogger.Info(LoggerName, "Dismissione copie di backup scadute completata.");
        }

        #endregion

        #region [ Events ]

        private void ValidationHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning:
                case XmlSeverityType.Error:
                    throw new XmlSchemaValidationException("Si è verificato un errore di validazione.", e.Exception);
            }
        }

        public delegate void ParsingErrorEventHandler(object sender, DocumentParserEventArgs args);
        public event ParsingErrorEventHandler OnParsingErrorEventHandler;
        public void OnParsingError(string message)
        {
            if (OnParsingErrorEventHandler == null)
                return;
            var args = new DocumentParserEventArgs(message);
            OnParsingErrorEventHandler(this, args);
        }

        #endregion

    }
}
