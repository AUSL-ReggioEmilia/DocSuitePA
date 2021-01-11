using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Preservations;
using BiblosDS.LegalExtension.AdminPortal.Helpers;
using BiblosDS.LegalExtension.AdminPortal.Models;
using BiblosDS.Library.Common.Utility;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Services.Preservations.Interactors
{
    public class CreatePreservationVerifyReportInteractor : IInteractor<PreservationVerifyReportRequestModel, PreservationVerifyReportResponseModel>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly string _reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
        #endregion

        #region [ Constructor ]
        public CreatePreservationVerifyReportInteractor(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public PreservationVerifyReportResponseModel Process(PreservationVerifyReportRequestModel request)
        {
            try
            {
                _logger.Info("Process -> creating report for verifiy action");
                IEnumerable<VerifyReportArchive> verifyReportArchives = request.VerifyModel.jobs.GroupBy(p => p.archiveName, (name, items) => new
                {
                    archiveName = name,
                    jobs = items
                })
                .Select(s => new VerifyReportArchive()
                {
                    ArchiveName = s.archiveName,
                    Preservations = s.jobs.Select(job => new VerifyReportPreservation
                    {
                        IdPreservation = job.idPreservation,
                        Name = job.preservationLabel ?? string.Empty,
                        Result = (job.result ?? string.Empty) == "ok" ? "Verifica positiva" : "Verifica NEGATIVA",
                        Errors = (job.errors ?? string.Empty).Replace("-", "").Replace("<br/>", "")
                    }).ToArray()
                });

                VerifyReport report = new VerifyReport()
                {
                    Archives = verifyReportArchives.ToArray(),
                    fromDate = request.VerifyModel.fromDate.ToString("dd/MM/yyyy"),
                    toDate = request.VerifyModel.toDate.ToString("dd/MM/yyyy"),
                    verifyDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };

                string xmlFilename = Path.Combine(_reportPath, String.Format("Verifica_cons_chiuse_dal_{0}_al_{1}.xml", request.VerifyModel.fromDate.ToString("dd-MM-yyyy"), request.VerifyModel.toDate.ToString("dd-MM-yyyy")));

                if (File.Exists(xmlFilename))
                {
                    File.Delete(xmlFilename);
                }

                report.SaveAs(xmlFilename);

                //crea html
                string html = XslFile.TransformFile(xmlFilename, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/reports", "VerifyReport.xsl"));

                //converte e salva in pdf
                string pdfFileName = string.Concat(Path.GetFileNameWithoutExtension(xmlFilename), ".pdf");
                string pdfFullName  = Path.Combine(_reportPath, pdfFileName);
                if (File.Exists(pdfFullName))
                {
                    File.Delete(pdfFullName);
                }

                Html2Pdf(html, pdfFullName);

                if (File.Exists(xmlFilename))
                {
                    File.Delete(xmlFilename);
                }

                _logger.Info($"Process -> report generated correctly: {pdfFileName}");
                return new PreservationVerifyReportResponseModel() { Response = "Report generato con successo", FileName = pdfFileName };
            }
            catch (Exception ex)
            {
                return new PreservationVerifyReportResponseModel() { Response = $"Errore durante la generazione del report: {ex.Message}", FileName = "error" };
            }
        }

        private void Html2Pdf(string htmlData, string pdfFilename)
        {
            using (MemoryStream ms = new MemoryStream())
            using (TextReader textReader = new StringReader(htmlData))
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();

                PdfWriter pdfWriter = PdfWriter.GetInstance(document, ms);
                pdfWriter.CloseStream = false; // say to PdfWriter: do not close the MemoryStream

                HTMLWorker htmlWorker = new HTMLWorker(document);

                document.Open();
                htmlWorker.StartDocument();
                htmlWorker.Parse(textReader);

                htmlWorker.EndDocument();
                htmlWorker.Close();
                document.Close();

                ms.Position = 0; // reset position for further read
                using (FileStream fs = System.IO.File.OpenWrite(pdfFilename))
                {
                    using (ms)
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }
        #endregion        
    }
}