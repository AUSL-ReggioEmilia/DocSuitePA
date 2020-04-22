using System.Collections.Generic;
using System.IO;
using System.Web;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Report;

namespace VecompSoftware.DocSuiteWeb.Facade.Report
{
    public static class ReportFacade
    {
        /// <summary>
        /// Genera un report a partire da un DataSet grezzo (es. una lista di elementi oppure una griglia Telerik)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawDataSet"></param>
        /// <param name="columns"> </param>
        /// <returns></returns>
        public static IReport<T> GenerateReport<T>(IList<T> rawDataSet, IList<Column> columns)
        {
            return GenerateReport(string.Empty, null, null, rawDataSet, columns);
        }

        /// <summary>
        /// Genera un report senza definire quale template utilizzare e passando la definizione delle colonne da utilizzare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="elements"></param>
        /// <param name="columns">Definizione delle colonne da utilizzare per l'esportazione</param>
        /// <returns></returns>
        public static IReport<T> GenerateReport<T>(Dictionary<string, string> parameters, IList<T> elements, IList<Column> columns)
        {
            return GenerateReport(string.Empty, parameters, elements, null, columns);
        }

        /// <summary>
        /// Genera un report a partire da uno specifico RDLC da utilizzare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rdlcReport"></param>
        /// <param name="parameters"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IReport<T> GenerateReport<T>(FileInfo rdlcReport, Dictionary<string, string> parameters, IList<T> elements)
        {
            return GenerateReport(rdlcReport.FullName, parameters, elements);
        }

        /// <summary>
        /// Genera un report a partire da un nome di report (presente in libreria) da utilizzare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reportName"></param>
        /// <param name="parameters"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IReport<T> GenerateReport<T>(string reportName, Dictionary<string, string> parameters, IList<T> elements)
        {
            return GenerateReport(reportName, parameters, elements, null, null);
        }

        /// <summary>
        /// Genera un report a partire da un nome di report (presente in libreria) da utilizzare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reportName"></param>
        /// <param name="parameters"></param>
        /// <param name="elements"></param>
        /// <param name="rawDataSet">DataSetGrezzo ovvero da convertire </param>
        /// <param name="columns">Definizione delle colonne da utilizzare per l'esportazione</param>
        /// <returns></returns>
        public static IReport<T> GenerateReport<T>(string reportName, Dictionary<string, string> parameters, IList<T> elements, IList<T> rawDataSet, IList<Column> columns)
        {
            var report = GetManager<T>();
            if (!string.IsNullOrEmpty(reportName)) report.RdlcPrint = ResolveRdlcPath(reportName);
            if (parameters != null) report.AddParameters(parameters);
            if (columns != null) report.CustomColumns = columns;
            if (elements != null) report.AddRange(elements);
            if (rawDataSet!=null) report.RawDataSet = rawDataSet;
            return report;
        }

        /// <summary>
        /// Definisce quale sia il gestore da utilizzare per la generazione del Report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IReport<T> GetManager<T>()
        {
            if (typeof(T) == typeof(Protocol)) return (IReport<T>)new ProtocolReport();
            if (typeof(T) == typeof(Collaboration)) return (IReport<T>)new CollaborationReport();
            if (typeof(T) == typeof(Resolution)) return (IReport<T>)new ResolutionReport();
            return new GenericDataSetGridReport<T>();
        }

        /// <summary>
        /// Carica il path effettivo dell'RDLC per la generazione del report
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        private static string ResolveRdlcPath(string reportName)
        {
            // Carico il path del report (ipotizzando di ricevere un percorso diretto all'RDLC)
            var rdlcPath = reportName;

            // Se non esiste il file significa che è stato mandato il nome del report da ricercare nella library
            if (!File.Exists(rdlcPath))
            {
                var reportLibraryPath = DocSuiteContext.Current.ProtocolEnv.ReportLibraryPath;

                // Carico la directory principale dei report. Se non esiste allora la carico
                if (!Directory.Exists(reportLibraryPath))
                {
                    reportLibraryPath = HttpContext.Current.Server.MapPath(reportLibraryPath);
                }

                rdlcPath = Path.Combine(reportLibraryPath, reportName);

                // Se il file non esiste ancora è possibile che manchi l'estensione
                if (!File.Exists(rdlcPath)) rdlcPath = Path.Combine(rdlcPath, ".rdlc");
            }

            return rdlcPath;
        }
    }
}
