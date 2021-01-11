using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using VecompSoftware.Commons;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Infos;
using VecompSoftware.Helpers.iTextSharp.CommonEx;
using VecompSoftware.Helpers.iTextSharp.iTextSharpEx;

namespace VecompSoftware.Helpers.iTextSharp
{
    public static class iTextSharpHelper
    {

        #region [ Methods ]

        public static List<ContentInfo> ExtractPortfolio(IPdfContent pdf)
        {
            try
            {
                using (var reader = pdf.ToPdfContent().GetReader())
                {
                    if (reader.Catalog.IsNullOrEmpty())
                        return null;

                    var catalogNames = reader.Catalog.GetAsDict(PdfName.NAMES);
                    if (catalogNames.IsNullOrEmpty())
                        return null;

                    var embeddedFiles = catalogNames.GetAsDict(PdfName.EMBEDDEDFILES);
                    if (embeddedFiles.IsNullOrEmpty())
                        return null;

                    var embeddedNames = embeddedFiles.GetAsArray(PdfName.NAMES);
                    if (embeddedNames.IsNullOrEmpty())
                        return null;

                    var result = new List<ContentInfo>();
                    for (int i = 1; i < embeddedNames.Size; i += 2)
                    {
                        var name = embeddedNames.GetAsDict(i);
                        if (name.IsNullOrEmpty())
                            continue;

                        var files = name.GetAsDict(PdfName.EF);
                        if (files.IsNullOrEmpty())
                            continue;

                        foreach (var item in files.Keys)
                        {
                            var indirect = files.GetAsIndirectObject(item);
                            if (indirect == null)
                                throw new ArgumentNullException("PdfIndirectReference allegato non valido, impossibile procedere.");
                            var stream = (PRStream)PdfReader.GetPdfObject(indirect);
                            if (stream == null)
                                throw new ArgumentNullException("PRStream allegato non valido, impossibile procedere.");
                            var content = PdfReader.GetStreamBytes(stream);
                            if (content.IsNullOrEmpty())
                                throw new ArgumentNullException("Allegato con dimensione zero, impossibile procedere.");

                            var fileName = name.GetAsString(item).ToUnicodeString().Trim();
                            var contentInfo = new ContentInfo(fileName, content);
                            result.Add(contentInfo);

                            if (!contentInfo.Extension.EqualsIgnoreCase(PathUtil.EXTENSIONPDF))
                                continue;
                            var recursive = ExtractPortfolio(contentInfo.ToPdfContent());
                            if (recursive.IsNullOrEmpty())
                                continue;
                            result.AddRange(recursive);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public static Dictionary<int, string> GetTextFromAllPages(IPdfContent pdf)
        {
            try
            {
                using (var reader = pdf.ToPdfContent().GetReader())
                    return reader.GetTextFromAllPages();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public static IEnumerable<int> GetScannedPageNumbers(IPdfContent pdf)
        {
            try
            {
                using (var reader = pdf.ToPdfContent().GetReader())
                    return reader.GetScannedPageNumbers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public static PdfContentInfo ApplyTransformer(IPdfContent pdf, PdfTransformer tx)
        {
            try
            {
                using (var document = new DocumentWrapper())
                {
                    document.LoadFrom(pdf, tx);
                    var transformed = new byte[document.Content.Length];
                    document.Content.CopyTo(transformed, 0);
                    return new PdfContentInfo(pdf.FileName, transformed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
        public static PdfContentInfo Merge(IEnumerable<IPdfContent> pdfs)
        {
            try
            {
                using (var document = new DocumentWrapper())
                {
                    document.LoadFrom(pdfs);
                    var content = (byte[])document.Content.Clone();
                    return new PdfContentInfo(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        #endregion

    }
}
