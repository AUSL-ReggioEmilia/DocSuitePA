using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService
{
    public class ProtocolJournalBuilder
    {
        #region [ Fields. ]
        private DateTime? _date;
        private string _templatePath;
        private string _outputPath;
        private NHibernateProtocolFinder _protocolFinder;
        private ProtocolJournalLogFacade _logFacade;
        private IList<Protocol> _protocols;
        private string _name;
        private string _tempName;
        private ProtocolJournalLog _journalLog;
        private ProtJournalPrint _journalPrint;
        #endregion

        #region [ Properties. ]

        public DateTime Date
        {
            get
            {
                return _date != null && _date != new DateTime() ? _date.Value : DateTime.Now;
            }
            set
            {
                DiscardBuilder();
                _date = value;
            }
        }
        public string TemplatePath
        {
            get
            {
                if (string.IsNullOrEmpty(_templatePath))
                    _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Report\RegistroProtocollo.rdlc");
                return _templatePath;
            }
            set { _templatePath = value; }
        }
        public string OutputPath
        {
            get
            {
                if (string.IsNullOrEmpty(_outputPath))
                {
                    _outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Registri");
                    if (!Directory.Exists(_outputPath))
                        Directory.CreateDirectory(_outputPath);
                }
                return _outputPath;
            }
            set
            { _outputPath = value; }
        }
        public Location Location { get; set; }
        public NHibernateProtocolFinder ProtocolFinder
        {
            get
            {
                return _protocolFinder ?? (_protocolFinder = new NHibernateProtocolFinder("ProtDB")
                                                                 {
                                                                     EnablePaging = false,
                                                                     NoStatus = true,
                                                                     RegistrationDateFrom = Date,
                                                                     RegistrationDateTo = Date,
                                                                     HasJournalLog = false,
                                                                     LoadFetchModeFascicleEnabled = false,
                                                                     LoadFetchModeProtocolLogs = false,
                                                                     ProtocolStatusCancel = NHibernateBaseFinder<Protocol, ProtocolHeader>.StatusSearchType.EvenStatusCancel
                                                                 });
            }
        }
        private ProtocolJournalLogFacade LogFacade
        {
            get { return _logFacade ?? (_logFacade = new ProtocolJournalLogFacade()); }
        }
        public bool ApplySign { get; set; }
        public string CertificateName { get; set; }
        public bool ApplyTimeStamp { get; set; }
        public int InfoCamereFormat { get; set; }
        public string ChainObjectDateFormat { get; set; }

        public IList<Protocol> Protocols
        {
            get { return _protocols ?? (_protocols = ProtocolFinder.DoSearch()); }
        }
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = string.Format("{0:yyyy-MM-dd}-RegistroProtocollo.pdf", Date);
                return _name;
            }
            set { _name = value; }
        }

        // Nome dell'appender
        public string LoggerName { get; set; }

        public string TempName
        {
            get
            {
                if (string.IsNullOrEmpty(_tempName))
                    _tempName = string.Format("{0:yyyy-MM-dd-HHmmss}-{1}-RegistroProtocollo.pdf", Date, Guid.NewGuid().ToString("N"));
                return _tempName;
            }
        }
        private ProtocolJournalLog JournalLog
        {
            get
            {
                return _journalLog ?? (_journalLog = new ProtocolJournalLog
                                                         {
                                                             StartDate = DateTime.Now,
                                                             EndDate = null,
                                                             Location = Location,
                                                             LogDescription = string.Empty,
                                                             LogDate = DateTime.Now,
                                                             ProtocolJournalDate = Date,
                                                             ProtocolActive = 0,
                                                             ProtocolCancelled = 0,
                                                             ProtocolRegister = 0,
                                                             ProtocolError = 0,
                                                             ProtocolOthers = 0,
                                                             ProtocolTotal = 0
                                                         });
            }
            set
            {
                _journalPrint = null; // discard di JournalPrint.
                _journalLog = value;
            }
        }
        private ProtJournalPrint JournalPrint
        {
            get
            {
                if (_journalPrint == null)
                {
                    _journalPrint = new ProtJournalPrint
                                        {
                                            JournalLog = JournalLog,
                                            RdlcPrint = TemplatePath,
                                            Protocols = Protocols,
                                            Emergenza = HasSuspendedProtocols()
                                        };
                    _journalPrint.DoPrint();
                }
                return _journalPrint;
            }
        }

        private TempFileDocumentInfo GetDocument(string source, string name, string archiveName)
        {
            TempFileDocumentInfo documentObject = new TempFileDocumentInfo(new FileInfo(source)) { Caption = name, Name = name };
            documentObject.Attributes.Add("Filename", name);
            documentObject.Attributes.Add("Signature", "Registro Giornaliero di Protocollo");
            documentObject.Attributes.Add("ANumero", JournalPrint.ANumero.ToString(CultureInfo.InvariantCulture));
            documentObject.Attributes.Add("DaNumero", JournalPrint.DaNumero.ToString(CultureInfo.InvariantCulture));
            string dataAttribute = Date.ToString(CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(ChainObjectDateFormat))
                dataAttribute = Date.ToString(ChainObjectDateFormat);
            documentObject.Attributes.Add("Data", dataAttribute);
            documentObject.Attributes.Add("Tipologia", "REG_PROTOCOLLO");
            documentObject.Attributes.Add("Anno", Date.Year.ToString());

            DateTimeOffset minDateTimeOffset = Protocols.Min(x => x.RegistrationDate).ToLocalTime();
            DateTimeOffset maxDateTimeOffeset = Protocols.Max(x => x.RegistrationDate).ToLocalTime();

            string daData = minDateTimeOffset.ToString(CultureInfo.InvariantCulture);
            string aData = maxDateTimeOffeset.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(ChainObjectDateFormat))
                daData = minDateTimeOffset.ToString(ChainObjectDateFormat);
                aData = maxDateTimeOffeset.ToString(ChainObjectDateFormat);

            documentObject.Attributes.Add("DaData", daData);
            documentObject.Attributes.Add("AData", aData);
            
            if (minDateTimeOffset.Date == maxDateTimeOffeset.Date)
                documentObject.Attributes.Add("Oggetto", string.Concat("Registro giornaliero di protocollo dal n. ", JournalPrint.DaNumero.ToString(CultureInfo.InvariantCulture), " al n. ", JournalPrint.ANumero.ToString(CultureInfo.InvariantCulture), " del ", minDateTimeOffset.Date.ToString("dd/MM/yyyy")));
            else
                documentObject.Attributes.Add("Oggetto", string.Concat("Registro giornaliero di protocollo dal n. ", JournalPrint.DaNumero.ToString(CultureInfo.InvariantCulture), " al n. ", JournalPrint.ANumero.ToString(CultureInfo.InvariantCulture), " dal ", minDateTimeOffset.Date.ToString("dd/MM/yyyy"), " al ", maxDateTimeOffeset.Date.ToString("dd/MM/yyyy")));

            documentObject.Attributes.Add("DataInserimentoDocumento", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            documentObject.Attributes.Add("NRegistrazioni", JournalPrint.JournalLog.ProtocolTotal.ToString());            
            documentObject.Attributes.Add("NAnnullati", JournalPrint.JournalLog.ProtocolCancelled.ToString());
            documentObject.Attributes.Add("NomeDocumento", name);
            documentObject.Attributes.Add("TipoDocumento", "DOCUMENTO");
            documentObject.Attributes.Add("Tipo", "DOCUMENTO");
            documentObject.Attributes.Add("ProgressivoDocumento", 1.ToString());
            documentObject.Attributes.Add("Riferimenti", "MD");
            documentObject.Attributes.Add("Numero", JournalLog.Id.ToString());
            documentObject.Attributes.Add("NomeArchivio", archiveName);

            FileLogger.Debug(LoggerName, string.Format("Valore della data da inserire in Biblos {0}", dataAttribute));

            return documentObject;
        }
        #endregion

        #region [ Methods. ]

        /// <summary>
        /// Reinizializza i singleton.
        /// </summary>
        private void DiscardBuilder()
        {
            _protocolFinder = null;
            _protocols = null;
            _journalLog = null;
            _journalPrint = null;
            _name = null;
            _tempName = null;
            //_chainObject = null;
        }
        /// <summary>
        /// Verifica se ci sono protocolli sospesi per creare il registro in regime di emergenza.
        /// </summary>
        /// <returns></returns>
        private static bool HasSuspendedProtocols()
        {
            try
            {
                var finder = new NHibernateProtocolFinder("ProtDB") 
                { 
                    IdStatus = -3,
                    LoadFetchModeFascicleEnabled = false,
                    LoadFetchModeProtocolLogs = false
                };
                var suspended = finder.DoSearch();
                return suspended.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("[HasSuspendedProtocols]", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Build()
        {
            var log = new ProtocolJournalLog
                          {
                              Location = JournalLog.Location,
                              LogDescription = JournalLog.LogDescription,
                              ProtocolActive = JournalLog.ProtocolActive,
                              ProtocolCancelled = JournalLog.ProtocolCancelled,
                              ProtocolRegister = JournalLog.ProtocolRegister,
                              ProtocolError = JournalLog.ProtocolError,
                              ProtocolOthers = JournalLog.ProtocolOthers,
                              ProtocolTotal = JournalLog.ProtocolTotal,
                              LogDate = JournalLog.LogDate,
                              ProtocolJournalDate = JournalLog.ProtocolJournalDate,
                              StartDate = JournalLog.StartDate,
                              EndDate = null
                          };
            FileLogger.Debug(LoggerName, "Salvo l'oggetto JournalLog:\n" + JournalLogToString(log));
            LogFacade.Save(ref log);
            JournalLog = log;

            FileLogger.Debug(LoggerName, "Cerco i Protocolli da inserire...");
            if (Protocols.Count > 0)
            {
                FileLogger.Debug(LoggerName, String.Format("{0} Protocolli da registrare.", Protocols.Count));
                
                // Scrivo il registro su disco.
                var destination = Path.Combine(OutputPath, TempName);
                FileLogger.Debug(LoggerName, "File di destinazione: " + destination);

                try
                {
                    //Salvo il pdf del registro
                    FileLogger.Debug(LoggerName, "Generazione report RDLC.");
                    JournalPrint.Report.DoPrint().SavePdf(new DirectoryInfo(OutputPath), TempName, String.Empty);
                    FileLogger.Debug(LoggerName, "Creazione registro avvenuta con successo: " + destination);

                }
                catch (Exception ex)
                {
                    JournalLog.LogDescription = "Errore in fase di Creazione PDF";
                    throw new Exception("Errore in fase di Creazione PDF", ex);
                }

                // Eseguo firma e marcatura temporale del registro (se richiesto).
                if (ApplySign)
                {
                    try
                    {
                        destination = ProtocolJournalTools.SignEngine.SignDocument(destination);
                        FileLogger.Debug(LoggerName, "Firma eseguita con successo: " + destination);
                        Name = Name + ".p7m";
                    }
                    catch (Exception ex)
                    {
                        JournalLog.LogDescription = "Errore in fase di firma Registro";
                        throw new Exception("Errore in fase di firma Registro.", ex);
                    }
                }

                // Eseguo firma e marcatura temporale del registro (se richiesto).
                if (ApplyTimeStamp)
                {
                    try
                    {
                        destination = ProtocolJournalTools.SignEngine.TimeStampDocument(destination);
                        FileLogger.Debug(LoggerName, "Marcatura temporale eseguita con successo: " + destination);
                        Name = Name + ".p7x";

                    }
                    catch (Exception ex)
                    {
                        JournalLog.LogDescription = "Errore in fase di marcatura temporale Registro";
                        throw new Exception("Errore in fase di marcatura temporale Registro.", ex);
                    }
                }

                try
                {
                    JournalLog.IdDocument = ProtocolJournalTools.SaveJournalToBiblos(Location,
                        GetDocument(destination, Name, Location.ProtBiblosDSDB));
                    FileLogger.Debug(LoggerName, "Documento salvato con successo: " + JournalLog.IdDocument);
                }
                catch (Exception ex)
                {
                    JournalLog.LogDescription = "Errore in fase di salvataggio Registro";
                    throw new Exception("Errore in fase di salvataggio Registro.", ex);
                }
            }
            else
            {
                FileLogger.Debug(LoggerName, "Nessun protocollo da registrare.");
            }

            JournalLog.EndDate = DateTime.Now;
            log = JournalLog;
            LogFacade.Update(ref log);
            JournalLog = log;
        }

        private static string JournalLogToString(ProtocolJournalLog log)
        {
            var builder = new StringBuilder();
            if (log.LogDate != null) builder.AppendFormat("LogDate: {0}\n", log.LogDate.Value.ToString(CultureInfo.InvariantCulture));
            if (log.ProtocolJournalDate != null) builder.AppendFormat("ProtocolJournalDate: {0}\n", log.ProtocolJournalDate.Value.ToString(CultureInfo.InvariantCulture));
            builder.AppendFormat("SystemComputer: {0}\n", log.SystemComputer);
            builder.AppendFormat("SystemUser: {0}\n", log.SystemUser);
            if (log.StartDate != null) builder.AppendFormat("StartDate: {0}\n", log.StartDate.Value.ToString(CultureInfo.InvariantCulture));
            if (log.EndDate != null) builder.AppendFormat("EndDate: {0}\n", log.EndDate.Value.ToString(CultureInfo.InvariantCulture));
            builder.AppendFormat("ProtocolTotal: {0}\n", log.ProtocolTotal);
            builder.AppendFormat("ProtocolRegister: {0}\n", log.ProtocolRegister);
            builder.AppendFormat("ProtocolError: {0}\n", log.ProtocolError);
            builder.AppendFormat("ProtocolCancelled: {0}\n", log.ProtocolCancelled);
            builder.AppendFormat("ProtocolActive: {0}\n", log.ProtocolActive);
            builder.AppendFormat("ProtocolOthers: {0}\n", log.ProtocolOthers);
            builder.AppendFormat("IdDocument: {0}\n", log.IdDocument);
            builder.AppendFormat("idLocation: {0}\n", log.Location != null ? log.Location.Id.ToString(CultureInfo.InvariantCulture) : String.Empty);
            builder.AppendFormat("LogDescription: {0}\n", log.LogDescription);
            return builder.ToString();
        }

        /// <summary>
        /// Elimina i file temporanei generati sul disco.
        /// </summary>
        public void DiscardTempFiles()
        {
            var pattern = TempName + "*";
            foreach (var destination in Directory.GetFiles(OutputPath, pattern).Select(fileName => Path.Combine(OutputPath, fileName)))
            {
                File.Delete(destination);
            }
        }
        #endregion
    }
}
