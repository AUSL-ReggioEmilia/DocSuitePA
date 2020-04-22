using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;
using Parameter = VecompSoftware.JeepService.Common.Parameter;

namespace VecompSoftware.JeepService
{
    public class DSWProtocolJournal : JeepModuleBase<DSWProtocolJournalParameters>
    {
        #region [  Fields. ]

        private ProtocolJournalLogFacade _logFacade;
        private LocationFacade _locationFacade;
        private ProtocolFacade _protocolFacade;

        #endregion

        #region [ Properties. ]

        private ProtocolJournalLogFacade LogFacade
        {
            get { return _logFacade ?? (_logFacade = new ProtocolJournalLogFacade()); }
        }
        
        private LocationFacade LocationFacade
        {
            get { return _locationFacade ?? (_locationFacade = new LocationFacade()); }
        }

        private ProtocolFacade ProtocolFacade
        {
            get { return _protocolFacade ?? (_protocolFacade = new ProtocolFacade()); }
        }
        #endregion

        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);

            // Inizializzo gli strumenti PJ
            InitializeJournalTools();
        }

        public override void SingleWork()
        {
            // OPERATION
            FileLogger.Info(Name, "INIZIO ESECUZIONE...");
            RestoreAndCreateJournals();
            FileLogger.Info(Name, "FINE ESECUZIONE.");
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            GC.Collect(GC.MaxGeneration);
        }

        private void InitializeJournalTools()
        {
            ProtocolJournalTools.SignEngineUrl = Parameters.SignEngineUrl;
            FileLogger.Info(Name, "SignEngineUrl: " + ProtocolJournalTools.SignEngineUrl);

            ProtocolJournalTools.SignEngineUser = Parameters.SignEngineUser;
            FileLogger.Info(Name, "SignEngineUser: " + ProtocolJournalTools.SignEngineUser);

            ProtocolJournalTools.SignEnginePassword = Parameters.SignEnginePassword;
            FileLogger.Info(Name, "SignEnginePassword: " + ProtocolJournalTools.SignEnginePassword);

            ProtocolJournalTools.CertificateName = Parameters.CertificateName;
            ProtocolJournalTools.InfocamereFormat = Parameters.InfoCamereFormat;

            ProtocolJournalTools.InitializeSignEngine();
        }
        private ProtocolJournalBuilder InitializeJournalBuilder()
        {
            var retval = new ProtocolJournalBuilder { OutputPath = Parameters.OutputPath };

            if (!Directory.Exists(retval.OutputPath))
                throw new FileNotFoundException("Percorso OutputPath non trovato: " + retval.OutputPath);
            FileLogger.Info(Name, "OutputPath: " + retval.OutputPath);

            retval.TemplatePath = Parameters.TemplatePath;
            if (!File.Exists(retval.TemplatePath))
                throw new FileNotFoundException("Percorso TemplatePath non trovato: " + retval.TemplatePath);
            FileLogger.Info(Name, "TemplatePath: " + retval.TemplatePath);

            retval.ApplySign = Parameters.ApplySign;
            FileLogger.Info(Name, "ApplySign: " + retval.ApplySign.ToString());

            retval.ApplyTimeStamp = Parameters.ApplyTimeStamp;
            FileLogger.Info(Name, "ApplyTimeStamp: " + retval.ApplyTimeStamp.ToString());

            FileLogger.Info(Name, "Location: " + Parameters.Location.ToString(CultureInfo.InvariantCulture));
            retval.Location = LocationFacade.GetById(Parameters.Location);
            if (retval.Location == null)
                throw new ArgumentNullException("La location specificata è inesistente.", new Exception());

            retval.ChainObjectDateFormat = Parameters.DateFormat;
            FileLogger.Info(Name, "ChainObjectDateFormat: " + retval.ChainObjectDateFormat);

            // Parametri necessari per la firma.
            if (retval.ApplySign)
            {
                retval.CertificateName = Parameters.CertificateName;
                FileLogger.Info(Name, "CertificateName: " + retval.CertificateName);
            }

            // Parametri necessari per la marcatura temporale.
            if (retval.ApplyTimeStamp)
            {
                retval.InfoCamereFormat = Parameters.InfoCamereFormat;
                FileLogger.Info(Name, "InfoCamereFormat: " + retval.InfoCamereFormat);
            }

            // Imposto il Logger
            retval.LoggerName = Name;

            return retval;
        }

        private DateTime GetLowerDateLimit()
        {
            // Imposto la data di limite inferiore da file di configurazione.
            return Parameters.LowerDateLimit;
        }
        private DateTime GetStartDate()
        {
            // Recupero la data di creazione dell'ultimo log creato.
            var last = LogFacade.GetLastLogDate();
            if (last.HasValue)
            {
                // Resetto i minuti
                last = last.Value.Date;
            }
            // Se ho configurato un limite inferiore di esecuzione e:
            // Non ho nessun log precedentemente creato
            // OPPURE
            // La data di creazione dell'ultimo log è antecedente alla data di limite inferiore
            // SEGUE
            // Imposto la data di limite inferiore da file di configurazione.
            var lowerDateLimit = GetLowerDateLimit();
            if (!last.HasValue || last.Value < lowerDateLimit)
                last = lowerDateLimit;
            FileLogger.Info(Name, "Data ultimo registro inserito: " + last.Value.ToString("dd/MM/yyyy"));

            // Imposto la data di avvio creazione dei registri.
            var retval = last.Value.AddDays(1);

            return retval;
        }

        /// <summary>
        /// Restituisce il limite superiore di elaborazione.
        /// </summary>
        /// <param name="start">Limite inferiore di elaborazione.</param>
        /// <returns>Data dell'ultima elaborazione da fare. Compresa nel periodo di elaborazione.</returns>
        private DateTime GetEndDate(DateTime start)
        {
            // Imposto di quanti giorni indietro sarà la data di limite superiore.
            // Data calcolata con upperDayLimit: il valore è COMPRESO nel ciclo
            var retval = DateTime.Today.AddDays(Parameters.UpperDayLimit * -1);

            // Verifico se la data di stop del batch è inferiore a quella prestabilita di limite superiore.
            // Data calcolata con batchDayLimit: il valore è NON COMPRESO nel ciclo, sotraggo 1 per omogeneità di retval.
            var batchEndDate = start.AddDays(Parameters.BatchDayLimit - 1);
            if (batchEndDate < retval)
                retval = batchEndDate;
            return retval;
        }

        private static string JournalListToString(ICollection<ProtocolJournalLog> list)
        {
            var tmp = new string[list.Count];
            byte i = 0;
            foreach (var journal in list)
            {
                tmp[i] = string.Format("Id={0}, JournalDate={1}", journal.Id, journal.ProtocolJournalDate);
                i++;
            }
            return string.Join("; ", tmp);
        }


        /// <summary>
        /// Crea un registro giornaliero di protocollo per la data specificata.
        /// </summary>
        /// <param name="pjb">Builder del registro</param>
        /// <param name="date">Data di cui creare il registro</param>
        private void BuildJournalByDate(ref ProtocolJournalBuilder pjb, DateTimeOffset date)
        {
            try
            {
                // Elimino dai protocolli per la data di cui creare il registro ogni riferimento a precedenti registri.
                LogFacade.ClearProtocolJournalReferencesByDate(date);
            }
            catch (Exception ex)
            {
                string err = "Errore in fase ripristino registro: ClearProtocolJournalReferencesByDate, " + date.Date.ToShortDateString();
                FileLogger.Error(Name, err);
                throw new Exception(err, ex);
            }

            try
            {
                // Imposto la data
                pjb.Date = date.Date;

                // Verifico se devo annullare protocolli
                if (Parameters.CancelBrokenProtocols)
                {
                    FileLogger.Info(Name, "Verifica protocolli da annullare...");
                    foreach (var protocol in pjb.Protocols.Where(
                        protocol => protocol.IdStatus.HasValue && (protocol.IdStatus.Value == (int)ProtocolStatusId.Errato) &&
                        (DateTimeOffset.Now - protocol.RegistrationDate).TotalDays > 0)
                        )
                    {
                        var currentProtocol = protocol;
                        currentProtocol.IdStatus = (int?)ProtocolStatusId.Annullato;
                        currentProtocol.LastChangedReason = Parameters.CancelBrokenProtocolMessage.Trim();
                        ProtocolFacade.Update(ref currentProtocol);
                        FileLogger.Info(Name, String.Format("Annullato protocollo {0}", protocol));
                    }
                }

                FileLogger.Info(Name, "Creazione registro per la data del: " + date.ToString("dd/MM/yyyy"));
                FileLogger.Info(Name, "Protocolli da registrare: " + pjb.Protocols.Count);

                //FileLogger.Info(Name, "Attributi ChainObject: " + pjb.ChainObjectToString());

                // Costruisco il registro.
                pjb.Build();
                FileLogger.Info(Name, "Creazione registro completata.");
                try
                {
                    FileLogger.Info(Name, "Elimino file temporanei: " + pjb.TempName);
                    // Se va tutto a buon fine elimino i file temporanei.
                    pjb.DiscardTempFiles();
                }
                catch (Exception discardTempFilesEx)
                {
                    // Se si verifica un errore in fase di eliminazione dei file temporanei comunque non dismetto il registro creato correttamente.
                    FileLogger.Info(Name, "Si è verificato un errore in: CreateJournals.DiscardTempFiles");
                    FileLogger.Error(Name, discardTempFilesEx.Message, discardTempFilesEx);
                }
            }
            catch (Exception buildEx)
            {
                SendMessage("Errore in creazione Journal Report per il giorno " + date.Date.ToLongDateString());
                FileLogger.Info(Name, "Si è verificato un errore in: CreateJournals.Build");
                FileLogger.Error(Name, buildEx.Message, buildEx);
                FileLogger.Info(Name, "File temporanei preservati per verifiche: " + pjb.TempName);
                throw new Exception("Errore in creazione Journal Report per il giorno " + date.Date.ToLongDateString());
            }
        }

        /// <summary>
        /// Recupera i registri incompleti e ne tenta la rigenerazione.
        /// </summary>
        /// <param name="pjb">Builder del registro</param>
        /// <param name="start">Data limite inferiore di verifica</param>
        /// <param name="end">Data limite superiore di verifica</param>
        private void RestoreJournals(ref ProtocolJournalBuilder pjb, DateTime start, DateTime end)
        {
            // Recupero i registri la cui creazione è rimasta incompleta.
            var brokenLogs = LogFacade.GetBrokenJournals(start, end);
            if (brokenLogs != null && brokenLogs.Count > 0)
            {
                FileLogger.Info(Name, "Registri giornalieri incompleti da riprocessare: " + brokenLogs.Count.ToString(CultureInfo.InvariantCulture));
                var joined = JournalListToString(brokenLogs);
                FileLogger.Info(Name, "Registri da riprocessare: " + joined);

                foreach (var journal in brokenLogs)
                {
                    if (Cancel)
                    {
                        FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                        return;
                    }

                    if (!CheckTimeStamps())
                    {
                        FileLogger.Info(Name, "Recupero Registro annullato per problemi in Verifica Marche Temporali");
                        continue;
                    }

                    try
                    {
                        // Elimino il registro incompleto per predisporne la rigenerazione.
                        var discarded = journal;
                        if (discarded.IdDocument.HasValue)
                        {
                            Services.Biblos.Service.DetachDocument(discarded.Location.DocumentServer, discarded.Location.ProtBiblosDSDB, 
                                discarded.IdDocument.Value);
                        }
                        LogFacade.Delete(ref discarded);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, "Errore in cancellazione registro da ripristinare.", ex);
                        SendMessage("Errore in cancellazione registro da ripristinare: " + ex);
                        return;
                    }

                    try
                    {
                        if (journal.ProtocolJournalDate != null)
                            BuildJournalByDate(ref pjb, journal.ProtocolJournalDate.Value);
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, "Errore in restoreJournals", ex);
                        SendMessage("Errore in restoreJournals: " + ex);
                    }
                }
            }
            else
                FileLogger.Info(Name, "Nessun registro incompleto da riprocessare.");
        }

        private bool CheckTimeStamps()
        {
            try
            {
                if (Parameters.ApplyTimeStamp)
                {
                    var message = "Sono state usate {0} marche temporali. Sono disponibili {1} marche.";
                    message = string.Format(message, ProtocolJournalTools.SignEngine.UsedTimeStamps, ProtocolJournalTools.SignEngine.AvailableTimeStamps);
                    FileLogger.Info(Name, message);
                }

                if (Parameters.ApplyTimeStamp && !ProtocolJournalTools.SignEngine.HasAvailableTimeStamps(Parameters.TimeStampWarningThreshold))
                {
                    var message = "Operazione interrotta per: Marche temporali in esaurimento. Sono disponibili {0} marche.";
                    message = string.Format(message, ProtocolJournalTools.SignEngine.AvailableTimeStamps);
                    FileLogger.Error(Name, message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Crea i registri nell'intervallo di date specificato.
        /// </summary>
        /// <param name="pjb">Builder del registro</param>
        /// <param name="start">Data limite inferiore di creazione</param>
        /// <param name="end">Data limite superiore di creazione</param>
        private void CreateJournals(ref ProtocolJournalBuilder pjb, DateTime start, DateTime end)
        {
            if (start.AddDays(-1) == end)
            {
                FileLogger.Info(Name, "Nessuna data da elaborare nell'intervallo indicato.");
                return;
            }

            if (end < start)
            {
                var message = "La data di limite superiore ({0}) è antecedente alla data di limite inferiore ({1}).";
                message = string.Format(message, end, start);
                throw new InvalidOperationException(message);
            }

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (Cancel)
                {
                    FileLogger.Info(Name, "Chiusura modulo invocata dall'utente.");
                    return;
                }

                if (!CheckTimeStamps())
                {
                    FileLogger.Info(Name, "Elaborazione Registro annullata per problemi in Verifica Marche Temporali");
                    continue;
                }

                try
                {
                    BuildJournalByDate(ref pjb, date);
                    SendMessage(string.Format("REGISTRO CREATO CORRETTAMENTE ({0:dddd dd/MM/yyyy}) - Protocolli registrati: {1}", date, pjb.Protocols.Count));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(Name, "Errore in createJournals", ex);
                    SendMessage("Errore in createJournals: " + ex);
                }
            }
        }

        /// <summary>
        /// Esegue il recupero e la creazione di registri giornalieri di protocollo.
        /// </summary>
        private void RestoreAndCreateJournals()
        {
            ProtocolJournalBuilder pjb;
            try
            {
                pjb = InitializeJournalBuilder();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Si è verificato un errore in: initializeJournalBuilder", ex);
                SendMessage("Si è verificato un errore in: initializeJournalBuilder, " + ex);
                return;
            }

            DateTime start, end, lowerDateLimit;
            try
            {
                start = GetStartDate();
                FileLogger.Info(Name, "Data avvio creazione registro: " + start.ToString("dd/MM/yyyy"));

                end = GetEndDate(start);
                FileLogger.Info(Name, "Data fine elaborazione batch corrente: " + end.ToString("dd/MM/yyyy"));

                lowerDateLimit = GetLowerDateLimit();
                FileLogger.Info(Name, "Data di limite inferiore: " + lowerDateLimit.ToString("dd/MM/yyyy"));
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Si è verificato un errore in fase di recupero date di elaborazione.", ex);
                return;
            }

            if (Parameters.RestoreBrokenLogs)
            {
                RestoreJournals(ref pjb, lowerDateLimit, end);
            }
            CreateJournals(ref pjb, start, end);
        }

    }
}
