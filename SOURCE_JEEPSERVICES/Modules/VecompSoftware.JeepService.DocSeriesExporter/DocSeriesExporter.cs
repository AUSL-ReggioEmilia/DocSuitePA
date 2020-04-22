using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using VecompSoftware.JeepService.Common;
using VecompSoftware.JeepService.DocSeriesExporter.Repository;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.DocSeriesExporter
{
    public class DocSeriesExporter : JeepModuleBase<DocSeriesExporterParameters>
    {
        #region Fields

        private Lazy<WsSeriesConnector> _wsSeriesConnector;
        #endregion

        #region Properties

        public WsSeriesConnector WsSeriesConnector
        {
            get { return _wsSeriesConnector.Value; }
        }

        #endregion

        private WsSeriesConnector InitConnector()
        {
            return new WsSeriesConnector(Parameters.WsSeriesUrl);
        }

        public override void Initialize(List<Parameter> parameters)
        {
            base.Initialize(parameters);

            ValidateParameters();

            this._wsSeriesConnector = new Lazy<WsSeriesConnector>(InitConnector);
        }

        private void ValidateParameters()
        {
            switch (Parameters.Repository)
            {
                case RepositoryType.FileSystem:
                    if (!Directory.Exists(Parameters.FileSystemPath))
                    {
                        FileLogger.Error(Name, string.Format("Errore in [Initialize] - Il percorso {0} non esiste.", Parameters.FileSystemPath));
                        throw new DirectoryNotFoundException();
                    }
                    break;
                case RepositoryType.Sharepoint:
                    try
                    {
                        if (String.IsNullOrEmpty(Parameters.SpUser))
                        {
                            throw new Exception(String.Format("Errore in [Initialize] - Parametro {0} non valorizzato.", Parameters.SpUser));
                        }
                        if (String.IsNullOrEmpty(Parameters.SpPassword))
                        {
                            throw new Exception(String.Format("Errore in [Initialize] - Parametro {0} non valorizzato.", Parameters.SpPassword));
                        }
                        if (String.IsNullOrEmpty(Parameters.SpSiteCollection))
                        {
                            throw new Exception(String.Format("Errore in [Initialize] - Parametro {0} non valorizzato.", Parameters.SpSiteCollection));
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, ex.Message);
                        throw;
                    }
                    break;
                default:
                    FileLogger.Error(Name, "Errore in [Initialize] - Repository non riconosciuto.");
                    throw new ArgumentException("Repository non riconosciuto.");
            }
        }

        private bool CancelRequest()
        {
            return Cancel;
        }

        public override void SingleWork()
        {
            FileLogger.Info(Name, "Inizio procedura di esportazione serie documentali.");

            IExportRepository wrapper = null;
            switch (Parameters.Repository)
            {
                case RepositoryType.FileSystem:
                    wrapper = new FileSystemRepository(Parameters);
                    break;
                case RepositoryType.Sharepoint:
                    wrapper = new SharepointRepository(Parameters);
                    break;
            }

            var families = this.WsSeriesConnector.GetFamilyWsos();
            foreach (var family in families)
            {
                if (CancelRequest())
                    return;

                foreach (var series in family.DocumentSeries)
                {
                    try
                    {
                        if (CancelRequest())
                            return;

                        Thread.Sleep(3000);

                        FileLogger.Info(Name, string.Format("Processo la serie documentale con ID {0} - {1}.", series.Id, series.Name));

                        FileLogger.Info(Name, "Recupero le serie documentali associate.");
                        var docItems = this.WsSeriesConnector.GetSeriesWsosById(series.Id);
                        var dynamicData = this.WsSeriesConnector.GetDynamicAttributeWsos(series.Id);

                        FileLogger.Info(Name, "Inizializzo export dati in CSV della serie");

                        wrapper.InitializeExport(docItems, dynamicData);

                        FileLogger.Info(Name, string.Format("Eseguo l'esportazione dei dati in CSV della serie con ID {0}- {1}.", series.Id, series.Name));
                        wrapper.StartExport(series.Id);

                        if (series.DocumentSeriesSubsections != null && series.DocumentSeriesSubsections.Count > 0)
                        {
                            foreach (var section in series.DocumentSeriesSubsections)
                            {
                                FileLogger.Info(Name, "Recupero le serie documentali associate alla sottosezione.");
                                var items = docItems.Where(s => s.IdDocumentSeriesSubsection == section.Id).ToList();

                                FileLogger.Info(Name, string.Format("Inizializzo export dati in CSV della sottosezione con ID {0}", section.Id));

                                wrapper.InitializeExport(items, dynamicData);

                                FileLogger.Info(Name, string.Format("Eseguo l'esportazione dei dati in CSV della sottossezione con ID {0}", section.Id));
                                wrapper.StartExport(section.Id, true);
                            }
                        }

                        FileLogger.Info(Name, string.Format("Serie documentale con ID {0} processata correttamente.", series.Id));

                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error(Name, string.Format("Serie documentale {0} processata con errori.", series.Name), ex);
                    }
                }                
            }
            
            FileLogger.Info(Name, "Fine procedura di esportazione serie documentali.");
        }
    }
}
