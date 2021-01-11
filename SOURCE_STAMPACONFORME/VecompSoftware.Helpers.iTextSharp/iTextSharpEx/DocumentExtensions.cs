using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Helpers.iTextSharp.CommonEx;

namespace VecompSoftware.Helpers.iTextSharp.iTextSharpEx
{
    public static class DocumentExtensions
    {

        public static void CloseQuietly(this Document source)
        {
            if (source == null)
                return;
            source.Close();
        }

        public static void NewPage(this Document source, Rectangle pageSize)
        {
            source.SetPageSize(pageSize);
            source.NewPage();
        }

        public static DocumentWrapper LoadFrom(this DocumentWrapper source, IPdfContent pdf, PdfTransformer tx)
        {
            PdfReader.unethicalreading = tx.UnethicalReading;

            PdfReader reader = null;
            PdfWriter writer = null;
            MemoryStream ms = null;
            try
            {
                reader = pdf.ToPdfContent().GetReader();
                source.IsEncrypted = reader.IsEncrypted();

                using (ms = new MemoryStream())
                using (writer = PdfWriter.GetInstance(source, ms))
                {
                    if (tx.PdfA)
                    {
                        writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                        writer.PDFXConformance = PdfWriter.PDFX1A2001;
                        writer.CreateXmpMetadata();
                    }

                    source.Open();
                    foreach (var p in reader.PageNumbers())
                    {
                        PdfImportedPage sourcePage = null;
                        try
                        {
                            sourcePage = writer.GetImportedPage(reader, p);
                        }
                        catch (ArgumentException)
                        {
                            PdfReader.unethicalreading = true;
                            sourcePage = writer.GetImportedPage(reader, p);
                            PdfReader.unethicalreading = tx.UnethicalReading;
                        }
                        
                        var sourcePageSize = reader.GetPageSizeWithRotation(p);
                        var template = tx;
                        template.SetOrigin(sourcePage, sourcePageSize);

                        if (reader.HasAnnotations(p))
                        {
                            Debug.WriteLine("Trovate annotazioni a pagina {0}, lo scaling del contenuto verrà ignorato.", p);
                            template.ClearScaling();
                        }

                        source.NewPage(template.DestinationPageSize);
                        writer.DirectContent.TransformTo(template);
                    }
                    source.Close();
                    source.Content = ms.ToDeepCopyArray();
                    return source;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }
            finally
            {
                source.CloseQuietly();
                ms.CloseQuietly();
                writer.CloseQuietly();
                reader.CloseQuietly();
            }
        }
        public static DocumentWrapper LoadFrom(this DocumentWrapper source, IPdfContent pdf)
        {
            return source.LoadFrom(pdf, PdfTransformerFactory.Default());
        }

        public static DocumentWrapper LoadFrom(this DocumentWrapper source, IEnumerable<IPdfContent> pdfs)
        {
            source.IsEncrypted = false;

            MemoryStream ms = null;
            PdfCopy pdfCopy = null;
            PdfReader reader = null;
            try
            {
                using (ms = new MemoryStream())
                using (pdfCopy = new PdfCopy(source, ms))
                {
                    source.Open();
                    foreach (var item in pdfs)
                    {
                        reader = item.ToPdfContent().GetReader();
                        source.IsEncrypted = source.IsEncrypted.Value || reader.IsEncrypted();
                        pdfCopy.AddAllPages(ref reader);
                        reader.Close();
                    }
                    source.Close();
                    source.Content = ms.ToDeepCopyArray();
                    return source;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                reader.CloseQuietly();
                source.CloseQuietly();
                pdfCopy.CloseQuietly();
                ms.CloseQuietly();
            }
        }

    }
}
