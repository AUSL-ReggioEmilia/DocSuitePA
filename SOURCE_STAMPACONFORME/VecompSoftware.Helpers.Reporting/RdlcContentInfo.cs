using System.Collections.Generic;
using System.Linq;
using Microsoft.Reporting.WebForms;
using VecompSoftware.Commons.Infos;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Helpers.Reporting.ReportingEx;

namespace VecompSoftware.Helpers.Reporting
{
    public class RdlcContentInfo : ContentInfo
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(RdlcContentInfo));

        #region [ Constructors ]

        public RdlcContentInfo(string fullName) : base(fullName) { }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportSource">Enumeratore contenente i valori da aggiungere al dataset del report</param>
        /// <param name="parameterSource">parametri da aggiungere al report</param>
        /// <returns></returns>
        public PdfContentInfo RenderToPdf(IEnumerable<IReport> reportSource, ReportParameter parameterSource)
        {
            var viewer = new ReportViewer();
            viewer.LocalReport.ReportPath = FullName;
            viewer.LocalReport.EnableExternalImages = true;
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", reportSource));
            
            if (parameterSource != null)
            {
                logger.Debug(string.Format("parameterSource: {0}", parameterSource));
                viewer.LocalReport.SetParameters(parameterSource);
            }
            else
                logger.Debug(string.Format("parameterSource: NO PARAMETER IS SET"));
            viewer.LocalReport.SubreportProcessing += LocalReportSubreportProcessing;
            
            var content = viewer.LocalReport.Render("PDF", string.Empty);
            return new PdfContentInfo(FileName, content);
        }

        public void LocalReportSubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var master = (LocalReport)sender;
            master.DataSources.ToList().ForEach(s => e.DataSources.Add(s.ApplyParameters(e.Parameters)));
        }

        #endregion

    }
}
