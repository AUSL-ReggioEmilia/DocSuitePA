using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using VecompSoftware.DocSuiteWeb.Report.Dynamic;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Report
{
    public abstract class ReportBase<T> : IReport<T>
    {
        #region [ Fields ]

        private ReportViewer _tablePrint;
        private List<T> _items;

        #endregion

        #region [ Properties ]

        protected static string LoggerName { get { return "FileLog"; } }

        public ReportViewer TablePrint
        {
            get { return _tablePrint ?? (_tablePrint = new ReportViewer()); }
        }
        public string TitlePrint { get; set; }
        public string RdlcPrint { get; set; }
        public IList<Column> CustomColumns { get; set; }
        public IList<T> RawDataSet { get; set; }
        public DataSet DataSource { get; set; }

        public string PrimaryTableName { get; set; }
        public string SubreportTableName { get; set; }

        public string RdlcFilenameSuffix { get; set; }
        private Dictionary<string, string> ReportParameters { get; set; }

        public ReportType ReportType { get; set; }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// Obbligo le classi derivate ad implementare il parametro che definisce il tipo di report --> sul quale definisco come generare i dati
        /// </summary>
        /// <param name="reportType"></param>
        protected ReportBase(ReportType reportType)
        {
            ReportType = reportType;
        }

        #endregion

        #region [ Methods ]

        public virtual void AddParameter(string name, string value)
        {
            if (ReportParameters == null) ReportParameters = new Dictionary<string, string>();
            ReportParameters.Add(name, value);
        }

        public virtual void AddParameters(Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                AddParameter(parameter.Key, parameter.Value);
            }
        }

        private bool ReportHasParameters()
        {
            return ReportParameters != null && ReportParameters.Count > 0;
        }

        protected virtual List<ReportParameter> GetReportParameters()
        {
            return ReportHasParameters() ? ReportParameters.Select(reportParameter => new ReportParameter(reportParameter.Key, reportParameter.Value)).ToList() : new List<ReportParameter>();
        }

        protected virtual void DataBind()
        {
            throw new NotImplementedException("Metodo virtual non implementato");
        }

        protected virtual IList<T> Items
        {
            get { return _items; }
        }

        public void AddRange(IList<T> elements)
        {
            if (_items == null) _items = new List<T>();
            _items.AddRange(elements);
        }

        protected void LocalReportSubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var protocolIds = e.Parameters["Protocol_ID"];
            string protocolId = null;
            if (protocolIds != null)
            {
                protocolId = protocolIds.Values.FirstOrDefault();
            }
            // Comportamento smart: estraggo solo i record strettamente necessari al subreport e risparmio tempo
            if (protocolId != null)
            {
                try
                {
                    var dataSource = e.DataSourceNames[0];
                    var query =
                        DataSource.Tables[dataSource.Split('_').Last()].AsEnumerable()
                            .Where(r => r.Field<string>("Protocol_ID") == protocolId);

                    DataTable dataTable;
                    // Se ci sono dati li carico, altrimenti prendo il suo dataTable e rimuovo le righe
                    if (query.Any())
                    {
                        dataTable = query.CopyToDataTable();
                    }
                    else
                    {
                        dataTable = DataSource.Tables[dataSource.Split('_').Last()].Copy();
                        dataTable.Rows.Clear();
                        // Aggiungo una riga vuota per forzare il rendering dei subreport
                        dataTable.Rows.Add(dataTable.NewRow());
                    }

                    dataTable.TableName = dataSource.Split('_').Last();
                    e.DataSources.Add(new ReportDataSource(dataSource, dataTable));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LoggerName, string.Format("Errore in fase Subreport processing per il protocollo {0}", protocolId), ex);
                }
            }
            else
            {
                // Comportamento full
                foreach (DataTable table in DataSource.Tables)
                {
                    e.DataSources.Add(new ReportDataSource(string.Format("{0}_{1}", DataSource.DataSetName, table.TableName), table));
                }
            }
        }

        private void InitReportViewer()
        {
            // Popolo il DataSource
            FileLogger.Debug(LoggerName, "ReportBase.InitReportViewer.DataBind");
            DataBind();

            if (DataSource == null || DataSource.Tables.Count == 0)
            {
                throw new Exception("Nessun dato da visualizzare");
            }

            // Svuoto i dataSource per caricarli con gli appositi metodi
            TablePrint.LocalReport.DataSources.Clear();

            // Gestisco la tipologia di report
            FileLogger.Debug(LoggerName, "ReportBase.InitReportViewer.ReportType");
            switch (ReportType)
            {
                case ReportType.Memory:
                    {
                        InitMemoryReportViewer();
                        break;
                    }
                case ReportType.Rdlc:
                    {
                        InitRdlcReportViewer();
                        break;
                    }
            }

            // Gestisco i parametri
            FileLogger.Debug(LoggerName, "InitReportViewer.LoadParameters()");
            LoadParameters();
        }

        private void InitRdlcReportViewer()
        {
            if (!string.IsNullOrEmpty(RdlcFilenameSuffix))
            {
                var temp = RdlcPrint.Replace(".rdlc", string.Format("_{0}.rdlc", RdlcFilenameSuffix));
                if (File.Exists(temp))
                {
                    RdlcPrint = temp;
                }
            }

            if (!File.Exists(RdlcPrint)) throw new FileNotFoundException(string.Format("Impossibile trovare il template {0}", RdlcPrint));
            TablePrint.LocalReport.ReportPath = RdlcPrint;
            TablePrint.LocalReport.EnableExternalImages = true;
            TablePrint.ProcessingMode = ProcessingMode.Local;

            // Aggiungo i DataSource
            var tab = string.IsNullOrEmpty(PrimaryTableName) ? DataSource.Tables[0] : DataSource.Tables[PrimaryTableName];
            TablePrint.LocalReport.DataSources.Add(new ReportDataSource(string.Format("{0}_{1}", DataSource.DataSetName, tab.TableName), tab));

            TablePrint.LocalReport.SubreportProcessing += LocalReportSubreportProcessing;
        }

        private void InitMemoryReportViewer()
        {
            var allFields = GetAvailableFields();
            var showFields = CustomColumns ?? allFields;

            var rdlcMemoryStream = GenerateRdl(allFields, showFields);
            TablePrint.LocalReport.LoadReportDefinition(rdlcMemoryStream);
            TablePrint.LocalReport.DataSources.Add(new ReportDataSource("MyData", DataSource.Tables[0]));
        }

        private IList<Column> GetAvailableFields()
        {
            var dataTable = DataSource.Tables[0];
            var availableFields = new List<Column>();
            for (var i = 0; i < dataTable.Columns.Count; i++) availableFields.Add(new Column(dataTable.Columns[i].ColumnName.Replace(".", "_")));
            return availableFields;
        }

        private static MemoryStream GenerateRdl(IList<Column> allFields, IList<Column> selectedFields)
        {
            var ms = new MemoryStream();
            var gen = new RdlGenerator { AllFields = allFields, SelectedFields = selectedFields };
            gen.WriteXml(ms);
            ms.Position = 0;
            return ms;
        }

        private void LoadParameters()
        {
            if (ReportHasParameters())
            {
                // Carico eventuali parametri solo se presenti nell'RDLC di destinazione
                var rdlcParameters = TablePrint.LocalReport.GetParameters().ToList();

                // Inserisco soltanto i parametri presenti
                if (rdlcParameters.Count <= 0) return;
                var usedParameters =
                    GetReportParameters().Where(
                        reportParameter => rdlcParameters.Exists(p => p.Name == reportParameter.Name)).ToList();
                // Se i parametri richiesti sono maggiori di quelli forniti allora lancio l'eccezione
                if (rdlcParameters.Count > usedParameters.Count)
                    throw new Exception(
                        string.Format("Il report {0} richiede almeno {1} parametri mentre ne sono stati dichiarati {2}",
                                      RdlcPrint, rdlcParameters.Count, usedParameters.Count));

                TablePrint.LocalReport.SetParameters(usedParameters);
            }
        }

        public virtual DocumentInfo DoPrint()
        {
            return DoPrint(string.Empty);
        }

        public virtual DocumentInfo DoPrint(string reportFileName)
        {
            FileLogger.Debug(LoggerName, string.Format("ReportBase.DoPrint [{0}]", reportFileName));
            InitReportViewer();

            FileLogger.Debug(LoggerName, "ReportBase.DoPrint Rendering");
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            var reportByte = TablePrint.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

            SetPrintingProperties(ref reportByte);

            FileLogger.Debug(LoggerName, "Returning...");
            return new MemoryDocumentInfo(reportByte, !string.IsNullOrEmpty(reportFileName) ? reportFileName : "Report.pdf");
        }

        private static void SetPrintingProperties(ref byte[] reportByte)
        {
            using (var reader = new PdfReader(reportByte))
            {
                using (var ms = new MemoryStream())
                {
                    var stamper = new PdfStamper(reader, ms);
                    // Disattivo il ridimensionamento
                    stamper.AddViewerPreference(PdfName.PRINTSCALING, PdfName.NONE);
                    var info = reader.Info;
                    info.AddSafe("Creator", "Dgroove Srl");
                    info.AddSafe("Author", "Dgroove Srl");
                    stamper.MoreInfo = info;
                    stamper.Close();
                    reportByte = ms.ToArray();
                }
            }
        }

        public virtual DocumentInfo ExportExcel()
        {
            return ExportExcel(string.Empty);
        }

        public virtual DocumentInfo ExportExcel(string reportFileName)
        {
            InitReportViewer();

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            var reportByte = TablePrint.LocalReport.Render(
               "Excel",
               null,
               out mimeType,
               out encoding,
               out extension,
               out streamids,
               out warnings
            );

            return new MemoryDocumentInfo(reportByte, !string.IsNullOrEmpty(reportFileName) ? reportFileName : "Report.xls");
        }
        #endregion
    }
}
