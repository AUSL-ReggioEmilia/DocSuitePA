using Microsoft.Reporting.WebForms;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Helpers.iTextSharp;
using VecompSoftware.Helpers.Reporting.ReportingEx;
using VecompSoftware.Services.Reporting;
using VecompSoftware.Services.Reporting.CommonEx;

namespace VecompSoftware.CompliantPrinting
{
    public static class ISignContentEx
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ISignContentEx));

        public static PdfContentInfo PrintSignContentReport(this ISignContent source, bool enableErrorSection)
        {
            if (!source.HasSignatures)
            {
                return null;
            }

            logger.Debug(string.Format("enableErrorSection: {0}", enableErrorSection));

            ReportParameter reportParameter = null;
            if (enableErrorSection)
            {
                reportParameter = ReportParameterEx.Add("ErrorText", "Contenuto da controllare rispetto all’originale: Errori rilevati nella conversione nel formato PDF/A");
            }

            return BiblosDSReportFactory.GetReport<SignContentReport>()
                                        .RenderToPdf(source.GetSignContentReports(),
                                            reportParameter
                                        );
        }
    }
}
